using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace FATALCOREHRP
{
    public class RoleNickname : Plugin<Config>
    {
        public static RoleNickname Instance { get; private set; }

        private readonly List<string> firstNames = new List<string>
        {
            "Алексей", "Иван", "Дмитрий", "Сергей", "Михаил", "Андрей", "Максим", "Роман", "Артем", "Егор",
            "Никита", "Владимир", "Вячеслав", "Константин", "Валерий", "Тимофей", "Денис", "Кирилл", "Петр", "Виктор"
        };

        private readonly List<string> lastNames = new List<string>
        {
            "Иванов", "Петров", "Сидоров", "Козлов", "Морозов", "Новиков", "Смирнов", "Попов", "Лебедев", "Кузнецов",
            "Григорьев", "Соловьев", "Беляев", "Федоров", "Захаров", "Крылов", "Шевченко", "Чернов", "Тимофеев", "Лавров"
        };

        private readonly List<string> scientistDescriptions = new List<string>
        {
            "Старший научный сотрудник", "Младший научный сотрудник", "Лаборант", "Ведущий исследователь", "Стажёр"
        };

        private readonly List<string> guardDescriptions = new List<string>
        {
            "Рядовой", "Сержант", "Лейтенант", "Капитан", "Офицер охраны"
        };

        private readonly List<string> callsigns = new List<string>
        {
            "Альфа", "Бета", "Гамма", "Дельта", "Эпсилон", "Зета", "Тета", "Йота", "Каппа", "Лямбда",
            "Омега", "Сигма", "Ро", "Пи", "Тау"
        };

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Player.Spawned += OnPlayerSpawned;
            Exiled.Events.Handlers.Player.PickingUpItem += OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnPlayerSpawned;
            Exiled.Events.Handlers.Player.PickingUpItem -= OnPickingUpItem;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Instance = null;
            base.OnDisabled();
        }

        private void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            UpdateNicknameAndInfo(ev.Player, ev.Player.Role.Type);
        }

        private void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (ev.IsAllowed && ev.Player != null)
            {
                UpdateDescriptionForItem(ev.Player, ev.Pickup.Type, true);
            }
        }

        private void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.IsAllowed && ev.Player != null)
            {
                UpdateDescriptionForItem(ev.Player, ev.Item.Type, false);
            }
        }

        private void UpdateNicknameAndInfo(Player player, RoleTypeId role)
        {
            string newNickname = null;
            string description = null;

            if (role == RoleTypeId.ClassD)
            {
                // D-Class: D-XXXX
                string randomNumber = UnityEngine.Random.Range(0, 10000).ToString("D4");
                newNickname = $"D-{randomNumber}";
                description = "D-Class";
            }
            else if (role == RoleTypeId.Scientist)
            {
                // Scientist: Имя-Фамилия Имя, случайное описание
                string firstName = firstNames[UnityEngine.Random.Range(0, firstNames.Count)];
                string lastName = lastNames[UnityEngine.Random.Range(0, lastNames.Count)];
                newNickname = $"{lastName} {firstName}";
                description = scientistDescriptions[UnityEngine.Random.Range(0, scientistDescriptions.Count)];
            }
            else if (role == RoleTypeId.FacilityGuard)
            {
                // Facility Guard: Имя-Фамилия Имя, случайное описание
                string firstName = firstNames[UnityEngine.Random.Range(0, firstNames.Count)];
                string lastName = lastNames[UnityEngine.Random.Range(0, lastNames.Count)];
                newNickname = $"{lastName} {firstName}";
                description = guardDescriptions[UnityEngine.Random.Range(0, guardDescriptions.Count)];
            }
            else if (IsMtfRole(role))
            {
                // MTF: Позывной-Число, описание по роли
                string callsign = callsigns[UnityEngine.Random.Range(0, callsigns.Count)];
                string number = UnityEngine.Random.Range(1, 100).ToString("D2");
                newNickname = $"{callsign}-{number}";
                description = GetMtfDescription(role);
            }
            else if (IsChaosRole(role))
            {
                // Chaos: Позывной-Число, описание по роли
                string callsign = callsigns[UnityEngine.Random.Range(0, callsigns.Count)];
                string number = UnityEngine.Random.Range(1, 100).ToString("D2");
                newNickname = $"{callsign}-{number}";
                description = GetChaosDescription(role);
            }
            else if (role == RoleTypeId.Spectator || role == RoleTypeId.Overwatch)
            {
                // SCP и Observer: ???
                newNickname = player.Nickname;
                description = "???";
            }
            else if (IsScpRole(role) )
            {
                // SCP и Observer: ???
                newNickname = "???";
                description = "???";
            }

            if (newNickname != null)
            {
                // Устанавливаем ник через CustomName
                player.CustomName = newNickname;

                // Добавляем информацию о броне и оружии (кроме SCP и Observer)
                if (description != null && role != RoleTypeId.Spectator && !IsScpRole(role))
                {
                    description = AddInventoryInfo(player, description);
                }

                // Устанавливаем описание через CustomInfo, если есть
                if (description != null)
                {
                    player.CustomInfo = description;
                }

                // Уведомляем игрока
                string message = description != null
                    ? $"Ваш ник изменён на {newNickname}. Описание: {description}."
                    : $"Ваш ник изменён на {newNickname}.";
                player.SendConsoleMessage(message, "Green");
            }
        }

        private void UpdateDescriptionForItem(Player player, ItemType itemType, bool isPickingUp)
        {
            if (player.Role.Type == RoleTypeId.Spectator || IsScpRole(player.Role.Type))
            {
                return; // Не обновляем для SCP и Spectator
            }

            string baseDescription = GetBaseDescription(player.Role.Type);
            string currentDescription = baseDescription ?? string.Empty;

            // Проверяем, является ли предмет бронежилетом или оружием
            string itemDescription = GetItemDescription(itemType);
            if (itemDescription == null)
            {
                return; // Предмет не бронежилет и не оружие
            }

            // Получаем текущее состояние инвентаря
            string armorDescription = null;
            string weaponDescription = null;

            // Если подбираем предмет, добавляем его в инвентарь для проверки
            if (isPickingUp)
            {
                foreach (var item in player.Items)
                {
                    string desc = GetItemDescription(item.Type);
                    if (desc != null)
                    {
                        if (IsArmor(item.Type) && armorDescription == null)
                        {
                            armorDescription = desc;
                        }
                        else if (IsWeapon(item.Type) && weaponDescription == null)
                        {
                            weaponDescription = desc;
                        }
                    }
                }
                // Добавляем новый предмет
                if (IsArmor(itemType))
                {
                    armorDescription = itemDescription;
                }
                else if (IsWeapon(itemType))
                {
                    weaponDescription = itemDescription;
                }
            }
            // Если выкидываем предмет, исключаем его из инвентаря
            else
            {
                foreach (var item in player.Items)
                {
                    if (item.Type != itemType) // Игнорируем выброшенный предмет
                    {
                        string desc = GetItemDescription(item.Type);
                        if (desc != null)
                        {
                            if (IsArmor(item.Type) && armorDescription == null)
                            {
                                armorDescription = desc;
                            }
                            else if (IsWeapon(item.Type) && weaponDescription == null)
                            {
                                weaponDescription = desc;
                            }
                        }
                    }
                }
            }

            // Формируем новое описание с запятыми
            List<string> descriptionParts = new List<string>();
            if (!string.IsNullOrEmpty(baseDescription))
            {
                descriptionParts.Add(baseDescription);
            }
            if (armorDescription != null)
            {
                descriptionParts.Add($"надет {armorDescription}");
            }
            if (weaponDescription != null)
            {
                descriptionParts.Add($"на спине висит {weaponDescription}");
            }

            currentDescription = string.Join(", ", descriptionParts);

            // Устанавливаем новое описание
            player.CustomInfo = currentDescription;

            // Уведомляем игрока
            string message = $"Ваш ник: {player.CustomName}. Описание: {currentDescription}.";
            player.SendConsoleMessage(message, "Green");
        }

        private string AddInventoryInfo(Player player, string baseDescription)
        {
            string armorDescription = null;
            string weaponDescription = null;

            // Проверяем инвентарь
            foreach (var item in player.Items)
            {
                switch (item.Type)
                {
                    case ItemType.ArmorLight:
                        armorDescription = "лёгкий бронежилет";
                        break;
                    case ItemType.ArmorCombat:
                        armorDescription = "боевой бронежилет";
                        break;
                    case ItemType.ArmorHeavy:
                        armorDescription = "тяжёлый бронежилет";
                        break;
                    case ItemType.GunCrossvec:
                        weaponDescription = "Crossvec";
                        break;
                    case ItemType.GunLogicer:
                        weaponDescription = "Logicer";
                        break;
                    case ItemType.GunE11SR:
                        weaponDescription = "E11SR";
                        break;
                    case ItemType.GunFRMG0:
                        weaponDescription = "FRMG0";
                        break;
                    case ItemType.GunA7:
                        weaponDescription = "A7";
                        break;
                    case ItemType.GunShotgun:
                        weaponDescription = "Shotgun";
                        break;
                    case ItemType.GunAK:
                        weaponDescription = "AK";
                        break;
                    case ItemType.ParticleDisruptor:
                        weaponDescription = "Particle Disruptor";
                        break;
                    case ItemType.GunFSP9:
                        weaponDescription = "FSP9";
                        break;
                }
                if (armorDescription != null && weaponDescription != null)
                {
                    break; // Выходим, если нашли оба
                }
            }

            // Формируем описание с запятыми
            List<string> descriptionParts = new List<string>();
            if (!string.IsNullOrEmpty(baseDescription))
            {
                descriptionParts.Add(baseDescription);
            }
            if (armorDescription != null)
            {
                descriptionParts.Add($"надет {armorDescription}");
            }
            if (weaponDescription != null)
            {
                descriptionParts.Add($"на спине висит {weaponDescription}");
            }

            return string.Join(", ", descriptionParts);
        }

        private string GetItemDescription(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.ArmorLight:
                    return "лёгкий бронежилет";
                case ItemType.ArmorCombat:
                    return "боевой бронежилет";
                case ItemType.ArmorHeavy:
                    return "тяжёлый бронежилет";
                case ItemType.GunCrossvec:
                    return "Crossvec";
                case ItemType.GunLogicer:
                    return "Logicer";
                case ItemType.GunE11SR:
                    return "E11SR";
                case ItemType.GunFRMG0:
                    return "FRMG0";
                case ItemType.GunA7:
                    return "A7";
                case ItemType.GunShotgun:
                    return "Shotgun";
                case ItemType.GunAK:
                    return "AK";
                case ItemType.ParticleDisruptor:
                    return "Particle Disruptor";
                case ItemType.GunFSP9:
                    return "FSP9";
                default:
                    return null; // Не бронежилет и не оружие
            }
        }

        private bool IsArmor(ItemType itemType)
        {
            return itemType == ItemType.ArmorLight ||
                   itemType == ItemType.ArmorCombat ||
                   itemType == ItemType.ArmorHeavy;
        }

        private bool IsWeapon(ItemType itemType)
        {
            return itemType == ItemType.GunCrossvec ||
                   itemType == ItemType.GunLogicer ||
                   itemType == ItemType.GunE11SR ||
                   itemType == ItemType.GunFRMG0 ||
                   itemType == ItemType.GunA7 ||
                   itemType == ItemType.GunShotgun ||
                   itemType == ItemType.GunAK ||
                   itemType == ItemType.ParticleDisruptor ||
                   itemType == ItemType.GunFSP9;
        }

        private string GetBaseDescription(RoleTypeId role)
        {
            if (role == RoleTypeId.ClassD)
            {
                return "D-Class";
            }
            else if (role == RoleTypeId.Scientist)
            {
                return scientistDescriptions[UnityEngine.Random.Range(0, scientistDescriptions.Count)];
            }
            else if (role == RoleTypeId.FacilityGuard)
            {
                return guardDescriptions[UnityEngine.Random.Range(0, guardDescriptions.Count)];
            }
            else if (IsMtfRole(role))
            {
                return GetMtfDescription(role);
            }
            else if (IsChaosRole(role))
            {
                return GetChaosDescription(role);
            }
            return null; // Для других ролей без базового описания
        }

        private bool IsMtfRole(RoleTypeId role)
        {
            return role == RoleTypeId.NtfPrivate ||
                   role == RoleTypeId.NtfSergeant ||
                   role == RoleTypeId.NtfCaptain ||
                   role == RoleTypeId.NtfSpecialist;
        }

        private bool IsChaosRole(RoleTypeId role)
        {
            return role == RoleTypeId.ChaosConscript ||
                   role == RoleTypeId.ChaosRifleman ||
                   role == RoleTypeId.ChaosRepressor ||
                   role == RoleTypeId.ChaosMarauder;
        }

        private bool IsScpRole(RoleTypeId role)
        {
            return role == RoleTypeId.Scp049 ||
                   role == RoleTypeId.Scp079 ||
                   role == RoleTypeId.Scp096 ||
                   role == RoleTypeId.Scp106 ||
                   role == RoleTypeId.Scp173 ||
                   role == RoleTypeId.Scp939;
        }

        private string GetMtfDescription(RoleTypeId role)
        {
            switch (role)
            {
                case RoleTypeId.NtfPrivate: return "Рядовой";
                case RoleTypeId.NtfSergeant: return "Сержант";
                case RoleTypeId.NtfCaptain: return "Капитан";
                case RoleTypeId.NtfSpecialist: return "Специалист";
                default: return "Оперативник";
            }
        }

        private string GetChaosDescription(RoleTypeId role)
        {
            switch (role)
            {
                case RoleTypeId.ChaosConscript: return "Призывник";
                case RoleTypeId.ChaosRifleman: return "Стрелок";
                case RoleTypeId.ChaosRepressor: return "Репрессор";
                case RoleTypeId.ChaosMarauder: return "Мародёр";
                default: return "Боевик";
            }
        }
    }
}