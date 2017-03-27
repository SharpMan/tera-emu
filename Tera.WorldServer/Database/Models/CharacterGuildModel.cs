using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;
using Tera.Libs.Utils;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Database.Models
{
    public class CharacterGuild
    {
        public long ID { get; set; }
        public int Guild { get; set; }
        public int Grade { get; set; }
        public int Restriction { get; set; }
        public long Experience { get; set; }
        public int ExperiencePercent { get; set; }
        public String Name { get; set; }
        public int Level { get; set; }
        public int Gfx { get; set; }
        public int Alignement { get; set; }
        public DateTime lastConnection { get; set; }


        private Guild myGuildReference = null;
        public Guild GuildCache
        {
            get
            {
                if (myGuildReference == null)
                {
                    myGuildReference = GuildTable.GetGuild(this.Guild);
                }
                return myGuildReference;
            }
            set
            {
                this.myGuildReference = value;
            }
        }

        public int getHoursFromLastCo()
        {
            DateTime now = DateTime.Now;
            return CDateTimeUtil.GetDaysBetweenDates(this.lastConnection, now) * 24;
        }

        public string SerializeAs_GuildAddMemberInformationMessage()
        {
            String online = "0";
            if ((this.getPerso() != null) && (this.getPerso().IsOnline()))
            {
                online = "1";
            }
            var Packet = new StringBuilder();

            Packet.Append(this.ID).Append(';');
            Packet.Append(this.Name).Append(';');
            Packet.Append(this.Level).Append(';');
            Packet.Append(this.Gfx).Append(';');
            Packet.Append(this.Grade).Append(';');
            Packet.Append(this.Experience).Append(';');
            Packet.Append(this.ExperiencePercent).Append(';');
            Packet.Append(this.Restriction).Append(';');
            Packet.Append(online).Append(';');
            Packet.Append(this.Alignement).Append(';');
            Packet.Append(this.getHoursFromLastCo()); 

            return Packet.ToString();
        }

        public Player getPerso()
        {
            return CharacterTable.GetCharacter(this.ID);
        }

        public bool Can(GuildRightEnum Right)
        {
            return (this.Restriction & (int)Right) == (int)Right;
        }

        public GuildGradeEnum GradeType
        {
            get
            {
                return (GuildGradeEnum)this.Grade;
            }
        }

        public void SetCan(GuildRightEnum Right, bool CanOrIs)
        {
            if (CanOrIs)
            {
                if (!this.Can(Right))
                    this.Restriction |= (int)Right;
            }
            else
                if (this.Can(Right))
                    this.Restriction ^= (int)Right;
        }

        public void OnResetRights()
        {
            foreach (GuildRightEnum Right in Enum.GetValues(typeof(GuildRightEnum)))
                this.SetCan(Right, false);
        }

        public void SetGuild(Guild Guild, GuildGradeEnum Grade)
        {
            this.SetGrade(Grade);
            this.GuildCache = Guild;
            this.Guild = Guild.ID;
        }

        public void giveXpToGuild(long xp)
        {
            this.Experience += xp;
            this.GuildCache.addXp(xp);
        }

        public void SetGrade(GuildGradeEnum Grade)
        {
            this.Grade = (int)Grade;

            if (Grade == GuildGradeEnum.GRADE_BOSS)
                foreach (GuildRightEnum Right in Enum.GetValues(typeof(GuildRightEnum)))
                    this.SetCan(Right, true);
        }

        public void SendGuildSettingsInfos()
        {
            if (this.ID != -1)
            {
                var chara = CharacterTable.GetCharacter(this.ID);
                if (chara != null)
                    chara.Send(new GuildSettingInformationMessage(this.GuildCache, StringHelper.EncodeBase36(this.Restriction)));
            }
        }



    }
}
