using System;
using MEC;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using System.Collections.Generic;

namespace FATALCOREHRP
{
    public class HUDModule
    {
        private CoroutineHandle _hudCoroutine;
        private CoroutineHandle _healthCheckCoroutine;
        private float _roundTime;

        public void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        public void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Timing.KillCoroutines(_hudCoroutine, _healthCheckCoroutine);
        }

        private void OnRoundStarted()
        {
            Timing.KillCoroutines(_hudCoroutine, _healthCheckCoroutine);
            _roundTime = 0f;
            _hudCoroutine = Timing.RunCoroutine(UpdateHud());
            _healthCheckCoroutine = Timing.RunCoroutine(CheckPlayersHealth());
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Timing.KillCoroutines(_hudCoroutine, _healthCheckCoroutine);
        }

        private IEnumerator<float> CheckPlayersHealth()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                MainPlugin.Instance.Medical.ProcessBleeding();
            }
        }

        private IEnumerator<float> UpdateHud()
        {
            while (true)
            {
                UpdateHudForAllPlayers();
                yield return Timing.WaitForSeconds(1f);
                _roundTime += 1f;
            }
        }

        private void UpdateHudForAllPlayers()
        {
            foreach (Player player in Player.List)
            {
                if (player == null || !player.IsConnected) continue;

                TimeSpan timeSpan = TimeSpan.FromSeconds(_roundTime);
                string timeText = $"{timeSpan:hh\\:mm\\:ss}";
                string hudText =
                    $"<align=left><pos=-355>\u200B\u200B\u200B<size=17><b>[<color=#fff>R</color><color=#002aff>U</color><color=#e30000>S</color>] <color=#778899>Чумовой </color> | <color=#A9A9A9>H</color><color=#C0C0C0>a</color><color=#D3D3D3>r</color><color=#D3D3D3>d</color><color=#C0C0C0>RP</color></b></size></pos></align>\n" +
                    $"<align=left><pos=-355><voffset=-0.5em>\u200B\u200B\u200B<b><size=17>ник: <color=white>{player.CustomName}<size=22> | </size>{player.Nickname}</color></size></b></pos></align>\n" +
                    $"<align=left><pos=-355><voffset=-0.5em>\u200B\u200B\u200B<b><size=17>Ваш чарсет: <color=white>{player.CustomInfo}</color></size></b></pos></align>\n" +
                    $"<align=left><pos=-355><voffset=-0.5em>\u200B\u200B\u200B<b><size=17>Состояние: {MainPlugin.Instance.Medical.GetPlayerState(player)}</size></b></pos></align>\n" +
                    $"<align=left><pos=-355><voffset=0.05em>\u200B\u200B\u200B<b><size=17>время раунда: <color=#00FF00>{timeText}</color></size></b></voffset></voffset></pos></align>\n" +
                    $"\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n";
                player.ShowHint(hudText, 1.1f);
            }
        }
    }
}