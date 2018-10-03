using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smod2;
using Smod2.Attributes;
using Smod2.Commands;
using Smod2.EventHandlers;
using Smod2.Events;

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
        public override void OnDisable()
        {
            throw new NotImplementedException();
        }

        public override void OnEnable()
        {
            throw new NotImplementedException();
        }

        public override void Register()
        {
            this.AddEventHandlers(new PlayerJoin(this), Priority.Highest);
        }
    }
    public class PlayerJoin : IEventHandlerPlayerJoin
    {
        private ToggleTag plugin;
        public PlayerJoin(ToggleTag plugin)
        {
            this.plugin = plugin;
        }
        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            throw new NotImplementedException();
        }
    }

    public class OnCommand : IEventHandlerAdminQuery
    {
        private ToggleTag plugin;
        public OnCommand(ToggleTag plugin)
        {
            this.plugin = plugin;
        }
        public void OnAdminQuery(AdminQueryEvent ev)
        {
            
        }
    }
}
