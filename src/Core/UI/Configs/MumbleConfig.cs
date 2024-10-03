using Blish_HUD.Input;
using Newtonsoft.Json;

namespace Nekres.Mumble_Info.Core.UI {
    internal class MumbleConfig : ConfigBase {
        private bool _swapYZ;
        [JsonProperty("swap_yz_axis")]
        public bool SwapYZ { 
            get =>  _swapYZ;
            set {
                if (SetProperty(ref _swapYZ, value)) {
                    SaveConfig(MumbleInfoModule.Instance.MumbleConfig);
                }
            }
        }

        private KeyBinding _shortcut;
        [JsonProperty("shortcut")]
        public KeyBinding Shortcut {
            get => _shortcut;
            set {
                if (SetProperty(ref _shortcut, value)) {
                    SaveConfig(MumbleInfoModule.Instance.MumbleConfig);
                }
            }
        }

        protected override void BindingChanged() {
            SaveConfig(MumbleInfoModule.Instance.MumbleConfig);
        }
    }
}
