using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Types
{
    public sealed class KolizeumFight : Fight
    {
        public KolizeumFight(Map Map, WorldClient Attacker, WorldClient Defender)
            : base(FightType.TYPE_KOLIZEUM, Map)
        {
            // Init du combat
            var AttFighter = new CharacterFighter(this, Attacker);
            var DefFighter = new CharacterFighter(this, Defender);

            Attacker.AddGameAction(new GameFight(AttFighter, this));
            Defender.AddGameAction(new GameFight(DefFighter, this));

            base.InitFight(AttFighter, DefFighter);
        }

        public override int GetStartTimer()
        {
            return 30000;
        }

        public override int GetTurnTime()
        {
            return 30000;
        }

        private StringBuilder mySerializedString = null;
        public override void SerializeAs_FlagDisplayInformations(StringBuilder Packet)
        {
            if (this.mySerializedString == null)
            {
                this.mySerializedString = new StringBuilder();
                this.mySerializedString.Append(this.FightId).Append(';');//Infos Fight
                this.mySerializedString.Append(0).Append('|');

                this.mySerializedString.Append(this.Team1.LeaderId).Append(';');//Infos Team2
                this.mySerializedString.Append(this.Team1.Leader.MapCell).Append(';');
                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER).Append(';');
                this.mySerializedString.Append((this.Team1.Leader as CharacterFighter).Character.Alignement).Append('|');

                this.mySerializedString.Append(this.Team2.LeaderId).Append(';');//Infos Team2
                this.mySerializedString.Append(this.Team2.Leader.MapCell).Append(';');
                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER).Append(';');
                this.mySerializedString.Append((this.Team2.Leader as CharacterFighter).Character.Alignement);
            }

            Packet.Append(this.mySerializedString.ToString());
        }

        public override string SerializeAs_FightListInformations()
        {
            var Infos = new StringBuilder(this.FightId.ToString()).Append(';');//FightID
            Infos.Append(this.FightState == FightState.STATE_ACTIVE ? (this.FightTime) : (-1)).Append(';');//FightTime

            Infos.Append(TeamTypeEnum.TEAM_TYPE_PLAYER).Append(","); //Infos Team1
            Infos.Append((this.Team1.Leader as CharacterFighter).Character.Alignement).Append(",");
            Infos.Append(this.Team1.GetAliveFighters().Count).Append(';');

            Infos.Append(TeamTypeEnum.TEAM_TYPE_PLAYER).Append(","); //Infos Team2
            Infos.Append((this.Team2.Leader as CharacterFighter).Character.Alignement).Append(",");
            Infos.Append(this.Team2.GetAliveFighters().Count).Append(';');

            return Infos.ToString();
        }

        /// <summary>
        /// Kické ou alors tout simplement annulé
        /// </summary>
        /// <param name="Character"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void OverridableLeaveFight(Fighter Fighter)
        {
            var WinnersFighter = Fighter.Fight.GetEnnemyTeam(Fighter.Team).GetFighters().OfType<CharacterFighter>().ToList();
            var LoosersFighter = Fighter.Team.GetFighters().OfType<CharacterFighter>().ToList();
            var WinnersTotalCote = WinnersFighter.Sum(x => x.Character.Cote);
            var LoosersTotalCote = LoosersFighter.Sum(x => x.Character.Cote);

            int LooseCote = 0;

            if (LoosersTotalCote - WinnersTotalCote < 0)
                LooseCote = 25;
            else if (WinnersTotalCote - LoosersTotalCote < 0)
                LooseCote = 75;
            else
                LooseCote = 50;


            // Un persos quitte le combat
            switch (this.FightState)
            {
                case Fights.FightState.STATE_PLACE:

                    this.Map.SendToMap(new GameFightTeamFlagFightersMessage(new List<Fighter> { Fighter }, Fighter.Team.LeaderId, false));

                    this.SendToFight(new GameActorDestroyMessage(Fighter.ActorId));

                    Fighter.LeaveFight();

                    Fighter.Send(new GameLeaveMessage());

                    int CoteLoosed = LooseCote;

                    if ((Fighter as CharacterFighter).Character.Cote - LooseCote < 0)
                        CoteLoosed = (Fighter as CharacterFighter).Character.Cote;

                    (Fighter as CharacterFighter).Character.Cote -= CoteLoosed;

                    (Fighter as CharacterFighter).Character.Send(new ChatGameMessage("Suite à cette defaites vous avez perdu " + CoteLoosed + " points de quote", "FF0000"));


                    break;

                case FightState.STATE_ACTIVE:
                    if (Fighter.TryDie(Fighter.ActorId, true) != -3)
                    {
                        Fighter.LeaveFight();

                        Fighter.Send(new GameLeaveMessage());
                        CoteLoosed = LooseCote;

                        if ((Fighter as CharacterFighter).Character.Cote - LooseCote < 0)
                            CoteLoosed = (Fighter as CharacterFighter).Character.Cote;

                        (Fighter as CharacterFighter).Character.Cote -= CoteLoosed;

                        (Fighter as CharacterFighter).Character.Send(new ChatGameMessage("Suite à cette defaites vous avez perdu " + CoteLoosed + " points de quote", "FF0000"));
                    }
                    break;
            }
        }

        public override void OverridableEndFight(FightTeam Winners, FightTeam Loosers)
        {
            var WinnersFighter = Winners.GetFighters().OfType<CharacterFighter>().ToList();
            var LoosersFighter = Loosers.GetFighters().OfType<CharacterFighter>().ToList();

            var WinnersTotalCote = WinnersFighter.Sum(x => x.Character.Cote);
            var LoosersTotalCote = LoosersFighter.Sum(x => x.Character.Cote);

            int WinCote = 0, LooseCote = 0;

            if (LoosersTotalCote - WinnersTotalCote < 0)
                WinCote = 25;
            else if (WinnersTotalCote - LoosersTotalCote < 0)
                WinCote = 75;
            else
                WinCote = 50;

            if (LoosersTotalCote - WinnersTotalCote < 0)
                LooseCote = 25;
            else if (WinnersTotalCote - LoosersTotalCote < 0)
                LooseCote = 75;
            else
                LooseCote = 50;



            var PossibleItemLoot = new List<ItemLoot>();
            var ItemDroped = new Dictionary<int, int>()
            {
                { Settings.AppSettings.GetIntElement("Kolizeum.WinItem") , 3 },
            };

            foreach (var Fighter in LoosersFighter)
            {
                int CoteLoosed = LooseCote;

                if (!Fighter.Left)
                {
                    if (Fighter.Character.Cote - LooseCote < 0)
                        CoteLoosed = Fighter.Character.Cote;
                    Fighter.Character.Cote -= CoteLoosed;
                }

                this.myResult.AddResult(Fighter, false, WinExp: -CoteLoosed);
            }

            foreach (var Fighter in WinnersFighter)
            {
                if (!Fighter.Left)
                {
                    Fighter.Character.Cote += WinCote;
                    this.myResult.AddResult(Fighter, true, WinExp: WinCote, WinItems: ItemDroped);
                    InventoryItemTable.TryCreateItem(ItemDroped.First().Key, Fighter.Character, ItemDroped.First().Value);
                }
                else
                {
                    this.myResult.AddResult(Fighter, true);
                }
            }
            base.EndFight();
        }
    }
}
