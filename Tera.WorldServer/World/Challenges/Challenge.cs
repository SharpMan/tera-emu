using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Spells;

namespace Tera.WorldServer.World.Challenges
{
    public abstract class Challenge : FightListener
    {
        public int Id {get;set;}
        public Fight Fight {get;set;}
        public bool State = true;

        public bool Signaled = false;

        public bool ShowTarget  {get;set;}
        public long TargetId  {get;set;}
        public int BasicXpBonus  {get;set;}
        public int TeamXpBonus  {get;set;}
        public int BasicDropBonus  {get;set;}
        public int TeamDropBonus { get; set; }

        public Challenge(int Id, Fight Fight)
        {
            this.Fight = Fight;
            this.Id = Id;
        }

        public virtual bool CanSet()
        {
            return true;
        }

        public virtual void Show(WorldClient Client)
        {
            Client.Send(new FightShowChallenge(this));
        }

        public void Success()
        {
            if (!Signaled)
            {
                State = true;
                Signaled = true;
                Fight.SendToFight(new FightChallengeOk(this));
            }
        }

        public void Failure(Fighter looser = null)
        {
            if (State)
            {
                State = false;
                Fight.SendToFight(new FightChallengeFail(this));
                if (looser != null)
                {
                    Fight.SendToFight(new TextInformationMessage(Libs.Enumerations.TextInformationTypeEnum.INFO, 188, looser.Name));
                }
            }
        }

        public void ShowCell(int CellID)
        {
            Fight.SendToFight(new FightShowCell(TargetId>0?(TargetId):(-1), CellID));
        }
        
        public virtual void onEndFight(FightTeam Winners, FightTeam Loosers)
        {
            if (State != false && Winners == Fight.Team1)
            {
                Success();
            }
        }

        public virtual void onLeaveFight(Fighter Fighter) { }
        public virtual void onStartFight() { }
        public virtual void onLaunchWeapon(Fighter Launcher, InventoryItemModel Weapon, int TargetCellId, Fighter TargetFighter, Dictionary<Character.WeaponEffect, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec, bool isPunch) { }
        public virtual void onLaunchSpell(Fighter Launcher, SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<EffectInfos, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec) { }
        public virtual void onActorMoved(Fighter Fighter, Maps.MovementPath Path, FightCell NewCell) { }
        public virtual void onBeginTurn(Fighter newFighter) { }
        public virtual void onMiddleTurn(Fighter fighter) { }
        public virtual void onEndTurn(Fighter endFighter, bool Finish = false) { }
        public virtual void onDie(Fighter fighterDead, Fighter Caster) { }

        public virtual void onApplyGroundLayer(Fights.FightObjects.FightGroundLayer layer, Fighter Caster, SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<EffectInfos, List<Fighter>> TargetEffects) { }
    }
}
