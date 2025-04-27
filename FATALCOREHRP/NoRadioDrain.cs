using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using System;

namespace FATALCOREHRP
{
    public class NoRadioDrain : Plugin<Config>
    {

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.UsingRadioBattery += OnUsingRadioBattery;
            Log.Info("NoRadioDrain включён. Батарея рации всегда на 100%!");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.UsingRadioBattery -= OnUsingRadioBattery;
            Log.Info("NoRadioDrain отключён.");
            base.OnDisabled();
        }

        private void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev)
        {
            try
            {
                // Если у игрока нет рации, выходим
                if (ev.Player.CurrentItem?.Type != ItemType.Radio)
                    return;

                // Отключаем естественный разряд (опционально, но на всякий случай)
                ev.Drain = 0f;
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка в NoRadioDrain: {ex}");
            }
        }
    }
}