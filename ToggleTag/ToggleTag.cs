using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smod2;
using Smod2.Attributes;
using Smod2.Commands;
using Smod2.EventHandlers;
using Smod2.Events;

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ToggleTag
{
    [PluginDetails(
        author = "Karl Essinger",
        name = "ToggleTag",
        description = "Persistant toggeling of role tags and overwatch.",
        id = "karlofduty.toggletag",
        version = "1.1.4",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
    )]
    public class ToggleTag : Plugin
    {
        public HashSet<string> tagsToggled;
        public HashSet<string> overwatchToggled;

        private readonly string defaultConfig =
        "{\n"                       +
        "    \"tags\": [],\n"       +
        "    \"overwatch\": []\n"   +
        "}";

        public override void OnDisable()
        {
            
        }

        public override void OnEnable()
        {
			new Task(async () =>
			{
				await Task.Delay(5000);
				RegisterDefaultPermission("toggletag.savetag");
                RegisterDefaultPermission("toggletag.saveoverwatch");
                if (!Directory.Exists(FileManager.GetAppFolder(GetConfigBool("toggletag_global")) + "ToggleTag"))
                {
                    Directory.CreateDirectory(FileManager.GetAppFolder(GetConfigBool("toggletag_global")) + "ToggleTag");
                }
            
                if (!File.Exists(FileManager.GetAppFolder(GetConfigBool("toggletag_global")) + "ToggleTag/data.json"))
                {
                    File.WriteAllText(FileManager.GetAppFolder(GetConfigBool("toggletag_global")) + "ToggleTag/data.json", defaultConfig);
                }
                JToken jsonObject = JToken.Parse(File.ReadAllText(FileManager.GetAppFolder(GetConfigBool("toggletag_global")) + "ToggleTag/data.json"));

                tagsToggled = new HashSet<string>(jsonObject.SelectToken("tags").Values<string>());
                overwatchToggled = new HashSet<string>(jsonObject.SelectToken("overwatch").Values<string>());
                this.Info("Toggletag enabled.");
			}).Start();
		}
        
        public override void Register()
        {
            this.AddEventHandlers(new PlayerJoinHandler(this), Priority.High);
            this.AddEventHandlers(new TagCommandHandler(this), Priority.High);
            this.AddCommand("console_hidetag", new HideTagCommand(this));
            this.AddCommand("console_showtag", new ShowTagCommand(this));
            this.AddConfig(new Smod2.Config.ConfigSetting("toggletag_global", false, Smod2.Config.SettingType.BOOL, true, "Whether or not to use the global config dir to save data, default is false"));
        }

        public void SaveTagsToFile()
        {
            // Save the state to file
            StringBuilder builder = new StringBuilder();
            builder.Append("{\n");
            builder.Append("    \"tags\":\n");
            builder.Append("    [\n");
            foreach (string line in tagsToggled)
            {
                builder.Append("        \"" + line + "\"," + "\n");
            }
            builder.Append("    ],\n");
            builder.Append("    \"overwatch\":\n");
            builder.Append("    [\n");
            foreach (string line in overwatchToggled)
            {
                builder.Append("        \"" + line + "\"," + "\n");
            }
            builder.Append("    ]\n}");
            File.WriteAllText(FileManager.GetAppFolder() + "ToggleTag/data.json", builder.ToString());
        }

        public static bool IsPossibleSteamID(string steamID)
        {
            return (steamID.Length == 17 && long.TryParse(steamID, out long n));
        }

        public Smod2.API.Player GetPlayer(string steamID)
        {
            foreach (Smod2.API.Player player in this.pluginManager.Server.GetPlayers())
            {
                if (player.SteamId == steamID)
                {
                    return player;
                }
            }
            return null;
        }
    }

    class HideTagCommand : ICommandHandler
    {
        private ToggleTag plugin;
        public HideTagCommand(ToggleTag plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "Hide the tag of a player, callable from the console.";
        }

        public string GetUsage()
        {
            return "console_hidetag <steamid>";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            // Check if SteamID is included
            if (args.Length > 0)
            {
                // Check if valid SteamID
                if (ToggleTag.IsPossibleSteamID(args[0]))
                {
                    // Check if already visible
                    if (plugin.tagsToggled.Add(args[0]))
                    {
                        // Gets the player name for the feedback and sets the playeer's tag status if they are online
                        string name = "";
                        Smod2.API.Player player = plugin.GetPlayer(args[0]);
                        if (player != null)
                        {
                            player.HideTag(true);
                            name = player.Name;
                        }
                        else
                        {
                            name = "offline player";
                        }

                        plugin.SaveTagsToFile();
                        return new string[] { "Tag hidden of " + name + "." };
                    }
                    else
                    {
                        // Still set the tag just in case it's status is not synced with the plugin's status for some reason
                        Smod2.API.Player player = plugin.GetPlayer(args[0]);
                        if (player != null)
                        {
                            player.HideTag(true);
                        }
                        return new string[] { "Tag was alreeady hidden." };
                    }
                }
                else
                {
                    return new string[] { "Invalid SteamID provided: '" + args[0] + "'." };
                }
            }
            return new string[] { "Not enough arguments provided. 'console_hidetag <steamid>'" };
        }
    }

    class ShowTagCommand : ICommandHandler
    {
        private ToggleTag plugin;
        public ShowTagCommand(ToggleTag plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "Show the tag of a player, callable from the console.";
        }

        public string GetUsage()
        {
            return "console_showtag <steamid>";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            // Check if SteamID is included
            if (args.Length > 0)
            {
                // Check if valid SteamID
                if (ToggleTag.IsPossibleSteamID(args[0]))
                {
                    // Check if already visible
                    if(plugin.tagsToggled.Remove(args[0]))
                    {
                        // Gets the player name for the feedback and sets the playeer's tag status if they are online
                        string name = "";
                        Smod2.API.Player player = plugin.GetPlayer(args[0]);
                        if(player != null)
                        {
                            player.HideTag(false);
                            name = player.Name;
                        }
                        else
                        {
                            name = "offline player";
                        }

                        plugin.SaveTagsToFile();
                        return new string[] { "Tag revealed of " + name + "."};
                    }
                    else
                    {
                        // Still set the tag just in case it's status is not synced with the plugin's status for some reason
                        Smod2.API.Player player = plugin.GetPlayer(args[0]);
                        if (player != null)
                        {
                            player.HideTag(false);
                        }
                        return new string[] { "Tag was alreeady revealed." };
                    }
                }
                else
                {
                    return new string[] { "Invalid SteamID provided: '" + args[0] + "'." };
                }
            }
            return new string[] { "Not enough arguments provided. 'console_showtag <steamid>'" };
        }
    }

    public class PlayerJoinHandler : IEventHandlerPlayerJoin
    {
        private readonly ToggleTag plugin;
        public PlayerJoinHandler(ToggleTag plugin)
        {
            this.plugin = plugin;
        }
        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if(plugin.tagsToggled.Contains(ev.Player.SteamId))
            {
                ev.Player.HideTag(true);
            }
            else
            {
                ev.Player.HideTag(false);
            }

            ev.Player.OverwatchMode = plugin.overwatchToggled.Contains(ev.Player.SteamId);
        }
    }

    public class TagCommandHandler : IEventHandlerAdminQuery
    {
        private readonly ToggleTag plugin;
        public TagCommandHandler(ToggleTag plugin)
        {
            this.plugin = plugin;
        }
        public void OnAdminQuery(AdminQueryEvent ev)
        {
            // Check if user or console command
            if (ev.Query == "REQUEST_DATA PLAYER_LIST SILENT" || ev.Admin == null || ev.Admin.SteamId == null)
            {
                return;
            }

            if (ev.Admin.HasPermission("toggletag.savetag"))
            {
                // Check normal version of command
                if (ev.Query == "hidetag")
                {
                    plugin.tagsToggled.Add(ev.Admin.SteamId);
                    plugin.SaveTagsToFile();
                    return;
                }
                else if (ev.Query == "showtag")
                {
                    plugin.tagsToggled.Remove(ev.Admin.SteamId);
                    plugin.SaveTagsToFile();
                    return;
                }
            }

            if(ev.Admin.HasPermission("toggletag.saveoverwatch"))
            {
                // Check overwatch command
                if (ev.Query.Split(' ')[0] == "overwatch" && ev.Query.Split(' ')[1] == ev.Admin.PlayerId.ToString() + ".")
                {
                    if(ev.Query.Split(' ')[2] == "0")
                    {
                        plugin.overwatchToggled.Remove(ev.Admin.SteamId);
                    }
                    else
                    {
                        plugin.overwatchToggled.Add(ev.Admin.SteamId);
                    }
                    plugin.SaveTagsToFile();
                }
            }

        }
    }
}
