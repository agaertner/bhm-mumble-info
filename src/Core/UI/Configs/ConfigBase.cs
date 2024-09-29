using Blish_HUD.Settings;

namespace Nekres.Mumble_Info.Core.UI {
    public abstract class ConfigBase {
        protected void SaveConfig<T>(SettingEntry<T> setting) where T : ConfigBase {
            if (setting?.IsNull ?? true) {
                return;
            }
            /* unset value first otherwise reassigning the same reference would
             not be recognized as a property change and not invoke a save. */
            setting.Value = null;
            setting.Value = this as T; 
        }

        protected bool SetProperty<T>(ref T property, T newValue) {
            if (object.Equals(property, newValue)) {
                return false;
            }
            property = newValue;
            return true;
        }
    }
}
