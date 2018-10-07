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
        version = "0.0.1",
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
            foreach(string s in tagsToggled)
            {
                this.Info(s);
            }
        }
        
        public override void Register()
        {
            this.AddEventHandlers(new PlayerJoin(this), Priority.High);
            this.AddEventHandlers(new OnCommand(this), Priority.High);
        }
    }
    public class PlayerJoin : IEventHandlerPlayerJoin
    {
        private readonly ToggleTag plugin;
        public PlayerJoin(ToggleTag plugin)
        {
            this.plugin = plugin;
        }
        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if(plugin.tagsToggled.Contains(ev.Player.SteamId))
            {
                ev.Player.HideTag(false);
            }
        }
    }

    public class OnCommand : IEventHandlerAdminQuery
    {
        private readonly ToggleTag plugin;
        public OnCommand(ToggleTag plugin)
        {
            this.plugin = plugin;
        }
        public void OnAdminQuery(AdminQueryEvent ev)
        {
            // Abort if no admin or steamid is included
            if(ev.Admin == null || ev.Admin.SteamId == null)
            {
                return;
            }

            // Check command
            if(ev.Query == "hidetag")
            {
                plugin.tagsToggled.Add(ev.Admin.SteamId);
            }
            else if (ev.Query == "showtag")
            {
                plugin.tagsToggled.Remove(ev.Admin.SteamId);
            }
            else
            {
                return;
            }

            // Save the state to file
            StringBuilder builder = new StringBuilder();
            builder.Append("[\n");
            foreach (string line in plugin.tagsToggled)
            {
                builder.Append("\"" + line + "\"," + "\n");
            }
            builder.Append("]");
            File.WriteAllText(FileManager.AppFolder + "toggletag.json", builder.ToString());
        }
    }
}
