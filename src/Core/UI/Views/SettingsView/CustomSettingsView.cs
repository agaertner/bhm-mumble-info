using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;

namespace Nekres.Mumble_Info.Core.UI {
    internal class CustomSettingsView : View {

        private StandardButton _settingsBttn;

        protected override void Build(Container buildPanel) {
            _settingsBttn = new StandardButton {
                Parent = buildPanel,
                Width  = 200,
                Height = 40,
                Left   = (buildPanel.ContentRegion.Width - 200) / 2,
                Top    = buildPanel.ContentRegion.Height / 2 - 40, // Purposefully a bit higher than centered.
                Text   = "Open Mumble Info Panel"
            };

            _settingsBttn.Click += (_, _) => {
                GameService.Content.PlaySoundEffectByName("button-click");
                MumbleInfoModule.Instance.ToggleWindow();
            };

            base.Build(buildPanel);
        }
    }
}
