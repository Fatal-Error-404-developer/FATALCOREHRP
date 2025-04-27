using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomItems.API;
using Exiled.CustomItems.API.Features;

namespace FATALCOREHRP
{
    public class MainPlugin : Plugin<Config>
    {
        public override string Name => "FatalCore";
        public override string Author => "Твой ник";
        public override Version Version => new Version(1, 1, 0);

        public static MainPlugin Instance { get; private set; }
        public HUDModule HUDModule { get; set; }
        public NoRadioDrain _NoRadioDrain { get; set; }
        public Medical Medical { get; set; }
        public CustomItemSpawner ItemSpawner { get; set; }
        public RoleNickname RoleNickname { get; set; }
        public SCP049Whitelist SCP049Whitelist { get; set; }
        public RadioNickname RadioNickname { get; set; }

        public override void OnEnabled()
        {
            Instance = this;

            // Очистка предметов ПЕРЕД регистрацией
            try
            {
                CustomItem.UnregisterItems();
                Log.Debug("Все кастомные предметы были отменены перед регистрацией");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка очистки предметов: {ex}");
            }

            // Инициализация модулей
            ItemSpawner = new CustomItemSpawner();
            HUDModule = new HUDModule();
            Medical = new Medical();
            _NoRadioDrain = new NoRadioDrain();
            RoleNickname = new RoleNickname();
            SCP049Whitelist = new SCP049Whitelist();
            RadioNickname = new RadioNickname();

            // Регистрация кастомных предметов
            CustomItem.RegisterItems();

            // Активация модулей
            EnableModules();

            Log.Info("<color=#2DDAFD>Плагин FatalCore успешно запущен</color>");
            base.OnEnabled();
        }

       

        private void EnableModules()
        {
            try
            {
                ItemSpawner.OnEnabled();
                HUDModule.OnEnabled();
                Medical.OnEnabled();
                _NoRadioDrain.OnEnabled();
                RoleNickname.OnEnabled();
                SCP049Whitelist.OnEnabled();
                RadioNickname.OnEnabled();
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при включении модулей: {ex}");
            }
        }

        public override void OnDisabled()
        {
            // Деактивация модулей
            DisableModules();

            // Отмена регистрации предметов
            CustomItem.UnregisterItems();

            // Очистка
            HUDModule = null;
            ItemSpawner = null;
            Medical = null;
            RoleNickname = null;
            SCP049Whitelist = null;
            _NoRadioDrain = null;
            RadioNickname = null;
            Instance = null;

            Log.Info("<color=#2DDAFD>Плагин FatalCore выгружен</color>");
            base.OnDisabled();
        }

        private void DisableModules()
        {
            try
            {
                ItemSpawner?.OnDisabled();
                HUDModule?.OnDisabled();
                Medical?.OnDisabled();
                _NoRadioDrain?.OnDisabled();
                RoleNickname?.OnDisabled();
                SCP049Whitelist?.OnDisabled();
                RadioNickname?.OnDisabled();
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при отключении модулей: {ex}");
            }
        }
    }
}