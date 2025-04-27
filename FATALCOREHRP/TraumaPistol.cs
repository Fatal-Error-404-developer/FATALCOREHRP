using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FATALCOREHRP
{
    [CustomItem(ItemType.GunCOM18)]
    public class TraumaPistol : CustomWeapon
    {
        public override uint Id { get; set; } = 666;
        public override string Name { get; set; } = "Травматический пистолет";
        public override float Weight { get; set; } = 1.2f;
        public override byte ClipSize { get; set; } = 15;
        public override string Description { get; set; } = "Травматический пистолет с низким уроном для нелетального подавления.";
        public override float Damage { get; set; } = 3f;
        public override bool FriendlyFire { get; set; } = true;
        public override Vector3 Scale { get; set; } = Vector3.one;
        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint
                {
                    Chance = 0,
                    Location = SpawnLocationType.InsideHczArmory
                }
            }
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shot += OnShot;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shot -= OnShot;
            base.UnsubscribeEvents();
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            try
            {
                if (ev.Player == null || ev.Firearm == null || !Check(ev.Firearm) || ev.Target == null)
                    return;
            }
            catch (Exception)
            {
            }
        }
    }
}