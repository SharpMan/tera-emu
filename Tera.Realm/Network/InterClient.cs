using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using Tera.Realm.Database.Models;
using Tera.Libs;
using Tera.Libs.Network;
using Tera.Libs.Enumerations;
using Tera.Realm.Database.Tables;

namespace Tera.Realm.Network
{
    public class InterClient
    {
        private SilverSocket m_socket;
        public object Locker = new object();
        public GameServerModel Server { get; set; }

        public InterClient(SilverSocket sock)
        {
            m_socket = sock;
            m_socket.OnDataArrivalEvent += m_socket_OnDataArrivalEvent;
            m_socket.OnSocketClosedEvent += m_socket_OnSocketClosedEvent;
        }

        public String getIP()
        {
            return m_socket.IP.Split(':')[0];
        }


        private void m_socket_OnDataArrivalEvent(byte[] data)
        {
            lock (Locker)
            {
                var packet = new TeraPacket(data);
                if(Server != null)
                    Logger.Info("Received packet '" + packet.ID.ToString() + "' from server '" + this.Server.ID);
                else
                    Logger.Info("Received packet@'" + packet.ID.ToString() + "' from unknown server '");
                switch (packet.ID)
                {
                    case PacketHeaderEnum.HelloKeyMessage:
                        this.HandleHelloKey(packet);
                        if (Server == null)
                            Kick();
                        else
                            Server.State = ServerStateEnum.Online;
                        break;
                }
            }
        }

        private void HandleHelloKey(TeraPacket packet)
        {
            string key = packet.Reader.ReadString();
            GameServerModel gsm = GameServerTable.Cache.Find(x => x.Key.Equals(key));
            if (gsm != null)
            {
                this.Server = gsm;
                Logger.Info("GameServer " + gsm.ID + " Online");
            }
            else
            {
                Logger.Warn("Unknown key '"+key+"' received");
            }
        }

        void m_socket_OnSocketClosedEvent()
        {
            lock (InterServer.Clients)
               InterServer.Clients.Remove(this);
            if(Server != null)
               Logger.Info(string.Format("GameServer "+Server.ID+" disconnected by server"));
            else
                Logger.Info(string.Format("Unknown InterClient disconnected by server"));

        }

        public void Kick()
        {
            try
            {
                this.m_socket.CloseSocket();
            }
            catch (Exception e)
            {
                Logger.Error("Can't disconnect InterClient : " + e.ToString());
            }
        }

        public void Send(TeraPacket packet)
        {
            try
            {
                if (this.Server != null && this.Server.State != ServerStateEnum.Offline)
                {
                    Logger.Debug("Send packet '" + packet.ID.ToString() + "' to server '" + Server.ID + "'");
                    this.m_socket.Send(packet.GetBytes);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Can't send packet to world server "+this.Server.ID+" : " + e.ToString());
            }
        }
    }
}
