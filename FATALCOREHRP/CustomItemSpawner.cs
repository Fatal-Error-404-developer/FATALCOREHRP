using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace FATALCOREHRP
{
    public class CustomItemSpawner
    {
        private readonly Dictionary<RoleTypeId, List<(bool isCustom, uint customItemId, ItemType itemType)>> roleItems;

        public CustomItemSpawner()
        {
            // Инициализация предметов для каждой роли
            roleItems = new Dictionary<RoleTypeId, List<(bool, uint, ItemType)>>
            {
                // Class D
                [RoleTypeId.ClassD] = new List<(bool, uint, ItemType)>
                {
                    (false, 0, ItemType.Coin),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.Painkillers),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None)
                },
                // Scientist
                [RoleTypeId.Scientist] = new List<(bool, uint, ItemType)>
                {
                    (false, 0, ItemType.KeycardScientist),
                    (false, 0, ItemType.Medkit),
                    (false, 0, ItemType.Flashlight),
                    (false, 0, ItemType.Radio),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None)
                },
                // Facility Guard
                [RoleTypeId.FacilityGuard] = new List<(bool, uint, ItemType)>
                {
                    (false, 0, ItemType.GunFSP9),
                    (false, 0, ItemType.Medkit),
                    (false, 0, ItemType.KeycardGuard),
                    (false, 0, ItemType.ArmorLight),
                    (true, 666, ItemType.None),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.Radio),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.None)
                },
                // MTF Private
                [RoleTypeId.NtfPrivate] = new List<(bool, uint, ItemType)>
                {
                    (false, 0, ItemType.GunCrossvec),
                    (false, 0, ItemType.Medkit),
                    (false, 0, ItemType.KeycardMTFOperative),
                    (false, 0, ItemType.ArmorCombat),
                    (false, 0, ItemType.Radio),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.Ammo9x19),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None)
                },
                // MTF Sergeant
                [RoleTypeId.NtfSergeant] = new List<(bool, uint, ItemType)>
                {
                    (false, 0, ItemType.GunE11SR),
                    (false, 0, ItemType.Medkit),
                    (false, 0, ItemType.KeycardMTFOperative),
                    (false, 0, ItemType.ArmorCombat),
                    (false, 0, ItemType.Radio),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None)
                },
                // MTF Captain
                [RoleTypeId.NtfCaptain] = new List<(bool, uint, ItemType)>
                {
                    (false, 0, ItemType.GunE11SR),
                    (false, 0, ItemType.Medkit),
                    (false, 0, ItemType.KeycardMTFCaptain),
                    (false, 0, ItemType.ArmorCombat),
                    (false, 0, ItemType.Radio),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None)
                },
                // MTF Specialist
                [RoleTypeId.NtfSpecialist] = new List<(bool, uint, ItemType)>
                {
                    (false, 0, ItemType.GunE11SR),
                    (false, 0, ItemType.Medkit),
                    (false, 0, ItemType.KeycardMTFOperative),
                    (false, 0, ItemType.ArmorCombat),
                    (true, 666, ItemType.None),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.Radio),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.Ammo556x45),
                    (false, 0, ItemType.None)
                },
                // Chaos Rifleman
                [RoleTypeId.ChaosRifleman] = new List<(bool, uint, ItemType)>
                {
                    (true, 666, ItemType.None),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.GunAK),
                    (false, 0, ItemType.KeycardChaosInsurgency),
                    (false, 0, ItemType.ArmorCombat),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None)
                },
                // Chaos Marauder
                [RoleTypeId.ChaosMarauder] = new List<(bool, uint, ItemType)>
                {
                    (true, 666, ItemType.None),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.GunAK),
                    (false, 0, ItemType.KeycardChaosInsurgency),
                    (false, 0, ItemType.ArmorCombat),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.Ammo44cal),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None)
                },
                // Chaos Conscript
                [RoleTypeId.ChaosConscript] = new List<(bool, uint, ItemType)>
                {
                    (true, 666, ItemType.None),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.GunAK),
                    (false, 0, ItemType.KeycardChaosInsurgency),
                    (false, 0, ItemType.ArmorLight),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None)
                },
                // Chaos Repressor
                [RoleTypeId.ChaosRepressor] = new List<(bool, uint, ItemType)>
                {
                    (true, 666, ItemType.None),           // TraumaPistol (ID: 3)
                    (false, 0, ItemType.GunLogicer),
                    (false, 0, ItemType.KeycardChaosInsurgency),
                    (false, 0, ItemType.ArmorHeavy),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.Ammo762x39),
                    (false, 0, ItemType.None),
                    (false, 0, ItemType.None)
                }
            };
        }

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
        }

        private void OnSpawned(SpawnedEventArgs ev)
        {
            try
            {
                if (ev.Player == null || !MainPlugin.Instance.Config.CustomItemSpawnerEnabled)
                {
                    if (MainPlugin.Instance.Config.CustomItemSpawnerDebug)
                        Log.Debug($"[CustomItemSpawner] Игнорирование спавна: Игрок = {ev.Player?.Nickname}, CustomItemSpawnerEnabled = {MainPlugin.Instance.Config.CustomItemSpawnerEnabled}");
                    return;
                }

                if (!roleItems.TryGetValue(ev.Player.Role.Type, out var items) || items == null)
                {
                    if (MainPlugin.Instance.Config.CustomItemSpawnerDebug)
                        Log.Debug($"[CustomItemSpawner] Нет предметов для роли {ev.Player.Role.Type}");
                    return;
                }

                // Очищаем инвентарь игрока после спавна
                ev.Player.ClearInventory();

                foreach (var (isCustom, customItemId, itemType) in items)
                {
                    // Пропускаем пустые слоты
                    if (!isCustom && itemType == ItemType.None)
                    {
                        if (MainPlugin.Instance.Config.CustomItemSpawnerDebug)
                            Log.Debug($"[CustomItemSpawner] Пропущен пустой слот для {ev.Player.Nickname} ({ev.Player.Role.Type})");
                        continue;
                    }

                    // Выдача кастомного предмета
                    if (isCustom && customItemId > 0)
                    {
                        var customItem = Exiled.CustomItems.API.Features.CustomItem.Get(customItemId);
                        if (customItem != null)
                        {
                            customItem.Give(ev.Player);
                            if (MainPlugin.Instance.Config.CustomItemSpawnerDebug)
                                Log.Debug($"[CustomItemSpawner] Выдан кастомный предмет {customItem.Name} (ID: {customItemId}) игроку {ev.Player.Nickname} ({ev.Player.Role.Type})");
                        }
                        else
                        {
                            Log.Warn($"[CustomItemSpawner] Кастомный предмет с ID {customItemId} не найден для роли {ev.Player.Role.Type}");
                        }
                    }
                    // Выдача обычного предмета
                    else if (itemType != ItemType.None)
                    {
                        ev.Player.AddItem(itemType);
                        if (MainPlugin.Instance.Config.CustomItemSpawnerDebug)
                            Log.Debug($"[CustomItemSpawner] Выдан обычный предмет {itemType} игроку {ev.Player.Nickname} ({ev.Player.Role.Type})");
                    }
                }

                // Проверяем инвентарь после выдачи
                if (MainPlugin.Instance.Config.CustomItemSpawnerDebug)
                {
                    Log.Debug($"[CustomItemSpawner] Инвентарь игрока {ev.Player.Nickname} ({ev.Player.Role.Type}) после выдачи:");
                    foreach (var item in ev.Player.Items)
                    {
                        Log.Debug($" - {item.Type} (Serial: {item.Serial})");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[CustomItemSpawner] Ошибка при выдаче предметов игроку {ev.Player?.Nickname}: {ex}");
            }
        }
    }
}