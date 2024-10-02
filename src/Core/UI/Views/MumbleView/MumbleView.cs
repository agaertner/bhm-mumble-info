using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Extended;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using Nekres.Mumble_Info.Core.UI.Controls;

namespace Nekres.Mumble_Info.Core.UI {
    internal class MumbleView : View<MumblePresenter> {

        private BitmapFont _font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size16, ContentService.FontStyle.Regular);

        private readonly Color _lemonGreen = new Color(84,  252, 84);
        private readonly Color _cyan       = new Color(84,  252, 252);
        private readonly Color _red        = new Color(252, 84,  84);

        public MumbleView() {
            this.WithPresenter(new MumblePresenter(this, MumbleInfoModule.Instance.MumbleConfig.Value));
        }
        
        protected override void Build(Container buildPanel) {
            var flowContainer = new FlowPanel {
                Parent      = buildPanel,
                CanCollapse = false,
                Width       = buildPanel.ContentRegion.Width,
                Height      = buildPanel.ContentRegion.Height,
                CanScroll = true
            };

            var pnlConfigs = new FlowPanel {
                Parent           = flowContainer,
                CanCollapse      = false,
                Width            = flowContainer.ContentRegion.Width,
                HeightSizingMode = SizingMode.AutoSize,
                FlowDirection    = ControlFlowDirection.RightToLeft,
                OuterControlPadding = new Vector2(5,5)
            };

            var cbSwapYz = new Checkbox {
                Parent  = pnlConfigs,
                Text    = "Swap YZ",
                Checked = this.Presenter.Model.SwapYZ
            };

            cbSwapYz.CheckedChanged += (_, e) => {
                this.Presenter.Model.SwapYZ = !e.Checked;
            };

            var pnlAvatar = new FlowPanel {
                Parent              = flowContainer,
                Title               = "Avatar",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false
            };

            var lblAvatarName = new DynamicLabel(this.Presenter.GetRace) {
                Parent = pnlAvatar,
                Width = pnlAvatar.ContentRegion.Width,
                Height = 25,
                Font = _font,
                StrokeText = true
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
                Prefix = "Position: ",
                TextColor = _lemonGreen
            };
            lblAvatarPos.MouseEntered += OnLabelEnter;
            lblAvatarPos.MouseLeft    += (o, _) => OnLabelLeft(o, _lemonGreen);
            lblAvatarPos.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetPlayerPosition(true));
            };

            var lblAvatarDir = new DynamicLabel(this.Presenter.GetPlayerDirection) {
                Parent    = pnlAvatar,
                Width     = pnlAvatar.ContentRegion.Width,
                Height    = 25,
                Font      = _font,
                Prefix    = "Direction: ",
                TextColor = _lemonGreen
            };
            lblAvatarDir.MouseEntered += OnLabelEnter;
            lblAvatarDir.MouseLeft    += (o, _) => OnLabelLeft(o, _lemonGreen);
            lblAvatarDir.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetPlayerDirection(true));
            };

            var lblWaypoint = new DynamicLabel(this.Presenter.GetClosestWaypoint) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Icon   = MumbleInfoModule.Instance.Api.WaypointIcon
            };
            lblWaypoint.MouseEntered += OnLabelEnter;
            lblWaypoint.MouseLeft    += (o, _) => OnLabelLeft(o, Color.White);
            lblWaypoint.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(MumbleInfoModule.Instance.Api.ClosestWaypoint.ChatLink);
            };

            var lblPoi = new DynamicLabel(this.Presenter.GetClosestPoi) {
                Parent = pnlAvatar,
                Width  = pnlAvatar.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Icon   = MumbleInfoModule.Instance.Api.PoiIcon
            };
            lblPoi.MouseEntered += OnLabelEnter;
            lblPoi.MouseLeft    += (o, _) => OnLabelLeft(o, Color.White);
            lblPoi.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(MumbleInfoModule.Instance.Api.ClosestPoi.ChatLink);
            };

            var pnlCamera = new FlowPanel {
                Parent              = flowContainer,
                Title               = "Camera",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false
            };
            var lblCameraPos = new DynamicLabel(this.Presenter.GetCameraPosition) {
                Parent    = pnlCamera,
                Width     = pnlCamera.ContentRegion.Width,
                Height    = 25,
                Font      = _font,
                Prefix    = "Position: ",
                TextColor = _red
            };
            lblCameraPos.MouseEntered += OnLabelEnter;
            lblCameraPos.MouseLeft    += (o, _) => OnLabelLeft(o, _red);
            lblCameraPos.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetCameraPosition(true));
            };

            var lblCameraDir = new DynamicLabel(this.Presenter.GetCameraDirection) {
                Parent    = pnlCamera,
                Width     = pnlCamera.ContentRegion.Width,
                Height    = 25,
                Font      = _font,
                Prefix    = "Direction: ",
                TextColor = _red
            };
            lblCameraDir.MouseEntered += OnLabelEnter;
            lblCameraDir.MouseLeft    += (o, _) => OnLabelLeft(o, _red);
            lblCameraDir.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetCameraDirection(true));
            };

            var pnlUserInterface = new FlowPanel {
                Parent              = flowContainer,
                Title               = "User Interface",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false

            };

            var lblUISize = new DynamicLabel(this.Presenter.GetUiSize) {
                Parent = pnlUserInterface,
                Width  = pnlUserInterface.ContentRegion.Width,
                Height = 25,
                Font   = _font
            };

            var lblCompass = new DynamicLabel(this.Presenter.GetCompassBounds) {
                Parent = pnlUserInterface,
                Width  = pnlUserInterface.ContentRegion.Width,
                Height = 25,
                Font   = _font
            };

            var pnlMap = new FlowPanel {
                Parent              = flowContainer,
                Title               = "Map",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false
            };

            var lblContinentName = new DynamicLabel(this.Presenter.GetContinent) {
                Parent = pnlMap,
                Width  = pnlMap.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                TextColor = _cyan
            };

            var lblMapName = new DynamicLabel(this.Presenter.GetMap) {
                Parent     = pnlMap,
                Width      = pnlMap.ContentRegion.Width,
                Height     = 25,
                Font       = _font,
                StrokeText = true,
                TextColor  = _cyan
            };

            var lblSectorName = new DynamicLabel(this.Presenter.GetSector) {
                Parent    = pnlMap,
                Width     = pnlMap.ContentRegion.Width,
                Height    = 25,
                Font      = _font,
                TextColor = _cyan
            };

            var lblMapType = new DynamicLabel(this.Presenter.GetMapType) {
                Parent = pnlMap,
                Width  = pnlMap.ContentRegion.Width,
                Height = 25,
                Font   = _font
            };

            var lblMapPos = new DynamicLabel(this.Presenter.GetMapPosition) {
                Parent = pnlMap,
                Width  = pnlMap.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                TextColor = _red
            };
            lblMapPos.MouseEntered += OnLabelEnter;
            lblMapPos.MouseLeft    += (o,_) => OnLabelLeft(o, _red);
            lblMapPos.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetMapPosition(true));
            };

            var lblMapHash = new DynamicLabel(this.Presenter.GetMapHash) {
                Parent = pnlMap,
                Width  = pnlMap.ContentRegion.Width,
                Height = 25,
                Font   = _font
            };
            lblMapHash.MouseEntered        += OnLabelEnter;
            lblMapHash.MouseLeft += (o, _) => OnLabelLeft(o, Color.White);
            lblMapHash.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetMapHash(true));
            };

            var pnlInfo = new FlowPanel {
                Parent              = flowContainer,
                Title               = "Info",
                Width               = flowContainer.ContentRegion.Width,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false
            };

            var lblBlishVersion = new Label {
                Parent = pnlInfo,
                Width  = pnlInfo.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Text   = BlishUtil.GetVersion()
            };

            var lblMumbleVersion = new Label {
                Parent = pnlInfo,
                Width  = pnlInfo.ContentRegion.Width,
                Height = 25,
                Font   = _font,
                Text = $"Mumble Link v{GameService.Gw2Mumble.Info.Version}"
            };

            var lblProcessId = new DynamicLabel(this.Presenter.GetProcessId) {
                Parent = pnlInfo,
                Width  = pnlInfo.ContentRegion.Width,
                Height = 25,
                Font   = _font
            };

            var lblServerAddress = new DynamicLabel(this.Presenter.GetServerAddress) {
                Parent = pnlInfo,
                Width  = pnlInfo.ContentRegion.Width,
                Height = 25,
                Font   = _font
            };

            var lblShardId = new DynamicLabel(this.Presenter.GetShardId) {
                Parent = pnlInfo,
                Width  = pnlInfo.ContentRegion.Width,
                Height = 25,
                Font   = _font
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

        private void OnLabelEnter(object sender, MouseEventArgs e) {
            var ctrl = (Label)sender;
            ctrl.TextColor = Color.LightBlue;
        }

        private void OnLabelLeft(object sender, Color color) {
            var ctrl = (Label)sender;
            ctrl.TextColor = color;
        }
    }
}
