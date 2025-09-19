using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace FATALCOREHRP
{
    public class SCP049Whitelist : Plugin<Config>
    {
        public static SCP049Whitelist Instance { get; private set; }

        public override string Name => "SCP049Whitelist";
        public override string Author => "YourAuthorName";
        public override Version Version => new Version(1, 0, 0);

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Player.Spawned += OnPlayerSpawned;
            
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnPlayerSpawned;
            Instance = null;
            base.OnDisabled();
        }

        private void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            if (ev.Player.Role.Type == RoleTypeId.Scp049)
            {
                ulong steamId64;
                bool isValidSteamId = ulong.TryParse(ev.Player.RawUserId, out steamId64);
                

                if (!isValidSteamId || !Config.WhitelistedSteamIds.Contains(steamId64))
                {
                    
 
                    RoleTypeId newRole = GetRandomRole();
                    ev.Player.Role.Set(newRole, RoleSpawnFlags.UseSpawnpoint);


                    ev.Player.ShowHint($"Вы не в белом списке для SCP-049! Назначена случайная роль: {GetRoleName(newRole)}.", 5f);
                }
                else
                {
                    
                }
            }
        }

        private RoleTypeId GetRandomRole()
        {

            RoleTypeId[] roles =
            {
                RoleTypeId.ClassD,
                RoleTypeId.Scientist,
                RoleTypeId.FacilityGuard,
                RoleTypeId.Scp173,
                RoleTypeId.Scp106,
                RoleTypeId.Scp096,
                RoleTypeId.Scp939,
                RoleTypeId.Scp079,
            };
            return roles[UnityEngine.Random.Range(0, roles.Length)];
        }

        private string GetRoleName(RoleTypeId role)
        {
            // Возвращаем название роли на русском
            switch (role)
            {
                case RoleTypeId.ClassD: return "D-Класс";
                case RoleTypeId.Scientist: return "Учёный";
                case RoleTypeId.FacilityGuard: return "Охранник";
                case RoleTypeId.Scp173: return "SCP-173";
                case RoleTypeId.Scp106: return "SCP-106";
                case RoleTypeId.Scp096: return "SCP-096";
                case RoleTypeId.Scp939: return "SCP-939";
                case RoleTypeId.Scp079: return "SCP-079";
                default: return role.ToString();
            }
        }
    }
}
