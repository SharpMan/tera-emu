using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using Tera.Realm.Utils;
using Tera.Libs;
using Tera.Libs.Network;

namespace Tera.Realm.Network
{
    public class BaseClient
    {
        private SilverSocket _socket { get; set; }

        public BaseClient(SilverSocket socket)
        {
            this._socket = socket;
            this._socket.OnDataArrivalEvent += new SilverEvents.DataArrival(OnData);
            this._socket.OnSocketClosedEvent += new SilverEvents.SocketClosed(OnClose);
        }

        public void DisconnectLater(int time)
        {
            var delayer = new Delayer<BaseClient>(GetType().GetMethod("Disconnect"), null, this, time);
            delayer.Start();
        }

        public void Disconnect()
        {
            try
            {
                this._socket.CloseSocket();
            }
            catch (Exception e)
            {
                Logger.Error("Can't disconnect account : " + e.ToString());
            }
        }

        public virtual void OnClose() { }

        private void OnData(byte[] data)
        {
            try
            {
                string noParsedPacket = Encoding.ASCII.GetString(data);
                try
                {
                    if (noParsedPacket.Length > 150)
                    {
                        this._socket.CloseSocket();
                        Logger.Error("Client kicked because: Packet flood");
                    }

                }
                catch (Exception e)
                {
                    Logger.Error("Can't kick packet : " + e.ToString());
                }
                foreach (string packet in noParsedPacket.Replace("\x0a", "").Split('\x00'))
                {
                    try
                    {
                        if (packet == "")
                            continue;
                        OnPacket(packet);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Can't parse packet : " + e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public virtual void OnPacket(String message) { }

        public virtual void Send(String packet)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(packet + "\x00");
                this._socket.Send(data);
            }
            catch (Exception e)
            {
                Logger.Error("Can't send packet : " + packet + "\n" + e.ToString());
            }
        }

        public String getIP()
        {
            return this._socket.IP.Split(':')[0];
        }
    }
}
