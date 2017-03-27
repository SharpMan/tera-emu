using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Utils;

namespace Tera.WorldServer.World.Commands.Player
{
    public static class PlayerCommandManager
    {
        private static Dictionary<string, PlayerCommand> m_commands = new Dictionary<string, PlayerCommand>();

        public static void Initialize()
        {
            var types = TypesManager.GetTypes(typeof(PlayerCommand));
            foreach (var command in types)
            {
                try
                {
                    PlayerCommand instance = (PlayerCommand)Activator.CreateInstance(command);
                    if (instance.NeedLoaded)
                    {
                        m_commands.Add(instance.Prefix, instance);
                    }
                }
                catch { }
            }
            Logger.Info("@PlayerCommands@ initialized");
        }

        public static PlayerCommand GetCommand(string prefix)
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

        public static List<PlayerCommand> Commands
        {
            get
            {
                return m_commands.Values.ToList();
            }
        }

        public static void Add(string key, PlayerCommand command)
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
