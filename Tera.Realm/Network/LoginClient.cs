using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;
using Tera.Realm.Utils;
using Tera.Realm.Managers;
using Tera.Realm.Database.Models;
using Tera.Realm.Database.Tables;
using Tera.Libs.Packets;
using Tera.Realm.Network.Packets;
using Tera.Libs.Network;
using System.Text.RegularExpressions;

namespace Tera.Realm.Network
{
    public class LoginClient : BaseClient
    {
        public string EncryptKey { get; set; }
        public RealmState State = RealmState.VERSION;
        public AccountModel Account { get; set; }
        private IPrintWriterEncrypted Out;

        public LoginClient(SilverSocket socket) : base(socket)
        {
            this.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?><cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" secure=\"false\" /><site-control permitted-cross-domain-policies=\"master-only\" /></cross-domain-policy>");
            this.Out = new IPrintWriterEncrypted(this);
            this.EncryptKey = StringHelper.RandomString(32);
            this.Out.GenAndSendKey();
            this.Send(LoginMessageFormatter.helloWorld(this.EncryptKey));
        }

        public override void OnClose()
        {
            Logger.Debug("Client disconnected !");
            this.Account = null;
            this.EncryptKey = null;
            this.Out = null;
            lock (LoginServer.Clients)
            {
                LoginServer.Clients.Remove(this);
            }
        }

        public bool HaveFailed = false;

        public override void OnPacket(String message)
        {
            if (message.Equals("1.29.1") || message.Equals("JK50"))
            {
                this.Out.printWithoutEncrypt("AlEv" + Definitions.DofusVersionRequired);
                return;
            }
            if (this.Out.getDecryptor() != null)
            {
                try
                {
                    message = Out.getDecryptor().decrypt(message);
                }
                catch (Exception e)
                {
                    Logger.Error("Fail to Parse -> " + e.ToString());
                    return;
                }
            }
            Logger.Debug("Received packet from client : " + message);
            try
            {
                switch (this.State)
                {
                    case RealmState.VERSION:
                        HandleVersion(message);
                        break;

                    case RealmState.ACCOUNT:
                        HandleAccount(message);
                        break;

                    case RealmState.SERVER_LIST:
                        HandleServerList(message);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Can't handle packet : " + e.ToString());
            }
        }

        public override void Send(String message)
        {
            Logger.Debug("Send packet : " + message);
            this.Out.print(message);
        }

        public void Write(String message)
        {
            base.Send(message);
        }


        public void HandleVersion(string version)
        {
            if (version == Definitions.DofusVersionRequired)
            {
                this.State = RealmState.ACCOUNT;
            }
            else
            {
                if (Out.getDecryptor() == null)
                {
                    Logger.Error("Client  has incorrect version of dofus @'" + version + "'@", false);
                    this.Send("AlEv" + Definitions.DofusVersionRequired);
                }
            }
        }

        private void HandleAccount(string packet)
        {
            string[] data = Regex.Split(packet,"\n");
            if (data.Length < 2)
            {
                this.Send(LoginMessageFormatter.WrongPassword());
                return;
            }
            string username = data[0];
            string password = data[1].Substring(2);
            var account = AccountTable.FindFirst(username);
            if (account != null)
            {
                if (Hash.cryptPass(this.EncryptKey, account.Password) == password)
                {
                    if (!GameServerManager.hasOpenedServers())
                    {
                        this.Send(LoginMessageFormatter.MessageBoxMaintenance());
                        this.Send(LoginMessageFormatter.ServerHasntDisponnible());
                        //base.Disconnect();
                        return;
                    }
                    if (AccountManager.isConnectedToRealm(username))
                    {
                        AccountManager.KickWithAccont(username);
                        this.Send(LoginMessageFormatter.isConnectedToRealm());
                        return;
                    }
                    if (account.isConnected())
                    {
                        this.Send(LoginMessageFormatter.isConnectedToRealm());
                        InterServer.Clients.ForEach(x => x.Send(new KickPlayerMessage(account.Username)));
                        return;
                    }
                    this.Account = account;
                    this.Send(LoginMessageFormatter.nicknameInformationMessage(this.Account.Pseudo));
                    this.Send(LoginMessageFormatter.communityInformationMessage(0));
                    this.Send(LoginMessageFormatter.serversInformationsMessage(true,this.Account.Level));
                    this.Send(LoginMessageFormatter.identificationSuccessMessage(this.Account.Level > 0));
                    this.Send(LoginMessageFormatter.accountQuestionInformationMessage(this.Account.SecretQuestion));
                    this.State = RealmState.SERVER_LIST;

                }
                else
                {
                    this.Send(LoginMessageFormatter.WrongPassword());
                    Logger.Debug("Wrong password for account '" + username + "'");
                }
            }
            else
            {
                this.Send(LoginMessageFormatter.WrongPassword());
                Logger.Debug("Can't found account '" + username + "'");
            }
        }

        public void HandleServerList(string packet)
        {
            if (packet.Substring(0, 2) == "Af")
            {
                this.Send("Af-1|");
            }
            else if (packet.Substring(0, 2) == "Ax")
            {
                this.Send(LoginMessageFormatter.charactersListMessage(Definitions.SubscriptionTime,this.Account.Characters));
            }
            else if (packet.Substring(0, 2) == "AX")
            {
                int serverID = int.Parse(packet.Substring(2));
                var GameServer = GameServerTable.Cache.Find(x => x.ID == serverID);
                if (GameServer == null)
                {
                    this.Disconnect();
                    return;
                }
                else if (GameServer.State == ServerStateEnum.InMaintenance || GameServer.State == ServerStateEnum.InSave)
                {
                    this.Send(LoginMessageFormatter.ServerInSave());
                    return;
                }
                else if (GameServer.LevelRequired > this.Account.Level)
                {
                    this.Send(LoginMessageFormatter.InaccessedServer());
                    return;
                }
                InterServer.Clients.Find(x => x.Server != null && x.Server.ID.Equals(serverID)).Send(new PlayerCommingMessage(Account, EncryptKey));
                this.Send(LoginMessageFormatter.selectedHostInformationMessage(GameServer.Adress,GameServer.Port,this.EncryptKey,true));
            }
        }
    }
}
