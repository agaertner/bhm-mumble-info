using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using Nekres.Mumble_Info.Core.UI.Controls;

namespace Nekres.Mumble_Info.Core.UI {
    internal class MumbleView : View<MumblePresenter> {

        private BitmapFont _font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size16, ContentService.FontStyle.Regular);
        public MumbleView() {
            this.WithPresenter(new MumblePresenter(this, MumbleInfoModule.Instance.MumbleConfig.Value));
        }
        
        protected override void Build(Container buildPanel) {
            var flowContainer = new FlowPanel() {
                Parent      = buildPanel,
                CanCollapse = false,
                Width       = buildPanel.ContentRegion.Width,
                Height      = buildPanel.ContentRegion.Height,
                CanScroll = true
            };

            var pnlAvatar = new FlowPanel() {
                Parent              = flowContainer,
                Title               = "Avatar",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5, 2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = true
            };

            var lblAvatarName = new DynamicLabel(() => $"{GameService.Gw2Mumble.PlayerCharacter.Name} - {GameService.Gw2Mumble.PlayerCharacter.Race}") {
                Parent = pnlAvatar,
                Width = pnlAvatar.ContentRegion.Width,
                Height = 25,
                Font = _font
            };

            var lblAvatarProfession = new DynamicLabel(this.Presenter.GetPlayerProfession) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Icon   = MumbleInfoModule.Instance.Api.GetClassIcon((int)GameService.Gw2Mumble.PlayerCharacter.Profession, GameService.Gw2Mumble.PlayerCharacter.Specialization)
            };

            var lblAvatarPos = new DynamicLabel(this.Presenter.GetPlayerPosition) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Prefix = "Position: "
            };

            var lblAvatarDir = new DynamicLabel(this.Presenter.GetPlayerDirection) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Prefix = "Direction: "
            };

            var lblWaypoint = new DynamicLabel(this.Presenter.GetClosestWaypoint) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Icon   = MumbleInfoModule.Instance.Api.WaypointIcon
            };

            var lblPoi = new DynamicLabel(this.Presenter.GetClosestPoi) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Icon   = MumbleInfoModule.Instance.Api.PoiIcon
            };

            var pnlCamera = new FlowPanel {
                Parent              = flowContainer,
                Title               = "Camera",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = true
            };

            var lblCameraPos = new DynamicLabel(this.Presenter.GetCameraPosition) {
                Parent = pnlCamera,
                Width  = pnlCamera.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Prefix = "Position: "
            };

            var lblCameraDir = new DynamicLabel(this.Presenter.GetCameraDirection) {
                Parent = pnlCamera,
                Width  = pnlCamera.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Prefix = "Direction: "
            };

            var pnlUserInterface = new FlowPanel() {
                Parent              = flowContainer,
                Title               = "User Interface",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = true

            };

            var pnlMap = new FlowPanel() {
                Parent              = flowContainer,
                Title               = "Map",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = true
            };

            var lblMapName = new DynamicLabel(this.Presenter.GetMap) {
                Parent = pnlMap,
                Width  = pnlMap.ContentRegion.Width,
                Height = 25,
                Font   = _font
            };

            var lblSectorName = new DynamicLabel(this.Presenter.GetSector) {
                Parent = pnlMap,
                Width  = pnlMap.ContentRegion.Width,
                Height = 25,
                Font   = _font
            };

            var pnlInfo = new FlowPanel() {
                Parent              = flowContainer,
                Title               = "Gameinfo",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = true
            };

            buildPanel.ContentResized += (_, e) => {
                flowContainer.Size = new Point(e.CurrentRegion.Width, e.CurrentRegion.Height);
            };

            flowContainer.ContentResized += (_, e) => {
                pnlAvatar.Width        = e.CurrentRegion.Width;
                pnlCamera.Width        = e.CurrentRegion.Width;
                pnlUserInterface.Width = e.CurrentRegion.Width;
                pnlMap.Width           = e.CurrentRegion.Width;
                pnlInfo.Width          = e.CurrentRegion.Width;
            };
        }
    }
}
