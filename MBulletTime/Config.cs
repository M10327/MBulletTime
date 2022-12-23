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
        public int BulletTimeMS;
        public float Gravity;
        public float Speed;
        public float DefaultGravity;
        public float DefaultSpeed;
        public bool OnlyAllowOncePerJump;
        public void LoadDefaults()
        {
            BulletTimeMS = 3000;
            Gravity = 0.1f;
            Speed = 0.1f;
            DefaultGravity = 1.0f;
            DefaultSpeed = 1.0f;
            OnlyAllowOncePerJump = true;
        }
    }
}
