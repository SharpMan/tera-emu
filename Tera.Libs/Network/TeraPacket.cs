using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tera.Libs.Enumerations;

namespace Tera.Libs.Network
{
    public class TeraPacket
    {
        public PacketHeaderEnum ID;
        public BinaryWriter Writer;
        public BinaryReader Reader;
        public MemoryStream Stream;

        public TeraPacket(PacketHeaderEnum id)
        {
            try
            {
                ID = id;
                Stream = new MemoryStream();
                Writer = new BinaryWriter(Stream);
                Writer.Write((byte)id);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error] Can't write OverPacket : " + e.ToString());
            }
        }

        public TeraPacket(byte[] data)
        {
            try
            {
                Stream = new MemoryStream(data);
                Reader = new BinaryReader(Stream);
                ID = (PacketHeaderEnum)Reader.ReadByte();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error] Can't read OverPacket : " + e.ToString());
            }
        }

        public byte[] GetBytes
        {
            get
            {
                return Stream.ToArray();
            }
        }
    }
}
