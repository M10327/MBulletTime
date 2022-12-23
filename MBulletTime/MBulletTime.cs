﻿using MBulletTime.Models;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MBulletTime
{
    public class MBulletTime : RocketPlugin<Config>
    {
        public static Config cfg;
        private static System.Timers.Timer timer;
        private static Dictionary<CSteamID, BulletTimeSetting> pls;
        protected override void Load()
        {
            cfg = Configuration.Instance;
            UseableGun.OnAimingChanged_Global += UseableGun_OnAimingChanged_Global;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += UnturnedPlayerEvents_OnPlayerUpdatePosition;
            timer = new System.Timers.Timer(10);
            timer.Elapsed += BulletTimer;
            timer.AutoReset = true;
            timer.Enabled = true;
            pls = new Dictionary<CSteamID, BulletTimeSetting>();
        }

        private void UnturnedPlayerEvents_OnPlayerUpdatePosition(UnturnedPlayer player, UnityEngine.Vector3 position)
        {
            if (pls.ContainsKey(player.CSteamID))
                if (player.Player.movement.isGrounded)
                {
                    pls.Remove(player.CSteamID);
                    SetMovement(player.Player.movement, false);
                }
        }

        private void BulletTimer(object sender, ElapsedEventArgs e)
        {
            foreach (var p in pls.ToArray())
            {
                if (p.Value.Duration > 1)
                {
                    pls[p.Key].Duration -= 10;
                }
                else
                {
                    UnturnedPlayer player = UnturnedPlayer.FromCSteamID(p.Key);
                    SetMovement(player.Player.movement, false);
                }
            }
        }

        private void UseableGun_OnAimingChanged_Global(UseableGun gun)
        {
            var p = gun.player.movement;
            var id = gun.player.channel.owner.playerID.steamID;
            if (p == null) return;
            if (gun.isAiming && !p.isGrounded)
            {
                if (!pls.ContainsKey(id))
                {
                    pls[id] = new BulletTimeSetting() { Allowed = true, Duration = cfg.BulletTimeMS };
                }
                if (pls[id].Duration > 0)
                {
                    if (p.pluginSpeedMultiplier == cfg.DefaultSpeed && p.pluginGravityMultiplier == cfg.DefaultGravity)
                    {
                        SetMovement(p, true);
                    }
                }
                else
                {
                    SetMovement(p, false);
                }
            }
            else 
            {
                if (pls.ContainsKey(id) && cfg.OnlyAllowOncePerJump) pls[id].Duration = 0; // this makes it so you can only do bullet time once per time in air
                if (p.pluginSpeedMultiplier != cfg.DefaultSpeed || p.pluginGravityMultiplier != cfg.DefaultGravity)
                    SetMovement(p, false);
            }
        }

        private void SetMovement(PlayerMovement p, bool on)
        {
            if (on)
            {
                p.sendPluginGravityMultiplier(cfg.Gravity);
                p.sendPluginSpeedMultiplier(cfg.Speed);
            }
            else
            {
                p.sendPluginGravityMultiplier(cfg.DefaultGravity);
                p.sendPluginSpeedMultiplier(cfg.DefaultSpeed);
            }
        }

        protected override void Unload()
        {

            UseableGun.OnAimingChanged_Global += UseableGun_OnAimingChanged_Global;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += UnturnedPlayerEvents_OnPlayerUpdatePosition;
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= BulletTimer;
            }
        }
    }
}
