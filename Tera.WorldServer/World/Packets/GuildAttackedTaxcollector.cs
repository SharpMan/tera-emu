using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public enum AttackedTaxcollectorState
    {
        ATTACKED = 'A',
        SURVIVED = 'S',
        DIED = 'D'
    }
    public class GuildAttackedTaxcollector : PacketBase
    {
        private AttackedTaxcollectorState state;
        private TaxCollector percepteur;

        public GuildAttackedTaxcollector(AttackedTaxcollectorState state, TaxCollector percepteur)
        {
            this.state = state;
            this.percepteur = percepteur;
        }

        public override string Compile()
        {
            return "gA" + (char)state + percepteur.N1 + "," + percepteur.N2 + "|" + percepteur.ActorId + "|" + percepteur.Map.X + "|" + percepteur.Map.Y;
        }
    }
}
