using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.Network
{
    public class IPrintWriterEncrypted
    {
        private NeoString Decryptor;
        private WorldClient Client;

        public IPrintWriterEncrypted(WorldClient Client)
        {
            this.Client = Client;
        }

        public void print(String s)
        {
            if (s == null)
            {
                s = "null";
            }
            write(parseToSend(s));
        }

        public void printWithoutEncrypt(String s)
        {
            if (s == null)
            {
                s = "null";
            }
            write(s + (char)0x00);
        }

        public void println(String s)
        {
            if (s == null)
            {
                s = "null";
            }
            if (Decryptor != null)
            {
                s = TeraCrypt.toUtf(Decryptor.encrypt(s));
            }
            //System.out.println("SERV >> CLIENT:{" + s + "};");
            write(s + (char)0x00);
        }

        public void GenAndSendKey()
        {
            String public_key = TeraCrypt.GenPublicKey();//CryptManager.GenPublicKey();
            String decrypt_key = TeraCrypt.DecryptKeyOfPublicKey(public_key);
            Decryptor = new NeoString(decrypt_key);
            write("JK" + public_key);
            //flush();
        }

        private String parseToSend(String toSend)
        {
            if (toSend != null && Decryptor != null)
            {
                //if (toSend.Contains(((char)0x00).ToString()))
                //{
                    String[] packets = toSend.Split(((char)0x00));
                    StringBuilder bToSend = new StringBuilder();
                    foreach (String packett in packets)
                    {
                        var packet = Decryptor.encrypt(packett);
                        //System.out.println("SERV >> CLIENT:{" + Packet + "};");
                        bToSend.Append((packet));
                        bToSend.Append((char)0x00);
                    }
                    return bToSend.ToString();
                //}
            } 
            return toSend;
        }

        public NeoString getDecryptor()
        {
            return Decryptor;
        }

        public void close()
        {
            this.Decryptor = null;
        }

        public void write(String message)
        {
            Client.Write(message);
        }


    }
}
