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

        public PlayerMeta(bool enabled, EPlayerKey keybind = EPlayerKey.HotKey1)
        {
            Enabled = enabled;
            DashKeyBind = keybind;
        }
    }
}
