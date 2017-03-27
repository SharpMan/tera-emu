using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using Tera.Libs;
using Tera.Realm.Utils;

namespace Tera.Realm.Network
{
    public static class LoginServer
    {
        public static SilverServer Server { get; set; }
        public static List<LoginClient> Clients = new List<LoginClient>();

        public static void Initialize()
        {
            Server = new SilverServer(Settings.AppSettings.GetStringElement("Login.Host"), Settings.AppSettings.GetIntElement("Login.Port"));
            Server.OnAcceptSocketEvent += new SilverEvents.AcceptSocket(Server_OnAcceptSocketEvent);
            Server.OnListeningFailedEvent += new SilverEvents.ListeningFailed(Server_OnListeningFailedEvent);
            Server.OnListeningEvent += new SilverEvents.Listening(Server_OnListeningEvent);
            Server.WaitConnection();
        }

        private static void Server_OnListeningEvent()
        {
            Logger.Info(string.Format("LoginServer Started on port {0}", Settings.AppSettings.GetIntElement("Login.Port")));
        }

        private static void Server_OnListeningFailedEvent(Exception ex)
        {
            Logger.Error("Can't start loginserver !");
            Logger.Error(ex);
        }

        private static void Server_OnAcceptSocketEvent(SilverSocket socket)
        {
            Logger.Debug("Input connection on loginserver <" + socket.IP + ">");
            lock (Clients)
            {
                Clients.Add(new LoginClient(socket));
            }
        }
    }
}
