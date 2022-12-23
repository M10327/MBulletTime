﻿using MBulletTime.Models;
using Rocket.Core.Plugins;
using Rocket.Unturned;
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
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace MBulletTime
{
    public class MBulletTime : RocketPlugin<Config>
    {
        public static Config cfg;
        public static System.Timers.Timer timer;
        public static Dictionary<CSteamID, BulletTimeSetting> bulletTime;
        public static Dictionary<CSteamID, int> doubleJump;
        public static Dictionary<CSteamID, PlayerMeta> meta;
        public static MBulletTime Instance;
        protected override void Load()
        {
            cfg = Configuration.Instance;
            UseableGun.OnAimingChanged_Global += UseableGun_OnAimingChanged_Global;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += UnturnedPlayerEvents_OnPlayerUpdatePosition;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            PlayerInputListener.PlayerKeyInput += OnPlayerInput;
            timer = new System.Timers.Timer(10);
            timer.Elapsed += BulletTimer;
            timer.AutoReset = true;
            timer.Enabled = true;
            bulletTime = new Dictionary<CSteamID, BulletTimeSetting>();
            meta = new Dictionary<CSteamID, PlayerMeta>();
            doubleJump = new Dictionary<CSteamID, int>();
            Instance = this;
            foreach (var x in Provider.clients)
            {
                UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(x);
                SetDefaults(p);
            }
        }

        private void OnPlayerInput(Player player, EPlayerKey key, bool down)
        {
            var id = player.channel.owner.playerID.steamID;
            if (!meta[id].Enabled) return;
            if (player.movement.isGrounded) return;
            if (!((key == EPlayerKey.Jump || key == EPlayerKey.HotKey1) && down)) return;
            if (!doubleJump.ContainsKey(id))
            {
                doubleJump[id] = cfg.MidAirJumps;
                // KNOWN ISSUE: rocket reloading prevents you from double jumping more than once
            }
            if (doubleJump[id] < 1) return;
            var offset = new Vector3(0, 0, 0);
            if (player.movement.fall < 0) offset.y = Math.Abs(player.movement.fall);
            if (key == EPlayerKey.Jump)
            {
                player.movement.pendingLaunchVelocity = ((new Vector3(0, 1, 0)) * cfg.DoubleJumpStrength) + offset;
                doubleJump[id]--;
            }
            else if (key == EPlayerKey.HotKey1)
            {
                var launch = (player.look.aim.forward * cfg.DoubleJumpStrength) + offset;
                player.movement.pendingLaunchVelocity = launch;
                doubleJump[id]--;
            }
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (meta.ContainsKey(player.CSteamID))
            {
                meta.Remove(player.CSteamID);
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            SetDefaults(player);
            var inp = player.Player.gameObject.AddComponent<PlayerInputListener>();
            inp.awake = true;
        }

        private void SetDefaults(UnturnedPlayer player)
        {
            if (!meta.ContainsKey(player.CSteamID))
            {
                meta[player.CSteamID] = new PlayerMeta(cfg.DefaultOn);
            }
        }

        private void UnturnedPlayerEvents_OnPlayerUpdatePosition(UnturnedPlayer player, UnityEngine.Vector3 position)
        {
            if (player.Player.movement.isGrounded)
            {
                if (bulletTime.ContainsKey(player.CSteamID))
                {
                    bulletTime.Remove(player.CSteamID);
                    SetMovement(player.Player.movement, false);
                }
                if (doubleJump.ContainsKey(player.CSteamID))
                {
                    doubleJump.Remove(player.CSteamID);
                }
            }                
        }

        private void BulletTimer(object sender, ElapsedEventArgs e)
        {
            foreach (var p in bulletTime.ToArray())
            {
                if (p.Value.BulletTimeDuration > 1)
                {
                    bulletTime[p.Key].BulletTimeDuration -= 10;
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
            if (!meta[id].Enabled) return;
            if (p == null) return;
            if (gun.isAiming && !p.isGrounded)
            {
                if (!bulletTime.ContainsKey(id))
                {
                    bulletTime[id] = new BulletTimeSetting() { Allowed = true, BulletTimeDuration = cfg.BulletTimeMS };
                }
                if (bulletTime[id].BulletTimeDuration > 0)
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
                if (bulletTime.ContainsKey(id) && cfg.OnlyAllowOncePerJump) bulletTime[id].BulletTimeDuration = 0; // this makes it so you can only do bullet time once per time in air
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

            UseableGun.OnAimingChanged_Global -= UseableGun_OnAimingChanged_Global;
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= UnturnedPlayerEvents_OnPlayerUpdatePosition;
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= BulletTimer;
            }
        }
    }
}
