using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using Tera.WorldServer.Utils;
using Tera.Libs;
using Tera.WorldServer.World.Chats;
using Tera.WorldServer.World.Kolizeum;

namespace Tera.WorldServer.Network
{
    public class WorldServer
    {
        public static List<WorldClient> Clients = new List<WorldClient>();
        private static SilverServer Server;
        private static ChatController myChatService = new ChatController();
        public static KolizeumManager Kolizeum;


        public static void Initialize()
        {
            Server = new SilverServer(Settings.AppSettings.GetStringElement("World.Host"), Settings.AppSettings.GetIntElement("World.Port"));
            Server.OnListeningFailedEvent += new SilverEvents.ListeningFailed(ListeningFailed);
            Server.OnListeningEvent += new SilverEvents.Listening(Listening);
            Server.OnAcceptSocketEvent += new SilverEvents.AcceptSocket(AcceptSocket);
            Server.WaitConnection();
            Kolizeum = new KolizeumManager();
            Kolizeum.Start();
        }

        private static void AcceptSocket(SilverSocket socket)
        {
            lock (Clients)
            {
                try
                {
                    Clients.Add(new WorldClient(socket));
                }
                catch (Exception e)
                {
                    Logger.Error("@Can't accept@ client : " + e.ToString());
                }
            }
        }

        private static void Listening()
        {
            Logger.Info(string.Format("WorldServer Started on port {0}", Settings.AppSettings.GetIntElement("World.Port")));
            myChatService.GenerateChannels();
        }

        private static void ListeningFailed(Exception ex)
        {
            Logger.Error(string.Format("Can't start the server... [{0}]", ex.ToString()));
        }

        public static ChatController GetChatController()
        {
            return myChatService;
        }
    }
}
