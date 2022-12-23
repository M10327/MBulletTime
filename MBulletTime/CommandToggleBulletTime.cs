using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBulletTime
{
    public class CommandToggleBulletTime : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "BulletTime";

        public string Help => "Toggled whether you can activate bullet time";

        public string Syntax => "none";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "bullettime" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var id = (caller as UnturnedPlayer).CSteamID;
            if (MBulletTime.meta[id].Enabled)
            {
                MBulletTime.meta[id].Enabled = false;
                UnturnedChat.Say(caller, "Turned bullet time off");
            }
            else
            {
                MBulletTime.meta[id].Enabled = true;
                UnturnedChat.Say(caller, "Turned bullet time on");
            }
        }
    }
}
