using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using Tera.Libs;
using Tera.Realm.Utils;

namespace Tera.Realm.Network
{
    public class InterServer
    {
        public static List<InterClient> Clients = new List<InterClient>();
        private static SilverServer Server;

        public static void Initialize()
        {
            Server = new SilverServer(Settings.AppSettings.GetStringElement("Inter.Host"), Settings.AppSettings.GetIntElement("Inter.Port"));
            Server.OnListeningFailedEvent += new SilverEvents.ListeningFailed(ListeningFailed);
            Server.OnListeningEvent += new SilverEvents.Listening(Listening);
            Server.OnAcceptSocketEvent += new SilverEvents.AcceptSocket(AcceptSocket);
            Server.WaitConnection();
        }

        private static void AcceptSocket(SilverSocket socket)
        {
            lock (Clients)
            {
                try
                {
                    Logger.Info(string.Format("New GameServer connected [{0}]", socket.IP));
                    Clients.Add(new InterClient(socket));
                }
                catch (Exception e)
                {
                    Logger.Error("@Can't accept@ client : " + e.ToString());
                }
            }
        }

        private static void Listening()
        {
            Logger.Info(string.Format("InterServer Started on port {0}", Settings.AppSettings.GetIntElement("Inter.Port")));
        }

        private static void ListeningFailed(Exception ex)
        {
            Logger.Error(string.Format("Can't start the server... [{0}]", ex.ToString()));
        }
    }
}
