using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Effects
{
    /// <summary>
    /// Classe de gestion des effets de teleportation
    /// </summary>
    public sealed class EffectTeleport : EffectBase
    {
        /// <summary>
        /// Application de l'effet
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <returns></returns>
        public override int ApplyEffect(EffectCast CastInfos)
        {
            var Caster = CastInfos.Caster;
            var Cell = Caster.Fight.GetCell(CastInfos.CellId);

            if (Cell != null)
            {
                Caster.Fight.SendToFight(new GameActionMessage((int)GameActionTypeEnum.MAP_TELEPORT, Caster.ActorId, Caster.ActorId + "," + CastInfos.CellId));

                return Caster.SetCell(Cell);
            }

            return -1;
        }
    }
}
