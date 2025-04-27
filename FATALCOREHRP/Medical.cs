using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerStatsSystem;
using Exiled.API.Enums;
using InventorySystem.Items.Usables;
using PlayerRoles;
using UnityEngine;
using Exiled.API.Features.Items;
using MEC;
using CommandSystem;
using RemoteAdmin;

namespace FATALCOREHRP
{
    public class Medical
    {
        private readonly Dictionary<Player, string> _playerStates = new Dictionary<Player, string>();
        private readonly Dictionary<Player, bool> _adrenalineUsed = new Dictionary<Player, bool>();
        private readonly Dictionary<Player, float> _stainedEffectTimers = new Dictionary<Player, float>();
        private readonly Dictionary<Player, float> _legInjuryTimers = new Dictionary<Player, float>();
        private readonly Dictionary<Player, float> _armInjuryTimers = new Dictionary<Player, float>();
        private readonly Dictionary<Player, CoroutineHandle> _healingCoroutines = new Dictionary<Player, CoroutineHandle>();

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Spawned += OnPlayerSpawned;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.Events.Handlers.Player.UsingItem += OnPlayerUsingItem;
            Exiled.Events.Handlers.Player.UsedItem += OnPlayerUsedItem;
            Exiled.Events.Handlers.Player.Shot += OnShot;
            Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnPlayerSpawned;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            Exiled.Events.Handlers.Player.UsingItem -= OnPlayerUsingItem;
            Exiled.Events.Handlers.Player.UsedItem -= OnPlayerUsedItem;
            Exiled.Events.Handlers.Player.Shot -= OnShot;
            Exiled.Events.Handlers.Player.ChangingItem -= OnChangingItem;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
        }

        public string GetPlayerState(Player player)
        {
            if (_legInjuryTimers.TryGetValue(player, out var legTimer) && legTimer > 0)
            {
                return "<color=orange>Повреждена нога</color>";
            }
            if (_armInjuryTimers.TryGetValue(player, out var armTimer) && armTimer > 0)
            {
                return "<color=orange>Повреждена рука</color>";
            }
            return _playerStates.TryGetValue(player, out var state) ? state : "<color=green>Отличное</color>";
        }

        private void OnWaitingForPlayers()
        {
            _playerStates.Clear();
            _adrenalineUsed.Clear();
            _stainedEffectTimers.Clear();
            _legInjuryTimers.Clear();
            _armInjuryTimers.Clear();
            foreach (var coroutine in _healingCoroutines.Values)
            {
                Timing.KillCoroutines(coroutine);
            }
            _healingCoroutines.Clear();
        }

        private void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            _playerStates[ev.Player] = ev.Player.Role.Side == Side.Scp || ev.Player.Role == RoleTypeId.Spectator || ev.Player.Role == RoleTypeId.Overwatch || ev.Player is Npc
                ? "<color=purple>Неизвестно</color>"
                : "<color=green>Отличное</color>";
            _adrenalineUsed[ev.Player] = false;
            _stainedEffectTimers[ev.Player] = 0f;
            _legInjuryTimers[ev.Player] = 0f;
            _armInjuryTimers[ev.Player] = 0f;
        }

        private void OnPlayerDied(DiedEventArgs ev)
        {
            _playerStates.Remove(ev.Player);
            _adrenalineUsed.Remove(ev.Player);
            _stainedEffectTimers.Remove(ev.Player);
            _legInjuryTimers.Remove(ev.Player);
            _armInjuryTimers.Remove(ev.Player);
            if (_healingCoroutines.TryGetValue(ev.Player, out var coroutine))
            {
                Timing.KillCoroutines(coroutine);
                _healingCoroutines.Remove(ev.Player);
            }
        }

        private void OnShot(ShotEventArgs ev)
        {
            if (ev.Target == null || ev.Target.IsDead)
                return;

            if (ev.Hitbox._dmgMultiplier == HitboxType.Limb)
            {
                
                
                    _legInjuryTimers[ev.Target] = 10000000f;
                    ev.Target.EnableEffect(EffectType.Stained, 10000000f);
                    _stainedEffectTimers[ev.Target] = 10000000f;
                
            }
            else if (ev.Hitbox._dmgMultiplier == HitboxType.Body)
            {
                _armInjuryTimers[ev.Target] = 10000000f;
                if (ev.Target.CurrentItem is Exiled.API.Features.Items.Firearm firearm && !IsPistol(firearm))
                {
                    ev.Target.RemoveItem(ev.Target.CurrentItem);
                }
            }
        }

        private void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (_armInjuryTimers.TryGetValue(ev.Player, out var armTimer) && armTimer > 0)
            {
                if (ev.Item is Exiled.API.Features.Items.Firearm firearm && !IsPistol(firearm))
                {
                    ev.IsAllowed = false;
                    ev.Player.ShowHint("Вы не можете использовать это оружие с поврежденной рукой!", 3f);
                }
            }
        }

        private bool IsPistol(Exiled.API.Features.Items.Firearm firearm)
        {
            return firearm.Type == ItemType.GunCOM15 ||
                   firearm.Type == ItemType.GunCOM18 ||
                   firearm.Type == ItemType.GunRevolver;
        }

        private void OnPlayerUsingItem(UsingItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.Painkillers && ev.Player.Health <= 51)
            {
                ev.IsAllowed = false;
            }
        }

        private void OnPlayerUsedItem(UsedItemEventArgs ev)
        {
            if (ev.Item.Type == ItemType.Adrenaline)
            {
                _adrenalineUsed[ev.Player] = true;
            }
        }

        public void ProcessBleeding()
        {
            foreach (Player player in Player.List)
            {
                if (player == null || !player.IsConnected || player.IsDead) continue;

                if (player.Role.Side == Side.Scp || player.Role == RoleTypeId.Spectator || player.Role.Type == RoleTypeId.Overwatch)
                {
                    _playerStates[player] = "<color=purple>Неизвестно</color>";
                    continue;
                }

                if (_stainedEffectTimers.ContainsKey(player) && _stainedEffectTimers[player] > 0)
                {
                    _stainedEffectTimers[player] -= Time.deltaTime;
                    if (_stainedEffectTimers[player] <= 0)
                    {
                        player.DisableEffect(EffectType.Stained);
                        _stainedEffectTimers[player] = 0f;
                    }
                }

                if (_legInjuryTimers.ContainsKey(player) && _legInjuryTimers[player] > 0)
                {
                    _legInjuryTimers[player] -= Time.deltaTime;
                    if (_legInjuryTimers[player] <= 0)
                    {
                        _legInjuryTimers[player] = 0f;
                    }
                }

                if (_armInjuryTimers.ContainsKey(player) && _armInjuryTimers[player] > 0)
                {
                    _armInjuryTimers[player] -= Time.deltaTime;
                    if (_armInjuryTimers[player] <= 0)
                    {
                        _armInjuryTimers[player] = 0f;
                    }
                }

                if (player.Health <= 0)
                {
                    player.Hurt(new CustomReasonDamageHandler("Кровотечение"));
                    continue;
                }

                if (player.Health <= 51)
                {
                    bool adrenalineUsed = _adrenalineUsed.TryGetValue(player, out var used) && used;
                    float damage = adrenalineUsed ? 30f : 2f;
                    player.Health -= damage;

                    if (player.Health <= 0)
                    {
                        player.Hurt(new CustomReasonDamageHandler("Кровотечение"));
                        continue;
                    }

                    _playerStates[player] = "<color=red>Кровотечение</color>";
                    _adrenalineUsed[player] = false;
                }
                else if (player.Health < 65)
                {
                    _playerStates[player] = "<color=yellow>Удовлетворительное</color>";
                }
                else if (player.Health < 80)
                {
                    _playerStates[player] = "<color=#FFA500>Нормальное</color>";
                }
                else
                {
                    _playerStates[player] = "<color=green>Отличное</color>";
                }
            }
        }

        public bool HealLimb(Player player, string limb)
        {
            if (_healingCoroutines.ContainsKey(player))
            {
                player.SendConsoleMessage("Вы уже лечите конечность!", "green");
                return false;
            }

            if (player.CurrentItem == null || player.CurrentItem.Type != ItemType.Medkit)
            {
                player.SendConsoleMessage("Для лечения необходимо держать аптечку в руках!", "yellow");
                return false;
            }

            if (limb.ToLower() == "нога" && _legInjuryTimers.TryGetValue(player, out var legTimer) && legTimer > 0)
            {
                _healingCoroutines[player] = Timing.RunCoroutine(HealLimbCoroutine(player, limb, _legInjuryTimers, _stainedEffectTimers));
                return true;
            }
            else if (limb.ToLower() == "рука" && _armInjuryTimers.TryGetValue(player, out var armTimer) && armTimer > 0)
            {
                _healingCoroutines[player] = Timing.RunCoroutine(HealLimbCoroutine(player, limb, _armInjuryTimers));
                return true;
            }

            player.SendConsoleMessage("Указанная конечность не повреждена!", "red");
            return false;
        }

        private IEnumerator<float> HealLimbCoroutine(Player player, string limb, Dictionary<Player, float> timerDict, Dictionary<Player, float> stainedDict = null)
        {
            player.SendConsoleMessage($"Начато лечение {limb}... Оставайтесь на месте 20 секунд, держите аптечку.", "Green");
            float healTime = 20f;
            Vector3 startPosition = player.Position;

            while (healTime > 0)
            {
                if (player.CurrentItem == null || player.CurrentItem.Type != ItemType.Medkit)
                {
                    player.SendConsoleMessage("Лечение прервано: вы не держите аптечку!", "red");
                    _healingCoroutines.Remove(player);
                    yield break;
                }

                if (Vector3.Distance(player.Position, startPosition) > 1f)
                {
                    player.SendConsoleMessage("Лечение прервано: вы переместились!", "red");
                    _healingCoroutines.Remove(player);
                    yield break;
                }

                healTime -= Time.deltaTime;
                player.SendConsoleMessage($"Лечение {limb}: {Mathf.Ceil(healTime)} сек. осталось", "green");
                yield return Timing.WaitForSeconds(0.1f);
            }

            timerDict[player] = 0f;
            if (stainedDict != null)
            {
                stainedDict[player] = 0f;
                player.DisableEffect(EffectType.Stained);
            }

            player.ShowHint($"{limb} вылечена!", 3f);
            _healingCoroutines.Remove(player);
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class HealCommand : ICommand
    {
        private readonly Medical _medical;
        public string Command => "heal";
        public string[] Aliases => new[] { "h" };
        public string Description => "Лечит поврежденную конечность игрока";

        public HealCommand()
        {
            _medical = MainPlugin.Instance.Medical;
        }

        public bool Execute(System.ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Использование: .heal <нога/рука>";
                return false;
            }

            if (!(sender is PlayerCommandSender playerSender))
            {
                response = "Эта команда может быть выполнена только игроком.";
                return false;
            }

            Player player = Player.Get(playerSender.ReferenceHub);
            string limb = arguments.At(0).ToLower();

            if (limb != "нога" && limb != "рука")
            {
                response = "Укажите 'нога' или 'рука'.";
                return false;
            }

            bool success = _medical.HealLimb(player, limb);
            response = success ? $"Начато лечение {limb}..." : "Не удалось начать лечение.";
            return success;
        }
    }
}