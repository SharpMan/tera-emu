using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.JS;
using Tera.WorldServer.Database.Models;
using System.Runtime.CompilerServices;
using Tera.WorldServer.Network;
using Jint.Native;
using Tera.WorldServer.World.Packets;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Scripting.Commands
{
    public class JSAdminCommand : JSClass
    {
        public JSAdminCommand(string Namespace, string FilePath, string Name)
            : base(Namespace, FilePath, Name)
        {
            this.Load();
            this.Compile();
            this.__construct(this);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Reload()
        {
            this.Load();
            this.Compile();
            this.__construct();
        }

        private int AccessLevel = 0;
        private List<string> authorizedSubCommands = new List<string>();

        [JSFunction]
        public void setAccessLevel(double level)
        {
            this.AccessLevel = (int)level;
        }

        [JSFunction]
        public void setAuthorizedSubCommands(params string[] SubcommandsNames)
        {
            authorizedSubCommands.Clear();
            if (SubcommandsNames != null)
            {
                foreach (string command in SubcommandsNames)
                {
                    authorizedSubCommands.Add(command.ToLower());
                }
            }
        }


        /// <summary>
        /// Call Action by name, True : function found / False : error found / function not found
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns>True : function found / False : error found / function not found</returns>
        ///
        public bool callAction(WorldClient Client, CommandParameters Params)
        {
            if (Client.Account.Level < AccessLevel)
            {
                return true;
            }

            if (authorizedSubCommands.Count == 0)
            {
                return main(Client, Params);
            }
            else if (Params.Lenght > 1)
            {
                string actionName = Params[1].ToLower();
                Params.ChangeParametersAfter(1);
                if (authorizedSubCommands.Contains(actionName))
                {
                    try
                    {
                        this.Invoke(actionName, Client, Params);
                        return true;
                    }
                    catch (JsException e)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return main(Client);
            }
        }

        private bool main(WorldClient Client, CommandParameters Params)
        {
            try
            {
                this.Invoke("main", Client, Params);
                return true;
            }
            catch (JsException e)
            {
                return false;
            }
        }

        private bool main(WorldClient Client)
        {
            try
            {
                this.Invoke("main", Client);
                return true;
            }
            catch (JsException e)
            {
                return false;
            }
        }

        [JSFunction]
        public void SendErrorMessage(WorldClient Client, String message)
        {
            Client.Send(new ConsoleMessage(message, ConsoleColorEnum.RED));
        }

        [JSFunction]
        public void SendSuccessMessage(WorldClient Client, String message)
        {
            Client.Send(new ConsoleMessage(message, ConsoleColorEnum.GREEN));
        }

        [JSFunction]
        public void SendInformationMessage(WorldClient Client, String message)
        {
            Client.Send(new ConsoleMessage(message, ConsoleColorEnum.WHITE));
        }
    }
}