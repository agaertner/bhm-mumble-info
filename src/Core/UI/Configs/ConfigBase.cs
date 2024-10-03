using Blish_HUD.Input;
using Blish_HUD.Settings;
using System;

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
            if (Equals(property, newValue)) {
                return false;
            }
            property = newValue;
            return true;
        }

        protected bool SetProperty(ref KeyBinding oldBinding, KeyBinding newBinding) {
            if (oldBinding == newBinding) {
                return false;
            }
            if (oldBinding != null) {
                oldBinding.Enabled        =  false;
                oldBinding.BindingChanged -= OnBindingChanged;
            }
            oldBinding                = newBinding ?? new KeyBinding();
            oldBinding.BindingChanged +=  OnBindingChanged;
            oldBinding.Enabled        =   true;
            return true;
        }

        protected virtual void BindingChanged() {
            /* NOOP */
        }

        private void OnBindingChanged(object sender, EventArgs e) {
            BindingChanged();
        }
    }
}
