using System;
using System.Collections.Generic;
using PluginFramework;
using PluginAPI.Core;
using PluginAPI.Events;
using EventManager = PluginFramework.EventManager;
using Logger = SCPSL_Framework.Utilities.Logger;

namespace CustomRoleManager
{
    public class RoleManager : IPlugin
    {
        public string Name => "Custom Role Manager";
        public string Version => "1.0.0";

        private static readonly Dictionary<string, string> PlayerRoles = new Dictionary<string, string>();

        public void OnLoad()
        {
            Logger.Log($"✅ [{Name}] Loaded successfully!");
            ServerConsole.AddLog($"✅ [{Name}] Loaded successfully!", ConsoleColor.Green);

            // ✅ تسجيل مستمع للأوامر الخاصة بـ "الرُتب المخصصة"
            EventManager.RegisterListener("server_command", new RoleCommandListener());
        }

        public static string GetPlayerRole(string playerName)
        {
            return PlayerRoles.ContainsKey(playerName) ? PlayerRoles[playerName] : "None";
        }

        public static void SetPlayerRole(string playerName, string roleName)
        {
            PlayerRoles[playerName] = roleName;
        }

        public static void RemovePlayerRole(string playerName)
        {
            if (PlayerRoles.ContainsKey(playerName))
            {
                PlayerRoles.Remove(playerName);
            }
        }
    }

    public class RoleCommandListener : IEventListener
    {
        public void OnEventTriggered(string eventName, object eventData)
        {
            if (eventName == "server_command" && eventData is string command)
            {
                string[] args = command.Split(' ');
                if (args.Length < 2) return;

                string action = args[0];
                string playerName = args[1];

                switch (action)
                {
                    case "role":
                        ShowPlayerRole(playerName);
                        break;

                    case "setrole":
                        if (args.Length < 3) return;
                        string roleName = args[2];
                        RoleManager.SetPlayerRole(playerName, roleName);
                        ServerConsole.AddLog($"✅ Set role '{roleName}' for player {playerName}!", ConsoleColor.Green);
                        break;

                    case "removerole":
                        RoleManager.RemovePlayerRole(playerName);
                        ServerConsole.AddLog($"✅ Removed role from player {playerName}!", ConsoleColor.Yellow);
                        break;

                    default:
                        ServerConsole.AddLog($"❌ Unknown command: {command}", ConsoleColor.Red);
                        break;
                }
            }
        }

        private void ShowPlayerRole(string playerName)
        {
            string role = RoleManager.GetPlayerRole(playerName);
            ServerConsole.AddLog($"ℹ️ {playerName} has role: {role}", ConsoleColor.Cyan);
        }
    }
}
