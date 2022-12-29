using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBulletTime
{
    public class Config : IRocketPluginConfiguration
    {
        public int GlideMS;
        public float Gravity;
        public float Speed;
        public float DefaultGravity;
        public float DefaultSpeed;
        public bool GlideOncePerJump;
        public ushort GlideEffect;
        public bool DefaultOn;
        public int DoubleJumps;
        public float DoubleJumpStrength;
        public ushort DoubleJumpEffect;
        public int Dashes;
        public float DashStrength;
        public ushort DashEffect;
        public int DashDefaultKey;
        public float DashVerticalBoost;
        public bool DashAllowFromGround;
        public void LoadDefaults()
        {
            GlideMS = 2000;
            Gravity = 0.2f;
            Speed = 0.2f;
            DefaultGravity = 1.1f;
            DefaultSpeed = 1.1f;
            GlideOncePerJump = false;
            DefaultOn = false;
            DoubleJumps = 2;
            DoubleJumpStrength = 13;
            DoubleJumpEffect = 1977;
            Dashes = 1;
            DashStrength = 13;
            DashEffect = 1978;
            GlideEffect = 0;
            DashDefaultKey = 3;
            DashVerticalBoost = 6;
            DashAllowFromGround = false;
        }
    }
}
