using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class SpellHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'B':
                    SpellHandler.ProcessSpellBoostRequest(Client, Packet);
                    break;

                case 'M':
                    SpellHandler.ProcessSpellMoveRequest(Client, Packet);
                    break;
            }
        }

        public static void ProcessSpellBoostRequest(WorldClient Client, string Packet)
        {
            // fake Packet
            if (Packet.Length < 3)
            {
                Client.Send(new SpellUpdateFailMessage());
                return;
            }

            int SpellId;

            // Packet Packet
            if (!int.TryParse(Packet.Substring(2), out SpellId))
            {
                Client.Send(new SpellUpdateFailMessage());
                return;
            }

            // persos
            var Character = Client.GetCharacter();
            var Book = Character.GetSpellBook();

            // n'a pas le spell
            if (!Book.HasSpell(SpellId))
            {
                for (int i = 1; i < Client.GetCharacter().Level; i++)
                {
                    Book.GenerateLevelUpSpell((ClassEnum)Client.GetCharacter().Classe, i);
                }
                Client.Send(new SpellUpdateFailMessage());
                Character.Send(new SpellsListMessage(Character));
                return;
            }

            var Spell = Book.GetSpellLevel(SpellId);

            // ps assez de points
            if (Character.SpellPoint < Spell.Level)
            {
                Client.Send(new SpellUpdateFailMessage());
                return;
            }

            lock (Client.BoostSpellSync)
            {
                Book.LevelUpSepll(SpellId);
                Character.SpellPoint -= Spell.Level;
            }

            using (CachedBuffer Buffer = new CachedBuffer(Client))
            {
                Buffer.Append(new SpellUpdateSuccessMessage(SpellId, Spell.Level + 1));
                Buffer.Append(new AccountStatsMessage(Character));
            }
        }

        public static void ProcessSpellMoveRequest(WorldClient Client, string Packet)
        {
            // fake Packet
            if (!Packet.Contains('|'))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Data = Packet.Substring(2).Split('|');

            // fake Packet
            if (Data.Length != 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            int SpellId;
            int Position;

            // fake
            if (!int.TryParse(Data[0], out SpellId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!int.TryParse(Data[1], out Position))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Book = Client.GetCharacter().GetSpellBook();

            // n'a pas le sort
            if (!Book.HasSpell(SpellId))
            {
                for (int i = 1; i < Client.GetCharacter().Level; i++)
                {
                    Book.GenerateLevelUpSpell((ClassEnum)Client.GetCharacter().Classe, i);
                }
                Client.Send(new BasicNoOperationMessage());
                Client.Send(new SpellsListMessage(Client.Character));
                return;
            }

            Book.MoveSpell(SpellId, Position);

            Client.Send(new BasicNoOperationMessage());
        }
    }
}
