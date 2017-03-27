using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ItemSetAppearMessage : PacketBase
    {
        public Player Character;
        public int Pano;

        public ItemSetAppearMessage(Player p, int pa)
        {
            this.Character = p;
            this.Pano = pa;
        }


        public override string Compile()
        {
            StringBuilder Packet = new StringBuilder("OS");
            int num = Character.InventoryCache.CountItemByItemSet(Pano);
            if (num <= 0) Packet.Append("-").Append(Pano);
            else
            {
                Packet.Append("+").Append(Pano).Append("|");
                var ItemSet = ItemSetTable.getItemSet(Pano);
                if (ItemSet != null)
                {
                    StringBuilder Items = new StringBuilder();
                    foreach (var Item in ItemSet.Items)
                    {
                        if (Character.InventoryCache.HasTemplateEquiped(Item.ID))
                        {
                            if (Items.Length > 0) Items.Append(";");
                            Items.Append(Item.ID);
                        }
                    }
                    var stat = ItemSet.getBonusStatByItemCount(num).ToItemStats();
                    if (stat.StartsWith(","))
                        stat = stat.Substring(1);
                    Packet.Append(Items.ToString()).Append("|").Append(stat);
                }
            }
            return Packet.ToString();
        }

    }
}
