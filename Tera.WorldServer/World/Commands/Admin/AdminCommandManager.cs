using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Utils;

namespace Tera.WorldServer.World.Commands
{
    public class AdminCommandManager
    {
        private static Dictionary<string, AdminCommand> m_commands = new Dictionary<string, AdminCommand>();

        public static void Initialize()
        {
            var types = TypesManager.GetTypes(typeof(AdminCommand));
            foreach (var command in types)
            {
                try
                {
                    AdminCommand instance = (AdminCommand)Activator.CreateInstance(command);
                    if (instance.NeedLoaded)
                    {
                        m_commands.Add(instance.Prefix, instance);
                    }
                }
                catch { }
            }
            Logger.Info("@AdminCommands@ initialized");
        }

        public static AdminCommand GetCommand(string prefix)
        {
            lock (m_commands)
            {
                if (m_commands.ContainsKey(prefix))
                {
                    return m_commands[prefix];
                }
                else
                {
                    return null;
                }
            }
        }

        public static List<AdminCommand> Commands
        {
            get
            {
                return m_commands.Values.ToList();
            }
        }

        public static void Add(string key, AdminCommand command)
        {
            lock (m_commands)
            {
                if (!m_commands.ContainsKey(key))
                {
                    m_commands.Add(key, command);
                }
            }
        }
    }
}
