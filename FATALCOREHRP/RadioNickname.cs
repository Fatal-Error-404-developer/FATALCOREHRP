using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using VoiceChat;

namespace FATALCOREHRP
{
    public class RadioNickname : Plugin<Config>
    {
        public static RadioNickname Instance { get; private set; }


        private readonly Dictionary<Player, string> originalNicknames = new Dictionary<Player, string>();
        private readonly Dictionary<Player, CoroutineHandle> activeCoroutines = new Dictionary<Player, CoroutineHandle>();

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
            Exiled.Events.Handlers.Player.DroppingItem += OnDroppingItem;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.VoiceChatting -= OnVoiceChatting;
            Exiled.Events.Handlers.Player.DroppingItem -= OnDroppingItem;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            foreach (var coroutine in activeCoroutines.Values)
            {
                Timing.KillCoroutines(coroutine);
            }
            originalNicknames.Clear();
            activeCoroutines.Clear();
            Instance = null;
            base.OnDisabled();
        }

        private void OnVoiceChatting(VoiceChattingEventArgs ev)
        {
            if (ev?.Player == null || ev.VoiceMessage.Channel != VoiceChatChannel.Radio)
            {
                return;
            }

            try
            {
                if (ev.Player.Items.Any(item => item.Type == ItemType.Radio))
                {
                    ChangeName(ev.Player);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"OnVoiceChatting error for player {ev.Player.Nickname}: {ex}");
            }
        }

        private void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Player == null || !ev.IsAllowed || ev.Item.Type != ItemType.Radio)
            {
                return;
            }

            Cleanup(ev.Player);
        }

        private void OnPlayerLeft(LeftEventArgs ev)
        {
            if (ev.Player == null)
            {
                return;
            }

            Cleanup(ev.Player);
        }

        private void ChangeName(Player player)
        {
            Cleanup(player);

            originalNicknames[player] = RemoveFormatting(player.CustomName ?? player.Nickname);

            string randomNickname = GenerateRandomNickname();
            player.CustomName = randomNickname;
            player.SendConsoleMessage($"Вы используете рацию. Ваш ник изменён на {randomNickname}.", "Green");

            activeCoroutines[player] = Timing.RunCoroutine(RestoreName(player));
        }

        private IEnumerator<float> RestoreName(Player player)
        {
            float elapsedTime = 0f;
            const float maxWaitTime = 30f;

            while (player != null && player.IsTransmitting && elapsedTime < maxWaitTime)
            {
                elapsedTime += 0.1f;
                yield return Timing.WaitForSeconds(0.1f);
            }

            yield return Timing.WaitForSeconds(0.5f);

            if (player != null && originalNicknames.TryGetValue(player, out string originalName))
            {
                player.CustomName = originalName;
                player.SendConsoleMessage($"Вы перестали использовать рацию. Ваш ник восстановлен: {originalName}.", "Green");
                originalNicknames.Remove(player);
            }

            activeCoroutines.Remove(player);
        }

        private void Cleanup(Player player)
        {

            if (activeCoroutines.TryGetValue(player, out var handle))
            {
                Timing.KillCoroutines(handle);
                activeCoroutines.Remove(player);
            }


            if (originalNicknames.TryGetValue(player, out string originalName))
            {
                player.CustomName = originalName;
                player.SendConsoleMessage($"Вы перестали использовать рацию. Ваш ник восстановлен: {originalName}.", "Green");
                originalNicknames.Remove(player);
            }
        }

        private string GenerateRandomNickname()
        {

            string[] parts = new string[4];
            for (int i = 0; i < 4; i++)
            {
                parts[i] = UnityEngine.Random.Range(0, 10000).ToString("D4");
            }
            return string.Join("-", parts); 
        }

        private string RemoveFormatting(string nickname)
        {
            if (string.IsNullOrEmpty(nickname)) return nickname;

            StringBuilder cleanName = new StringBuilder();
            bool insideTag = false;
            foreach (char ch in nickname)
            {
                if (ch == '<') insideTag = true;
                else if (ch == '>') insideTag = false;
                else if (!insideTag) cleanName.Append(ch);
            }
            return cleanName.ToString().Replace("*", string.Empty);
        }
    }
}
