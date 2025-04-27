using System.Collections.Generic;
using System.ComponentModel;

namespace FATALCOREHRP
{
    public class Config : Exiled.API.Interfaces.IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("List of SteamID64 allowed to play as SCP-049 (e.g., 7656119XXXXXXXXXX)")]
        public List<ulong> WhitelistedSteamIds { get; set; } = new List<ulong>
        {
            76561199597046617,
            76561190000000001
        };

        [Description("Enable CustomItemSpawner functionality")]
        public bool CustomItemSpawnerEnabled { get; set; } = true;

        [Description("Enable debug logging for CustomItemSpawner")]
        public bool CustomItemSpawnerDebug { get; set; } = true;
    }
}