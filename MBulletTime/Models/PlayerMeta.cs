using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBulletTime.Models
{
    public class PlayerMeta
    {
        public bool Enabled;
        public EPlayerKey DashKeyBind;
        public CoolDowns Cooldown;

        public PlayerMeta(bool enabled, EPlayerKey keybind = EPlayerKey.HotKey1)
        {
            Enabled = enabled;
            DashKeyBind = keybind;
            Cooldown = new CoolDowns();
        }
    }

    public class CoolDowns
    {
        public CoolDowns(int dj = 0, int dash = 0)
        {
            DoubleJump = dj;
            Dash = dash;
        }
        public int DoubleJump;
        public int Dash;
    }
}
