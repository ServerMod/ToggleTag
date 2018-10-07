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

namespace ToggleTag
{
    [PluginDetails(
        author = "Karl Essinger",
        name = "ToggleTag",
        description = "Persistant toggeling of role tags.",
        id = "karlofduty.toggletag",
        version = "1.0.0",
        SmodMajor = 3,
        SmodMinor = 1,
        SmodRevision = 18
    )]
    public class ToggleTag : Plugin
    {
        public HashSet<string> tagsToggled;

        public override void OnDisable()
        {
            
        }

        public override void OnEnable()
        {
            if(!File.Exists(FileManager.AppFolder + "toggletag.json"))
            {
                File.WriteAllText(FileManager.AppFolder + "toggletag.json", "[]");
            }
            JArray jsonObject = JArray.Parse(File.ReadAllText(FileManager.AppFolder + "toggletag.json"));
            tagsToggled = new HashSet<string>(jsonObject.Values<string>());
        }
        
        public override void Register()
        {
            this.AddEventHandlers(new PlayerJoinHandler(this), Priority.High);
            this.AddEventHandlers(new TagCommandHandler(this), Priority.High);
            this.AddCommand("console_hidetag", new HideTagCommand(this));
            this.AddCommand("console_showtag", new ShowTagCommand(this));
        }

        public void SaveTagsToFile()
        {
            // Save the state to file
            StringBuilder builder = new StringBuilder();
            builder.Append("[\n");
            foreach (string line in tagsToggled)
            {
                builder.Append("\"" + line + "\"," + "\n");
            }
            builder.Append("]");
            File.WriteAllText(FileManager.AppFolder + "toggletag.json", builder.ToString());
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
                plugin.Info("Tag hidden for " + ev.Player.Name);
            }
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
            if(ev.Admin == null || ev.Admin.SteamId == null)
            {
                return;
            }

            // Check normal version of command
            if(ev.Query == "hidetag")
            {
                plugin.tagsToggled.Add(ev.Admin.SteamId);
                plugin.SaveTagsToFile();
            }
            else if (ev.Query == "showtag")
            {
                plugin.tagsToggled.Remove(ev.Admin.SteamId);
                plugin.SaveTagsToFile();
            }
        }
    }
}
