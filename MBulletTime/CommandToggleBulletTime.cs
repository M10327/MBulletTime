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

        public string Name => "Parkour";

        public string Help => "manage parkour settings";

        public string Syntax => "/parkour <toggle/dash>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "bullettime" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var id = (caller as UnturnedPlayer).CSteamID;
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }
            if (command[0].ToLower() == "toggle")
            {
                if (MBulletTime.meta[id].Enabled)
                {
                    MBulletTime.meta[id].Enabled = false;
                    UnturnedChat.Say(caller, "Turned parkour features off");
                }
                else
                {
                    MBulletTime.meta[id].Enabled = true;
                    UnturnedChat.Say(caller, "Turned parkour features on");
                }

            }
            if (command[0].ToLower() == "dash")
            {
                if (command.Length < 2)
                {
                    UnturnedChat.Say(caller, "/parkour dash <1-5> to set the plugin hotkey");
                    return;
                }
                if (!int.TryParse(command[1], out int key) || key < 1 || key > 5){
                    UnturnedChat.Say(caller, "/parkour dash <1-5> to set the plugin hotkey");
                    return;
                }
                key += 9;
                MBulletTime.meta[id].DashKeyBind = (EPlayerKey)key;
                UnturnedChat.Say(caller, $"Set your dash hotkey to {MBulletTime.meta[id].DashKeyBind}");
            }
        }
    }
}
