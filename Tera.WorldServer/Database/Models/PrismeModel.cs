using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Fights.Types;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Database.Models
{
    public class Prisme : IGameActor
    {
        public long ActorId { get; set; }
        public int Orientation { get; set; }
        public int CellId { get; set; }
        private StringBuilder mySerializedPattern;
        public int Alignement { get; set; }
        public byte Level { get; set; }
        public short Mapid { get; set; }
        public int inFight = 0; //Todo PrismeStateEnum
        public Fight CurrentFight = null;
        public int TimeTurn = (int)TurnTimeEnum.PRISME;
        public int Honor { get; set; }
        public int Area { get; set; }
        public SpellBook mySpells = null;
        public GenericStats myFightStats;
        private bool myInitialized = false;

        public GameActorTypeEnum ActorType
        {
            get { return GameActorTypeEnum.TYPE_PRISM; }
        }

        public Map Map
        {
            get
            {
                return MapTable.Get(Mapid);
            }
        }

        public AlignmentTypeEnum AlignmentType
        {
            get
            {
                return (AlignmentTypeEnum)this.Alignement;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize()
        {
            if (this.myInitialized)
                return;

            this.mySpells = SpellBook.GenerateForPrisme();

            this.myFightStats = new GenericStats();
            this.myFightStats.AddBase(EffectEnum.AddForce, 1000 + 300 * Level);
            this.myFightStats.AddBase(EffectEnum.AddSagesse, 1000 + 300 * Level);
            this.myFightStats.AddBase(EffectEnum.AddIntelligence, 1000 + 300 * Level);
            this.myFightStats.AddBase(EffectEnum.AddChance, 1000 + 300 * Level);
            this.myFightStats.AddBase(EffectEnum.AddAgilite, 1000 + 300 * Level);
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentNeutre, 9 * Level);
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentFeu, 9 * Level);
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentEau, 9 * Level);
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentAir, 9 * Level);
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentTerre, 9 * Level);
            this.myFightStats.AddBase(EffectEnum.AddEsquivePA, 9 * Level);
            this.myFightStats.AddBase(EffectEnum.AddEsquivePM, 9 * Level);
            this.myFightStats.AddBase(EffectEnum.AddPA, 12);
            this.myFightStats.AddBase(EffectEnum.AddPM, 0);
            this.myFightStats.AddBase(EffectEnum.AddVitalite, 10000);

            this.myInitialized = true;
        }

        public void AddHonor(int honor)
        {
            Honor += honor;
            if (Honor >= 25000)
            {
                Level = 10;
                Honor = 25000;
            }
            for (int n = 1; n <= 10; n++)
            {
                if (Honor < ExpFloorTable.GetFloorByLevel(n).PvP)
                {
                    Level = (byte)(n - 1);
                    break;
                }
            }
        }

        public static void AnalyzeAttack(Player perso)
        {
            foreach (var Prisme in PrismeTable.Cache.Values.Where(x => x.Alignement == perso.Alignement && (x.inFight == 0 || x.inFight == -2)))
            {
                perso.Send(new PrismInformationsAttackMessage(PrismAttackers(Prisme.ActorId, Prisme.CurrentFight)));
            }
        }

        public static void AnalyzeDefense(Player perso)
        {
            foreach (var Prisme in PrismeTable.Cache.Values.Where(x => x.Alignement == perso.Alignement && (x.inFight == 0 || x.inFight == -2)))
            {
                perso.Send(new PrismInformationsDefenseMessage(PrismDefenders(Prisme.ActorId, Prisme.CurrentFight)));
            }
        }

        public static String PrismAttackers(long id, Fight fight)
        {

            StringBuilder str = new StringBuilder("+").Append(id); //StringBuilder str = new StringBuilder("+" + IntHelper.ToString(id, 36));
            if (fight != null)
            {

                foreach (Fighter f in fight.Team1.GetFighters())
                {
                    if (f == null || f.ActorType != GameActorTypeEnum.TYPE_CHARACTER)
                    {
                        continue;
                    }
                    str.Append("|" + IntHelper.toString((int)(f as CharacterFighter).Character.ActorId, 36) + ";");
                    str.Append(f.Name + ";");
                    str.Append(f.Name + ";");
                }

            }
            return str.ToString();
        }

        public static String PrismDefenders(long id, Fight fight)
        {
            StringBuilder str = new StringBuilder("+").Append(id);
            StringBuilder stra = new StringBuilder();
            if (fight != null)
            {
                foreach (Fighter f in fight.Team2.GetFighters())
                {
                    if (f == null || f.ActorType != GameActorTypeEnum.TYPE_CHARACTER)
                    {
                        continue;
                    }
                    str.Append("|" + IntHelper.toString((int)(f as CharacterFighter).Character.ActorId, 36) + ";");
                    str.Append(f.Name + ";");
                    str.Append(f.Skin + ";");
                    str.Append(f.Level + ";");
                    str.Append(IntHelper.toString((f as CharacterFighter).Character.Color1, 36)).Append(";");
                    str.Append(IntHelper.toString((f as CharacterFighter).Character.Color2, 36)).Append(";");
                    str.Append(IntHelper.toString((f as CharacterFighter).Character.Color3, 36)).Append(";");
                    if (fight.Team2.GetFighters().Count > 7)
                    {
                        str.Append("1;");
                    }
                    else
                    {
                        str.Append("0;");
                    }
                }
                stra.Append(str.ToString().Substring(1));
                (fight as PrismFight).DefenderList = (stra.ToString()); //Todo other form to serialize
            }

            return str.ToString();
        }


        public short Name
        {
            get
            {
                switch ((AlignmentTypeEnum)Alignement)
                {
                    case AlignmentTypeEnum.ALIGNMENT_BONTARIAN:
                        return 1111;
                    default:
                        return 1112;
                }
            }
        }

        public short Look
        {
            get
            {
                switch ((AlignmentTypeEnum)Alignement)
                {
                    case AlignmentTypeEnum.ALIGNMENT_BONTARIAN:
                        return 8101;
                    default:
                        return 8100;
                }
            }
        }

        public void SerializeAsGameMapInformations(StringBuilder SerializedString)
        {
            if (this.mySerializedPattern == null)
            {
                this.mySerializedPattern = new StringBuilder();
                this.mySerializedPattern.Append(this.CellId).Append(';');
                this.mySerializedPattern.Append(this.Orientation).Append(';');
                this.mySerializedPattern.Append('0').Append(';'); // Unknow
                this.mySerializedPattern.Append(this.ActorId).Append(';');
                this.mySerializedPattern.Append(Name).Append(";-10;");
                this.mySerializedPattern.Append(Look).Append("^100;");
                this.mySerializedPattern.Append(this.Level).Append(';');
                this.mySerializedPattern.Append(this.Level).Append(';');
                this.mySerializedPattern.Append(this.Alignement);
            }

            SerializedString.Append(this.mySerializedPattern.ToString());
        }
    }
}
