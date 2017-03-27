using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using Tera.Libs;
using Tera.WorldServer.Utils;
using Tera.Libs.Network;
using Tera.Libs.Enumerations;
using Tera.Libs.Packets;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Controllers;
using Tera.WorldServer.Database.Tables;
using System.Timers;

namespace Tera.WorldServer.Network
{
    public class InterClient
    {
        public static System.Timers.Timer timer { get; set; }
        public static SilverSocket socket { get; set; }
        public static object Locker = new object();

        public static void Initialize()
        {
            socket = new SilverSocket();
            socket.OnConnected += new SilverEvents.Connected(OnConnected);
            socket.OnDataArrivalEvent += new SilverEvents.DataArrival(OnDataArrivalEvent);
            socket.OnFailedToConnect += new SilverEvents.FailedToConnect(OnFailedToConnect);
            socket.OnSocketClosedEvent += new SilverEvents.SocketClosed(OnSocketClosedEvent);
            timer = new System.Timers.Timer(5000);
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(RetryConnect);
            timer.Start();
        }

        private static void OnFailedToConnect(Exception ex)
        {
            Logger.Debug("Failed to connect to RealmServer ");
        }

        private static void OnSocketClosedEvent()
        {
            Logger.Error("Connection lost with the RealmServer");
            timer.Enabled = true;
            timer.Start();
        }

        private static Timer SenderTimer;

        private static void OnConnected()
        {
            timer.Stop();
            timer.Enabled = false;
            timer.Close();
            Logger.Info("Synchronized with the RealmServer");
            SenderTimer = new Timer();
            SenderTimer.Interval = 1500;
            SenderTimer.Elapsed += new ElapsedEventHandler(timer_Tick);
            SenderTimer.Start();

        }

        static void timer_Tick(object sender, EventArgs e)
        {
            Send(new HelloKeyMessagePacket(Settings.AppSettings.GetStringElement("World.Key")));
            SenderTimer.Stop();
        }

        private static void RetryConnect(object sender = null, System.Timers.ElapsedEventArgs e = null)
        {
            socket.ConnectTo(Settings.AppSettings.GetStringElement("Inter.Host"), Settings.AppSettings.GetIntElement("Inter.Port"));
        }

        public static void Send(TeraPacket packet)
        {
            try
            {
                Logger.Debug("Send packet '" + packet.ID.ToString() + "' to RealmServer");
                socket.Send(packet.GetBytes);

            }
            catch (Exception e)
            {
                Logger.Error("Can't send packet to RealmServer : " + e.ToString());
            }
        }

        private static void OnDataArrivalEvent(byte[] data)
        {
            lock (Locker)
            {
                try
                {
                    var packet = new TeraPacket(data);
                    Logger.Debug("Received packet '" + packet.ID.ToString() + "' from RealmServer");
                    switch (packet.ID)
                    {
                        case PacketHeaderEnum.PlayerCommingMessage:
                            HandlePlayerComming(packet);
                            break;
                        case PacketHeaderEnum.KickPlayerMessage:
                            String accountName = packet.Reader.ReadString();
                            WorldClient wClient = WorldServer.Clients.Find(x => x.Account != null && x.Account.Username == accountName);
                            if (wClient != null)
                            {
                                wClient.Disconnect();
                                wClient.OnClose();
                            }
                            else
                                AccountTable.UpdateLogged(accountName, false);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("Can't read packet from server : " + e.ToString());
                }
            }
        }

        private static void HandlePlayerComming(TeraPacket packet)
        {
            string ticket = packet.Reader.ReadString();
            Logger.Debug("New Ticket added whith key " + ticket);
            var account = new AccountModel()
            {
                ID = packet.Reader.ReadInt32(),
                Username = packet.Reader.ReadString(),
                Password = packet.Reader.ReadString(),
                Pseudo = packet.Reader.ReadString(),
                Question = packet.Reader.ReadString(),
                Reponse = packet.Reader.ReadString(),
                Level = packet.Reader.ReadInt32(),
                LastIP = packet.Reader.ReadString(),
                LastConnectionDate = new DateTime(packet.Reader.ReadInt64()),
            };
            TicketController.RegisterTicket(new AccountTicket() { Ticket = ticket, Account = account, ExpireTime = 0 });
        }
    }
}
