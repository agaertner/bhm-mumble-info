using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Nekres.Mumble_Info.Core.UI.Controls;

namespace Nekres.Mumble_Info.Core.UI.Views.MumbleView {
    internal class MumbleView : View<MumblePresenter> {

        public MumbleView() {
            this.WithPresenter(new MumblePresenter(this, GameService.Gw2Mumble));
        }

        protected override void Build(Container buildPanel) {
            var pnlAvatar = new FlowPanel() {
                Title = "Avatar",
                Width = buildPanel.ContentRegion.Width,
                Height = buildPanel.ContentRegion.Height,
                Collapsed = true
            };

            var lblAvatarName = new DynamicLabel(() => $"{this.Presenter.Model.PlayerCharacter.Name} - {this.Presenter.Model.PlayerCharacter.Race}") {
                Parent = pnlAvatar,
                Width = pnlAvatar.ContentRegion.Width,
                Height = 50
            };

            var lblAvatarProfession = new DynamicLabel(this.Presenter.GetPlayerProfession) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 50
            };

            var lblAvatarPos = new DynamicLabel(this.Presenter.GetPlayerPosition) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 50
            };

            var lblAvatarDir = new DynamicLabel(this.Presenter.GetPlayerDirection) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 50
            };

            var pnlCamera = new FlowPanel() {
                Title     = "Camera",
                Width     = buildPanel.ContentRegion.Width,
                Height    = buildPanel.ContentRegion.Height,
                Collapsed = true
            };

            var lblCameraPos = new DynamicLabel(this.Presenter.GetCameraPosition) {
                Parent = pnlCamera,
                Width  = pnlCamera.ContentRegion.Width,
                Height = 50
            };

            var lblCameraDir = new DynamicLabel(this.Presenter.GetCameraDirection) {
                Parent = pnlCamera,
                Width  = pnlCamera.ContentRegion.Width,
                Height = 50
            };

            var pnlUserInterface = new FlowPanel() {
                Title     = "User Interface",
                Width     = buildPanel.ContentRegion.Width,
                Height    = buildPanel.ContentRegion.Height,
                Collapsed = true

            };

            var pnlMap = new FlowPanel() {
                Title     = "Map",
                Width     = buildPanel.ContentRegion.Width,
                Height    = buildPanel.ContentRegion.Height,
                Collapsed = true
            };

            var lblMapName = new DynamicLabel(this.Presenter.GetMap) {
                Parent = pnlMap,
                Width = pnlMap.ContentRegion.Width,
                Height = 50
            };

            var pnlInfo = new FlowPanel() {
                Title     = "Gameinfo",
                Width     = buildPanel.ContentRegion.Width,
                Height    = buildPanel.ContentRegion.Height,
                Collapsed = true
            };
        }
    }
}
