using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Extended;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using Nekres.Mumble_Info.Core.UI.Controls;
using System.Linq;

namespace Nekres.Mumble_Info.Core.UI {
    internal class MumbleView : View<MumblePresenter> {

        private BitmapFont _font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size16, ContentService.FontStyle.Regular);

        private const int SCROLLBAR_WIDTH = 13;

        private readonly Color _grey       = new Color(168, 168, 168);
        private readonly Color _orange     = new Color(252, 168, 0);
        private readonly Color _red        = new Color(252, 84,  84);
        private readonly Color _softRed    = new Color(250, 148, 148);
        private readonly Color _green      = new Color(0,   168, 0);
        private readonly Color _lemonGreen = new Color(84,  252, 84);
        private readonly Color _cyan       = new Color(84,  252, 252);
        private readonly Color _blue       = new Color(0,   168, 252);
        private readonly Color _brown      = new Color(158, 81,  44);
        private readonly Color _yellow     = new Color(252, 252, 84);
        private readonly Color _softYellow = new Color(250, 250, 148);

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
                Parent              = flowContainer,
                CanCollapse         = false,
                Width               = flowContainer.ContentRegion.Width - SCROLLBAR_WIDTH,
                HeightSizingMode    = SizingMode.AutoSize,
                FlowDirection       = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(8, 10),
                ShowBorder          = true
            };

            var cbSwapYz = new Checkbox {
                Parent  = pnlConfigs,
                Text    = "Swap YZ",
                BasicTooltipText = "Swap values of Y and Z for 3D coordinates.",
                Height = 25,
                Checked = this.Presenter.Model.SwapYZ
            };

            cbSwapYz.CheckedChanged += (_, e) => {
                this.Presenter.Model.SwapYZ = !e.Checked;
            };

            var kbShortcut = new KeybindingAssigner(this.Presenter.Model.Shortcut) {
                Parent           = pnlConfigs,
                KeyBindingName   = "Mumble Info Panel",
                BasicTooltipText = "Open or close the Mumble Info panel."
            };

            var pnlAvatar = new FlowPanel {
                Parent              = flowContainer,
                Title               = "Avatar",
                Width               = flowContainer.ContentRegion.Width - SCROLLBAR_WIDTH,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false
            };

            var lblAvatarName = new DynamicLabel(this.Presenter.GetRace) {
                Parent         = pnlAvatar,
                Width          = pnlAvatar.ContentRegion.Width,
                Height         = 25,
                Font           = _font,
                StrokeTextData = true
            };

            var lblAvatarProfession = new DynamicLabel(this.Presenter.GetPlayerProfession) {
                Parent         = pnlAvatar,
                Width          = pnlAvatar.ContentRegion.Width,
                Height         = 25,
                Font           = _font,
                Icon           = MumbleInfoModule.Instance.Api.GetClassIcon((int)GameService.Gw2Mumble.PlayerCharacter.Profession, GameService.Gw2Mumble.PlayerCharacter.Specialization),
                StrokeTextData = true
            };

            var lblAvatarPos = new DynamicLabel(this.Presenter.GetPlayerPosition) {
                Parent        = pnlAvatar,
                Width         = pnlAvatar.ContentRegion.Width,
                Height        = 25,
                Font          = _font,
                Text          = "Position: ",
                StrokeText    = true,
                TextColor     = _green,
                TextDataColor = _lemonGreen
            };
            lblAvatarPos.MouseEntered += OnLabelEnter;
            lblAvatarPos.MouseLeft    += (o, _) => OnLabelLeft(o, _green, _lemonGreen);
            lblAvatarPos.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetPlayerPosition(true));
            };

            var lblAvatarDir = new DynamicLabel(this.Presenter.GetPlayerDirection) {
                Parent        = pnlAvatar,
                Width         = pnlAvatar.ContentRegion.Width,
                Height        = 25,
                Font          = _font,
                Text          = "Direction: ",
                StrokeText    = true,
                TextColor     = _green,
                TextDataColor = _lemonGreen
            };
            lblAvatarDir.MouseEntered += OnLabelEnter;
            lblAvatarDir.MouseLeft    += (o, _) => OnLabelLeft(o, _green, _lemonGreen);
            lblAvatarDir.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetPlayerDirection(true));
            };

            var lblWaypoint = new DynamicLabel(this.Presenter.GetClosestWaypoint) {
                Parent         = pnlAvatar,
                Width          = pnlAvatar.ContentRegion.Width,
                Height         = 25,
                Font           = _font,
                Icon           = MumbleInfoModule.Instance.Api.WaypointIcon,
                StrokeTextData = true
            };
            lblWaypoint.MouseEntered += OnLabelEnter;
            lblWaypoint.MouseLeft    += (o, _) => OnLabelLeft(o, Color.White);
            lblWaypoint.Click += async (_, _) => {
                if (MumbleInfoModule.Instance.Api.ClosestWaypoint == null) {
                    return;
                }
                await this.Presenter.CopyToClipboard(MumbleInfoModule.Instance.Api.ClosestWaypoint.ChatLink);
            };

            var lblPoi = new DynamicLabel(this.Presenter.GetClosestPoi) {
                Parent         = pnlAvatar,
                Width          = pnlAvatar.ContentRegion.Width,
                Height         = 25,
                Font           = _font,
                Icon           = MumbleInfoModule.Instance.Api.PoiIcon,
                StrokeTextData = true
            };
            lblPoi.MouseEntered += OnLabelEnter;
            lblPoi.MouseLeft    += (o, _) => OnLabelLeft(o, Color.White);
            lblPoi.Click += async (_, _) => {
                if (MumbleInfoModule.Instance.Api.ClosestPoi == null) {
                    return;
                }
                await this.Presenter.CopyToClipboard(MumbleInfoModule.Instance.Api.ClosestPoi.ChatLink);
            };

            var pnlCamera = new FlowPanel {
                Parent              = flowContainer,
                Title               = "Camera",
                Width               = flowContainer.ContentRegion.Width - SCROLLBAR_WIDTH,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false
            };
            var lblCameraPos = new DynamicLabel(this.Presenter.GetCameraPosition) {
                Parent        = pnlCamera,
                Width         = pnlCamera.ContentRegion.Width,
                Height        = 25,
                Font          = _font,
                Text          = "Position: ",
                StrokeText    = true,
                TextColor     = _red,
                TextDataColor = _softRed
            };
            lblCameraPos.MouseEntered += OnLabelEnter;
            lblCameraPos.MouseLeft    += (o, _) => OnLabelLeft(o, _red, _softRed);
            lblCameraPos.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetCameraPosition(true));
            };

            var lblCameraDir = new DynamicLabel(this.Presenter.GetCameraDirection) {
                Parent        = pnlCamera,
                Width         = pnlCamera.ContentRegion.Width,
                Height        = 25,
                Font          = _font,
                Text          = "Direction: ",
                StrokeText    = true,
                TextColor     = _red,
                TextDataColor = _softRed
            };
            lblCameraDir.MouseEntered += OnLabelEnter;
            lblCameraDir.MouseLeft    += (o, _) => OnLabelLeft(o, _red, _softRed);
            lblCameraDir.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetCameraDirection(true));
            };

            var pnlUserInterface = new FlowPanel {
                Parent              = flowContainer,
                Title               = "User Interface",
                Width               = flowContainer.ContentRegion.Width - SCROLLBAR_WIDTH,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false
            };

            var lblUISize = new DynamicLabel(this.Presenter.GetUiSize) {
                Parent     = pnlUserInterface,
                Width      = pnlUserInterface.ContentRegion.Width,
                Height     = 25,
                Font       = _font,
                Text       = "UI Size: ",
                StrokeText = true
            };

            var lblCompass = new DynamicLabel(this.Presenter.GetCompassBounds) {
                Parent     = pnlUserInterface,
                Width      = pnlUserInterface.ContentRegion.Width,
                Height     = 25,
                Font       = _font,
                Text       = "Compass: ",
                StrokeText = true
            };
            lblCompass.MouseEntered += OnLabelEnter;
            lblCompass.MouseLeft    += (o, _) => OnLabelLeft(o, Color.White);
            lblCompass.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetCompassBounds());
            };

            var pnlMap = new FlowPanel {
                Parent              = flowContainer,
                Title               = "Map",
                Width               = flowContainer.ContentRegion.Width - SCROLLBAR_WIDTH,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false
            };

            var lblContinentName = new DynamicLabel(this.Presenter.GetContinent) {
                Parent         = pnlMap,
                Width          = pnlMap.ContentRegion.Width,
                Height         = 25,
                Font           = _font,
                TextDataColor  = _softYellow,
                StrokeTextData = true
            };

            var lblMapName = new DynamicLabel(this.Presenter.GetMap) {
                Parent         = pnlMap,
                Width          = pnlMap.ContentRegion.Width,
                Height         = 25,
                Font           = _font,
                StrokeText     = true,
                TextDataColor  = _yellow,
                StrokeTextData = true
            };
            lblMapName.MouseEntered += OnLabelEnter;
            lblMapName.MouseLeft    += (o, _) => OnLabelLeft(o, Color.White, _yellow);
            lblMapName.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetMap());
            };

            var lblSectorName = new DynamicLabel(this.Presenter.GetSector) {
                Parent         = pnlMap,
                Width          = pnlMap.ContentRegion.Width,
                Height         = 25,
                Font           = _font,
                TextDataColor  = _orange,
                StrokeTextData = true
            };

            var lblMapType = new DynamicLabel(this.Presenter.GetMapType) {
                Parent        = pnlMap,
                Width         = pnlMap.ContentRegion.Width,
                Height        = 25,
                Font          = _font,
                Text          = "Type: ",
                StrokeText    = true
            };

            var lblMapPos = new DynamicLabel(this.Presenter.GetMapPosition) {
                Parent        = pnlMap,
                Width         = pnlMap.ContentRegion.Width,
                Height        = 25,
                Font          = _font,
                TextColor     = _red,
                Text          = "Position: ",
                StrokeText    = true,
                TextDataColor = _softRed
            };
            lblMapPos.MouseEntered += OnLabelEnter;
            lblMapPos.MouseLeft    += (o,_) => OnLabelLeft(o, _red, _softRed);
            lblMapPos.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetMapPosition(true));
            };

            var lblMapHash = new DynamicLabel(this.Presenter.GetMapHash) {
                Parent     = pnlMap,
                Width      = pnlMap.ContentRegion.Width,
                Height     = 25,
                Font       = _font,
                Text       = "Hash: ",
                StrokeText = true
            };
            lblMapHash.MouseEntered        += OnLabelEnter;
            lblMapHash.MouseLeft += (o, _) => OnLabelLeft(o, Color.White);
            lblMapHash.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetMapHash(true));
            };

            var pnlInfo = new FlowPanel {
                Parent              = flowContainer,
                Title               = "Info",
                Width               = flowContainer.ContentRegion.Width - SCROLLBAR_WIDTH,
                HeightSizingMode    = SizingMode.AutoSize,
                ControlPadding      = new Vector2(5,  2),
                OuterControlPadding = new Vector2(10, 2),
                Collapsed           = false
            };

            var lblBlishVersion = new Label {
                Parent    = pnlInfo,
                Width     = pnlInfo.ContentRegion.Width,
                Height    = 25,
                Font      = _font,
                Text      = BlishUtil.GetVersion(),
                TextColor = _grey
            };

            var lblMumbleVersion = new Label {
                Parent    = pnlInfo,
                Width     = pnlInfo.ContentRegion.Width,
                Height    = 25,
                Font      = _font,
                Text      = $"Mumble Link v{GameService.Gw2Mumble.Info.Version}",
                TextColor = _grey
            };

            var lblProcessId = new DynamicLabel(this.Presenter.GetProcessId) {
                Parent     = pnlInfo,
                Width      = pnlInfo.ContentRegion.Width,
                Height     = 25,
                Font       = _font,
                Text       = "PID: ",
                StrokeText = true
            };
            lblProcessId.MouseEntered += OnLabelEnter;
            lblProcessId.MouseLeft    += (o, _) => OnLabelLeft(o, Color.White);
            lblProcessId.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetProcessId());
            };

            var lblServerAddress = new DynamicLabel(this.Presenter.GetServerAddress) {
                Parent        = pnlInfo,
                Width         = pnlInfo.ContentRegion.Width,
                Height        = 25,
                Font          = _font,
                Text          = "Server Addr.: ",
                StrokeText    = true,
                TextColor     = _blue,
                TextDataColor = _cyan
            };
            lblServerAddress.MouseEntered += OnLabelEnter;
            lblServerAddress.MouseLeft    += (o, _) => OnLabelLeft(o, _blue, _cyan);
            lblServerAddress.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetServerAddress());
            };

            var lblShardId = new DynamicLabel(this.Presenter.GetShardId) {
                Parent     = pnlInfo,
                Width      = pnlInfo.ContentRegion.Width,
                Height     = 25,
                Font       = _font,
                Text       = "Shard ID: ",
                StrokeText = true
            };
            lblShardId.MouseEntered += OnLabelEnter;
            lblShardId.MouseLeft    += (o, _) => OnLabelLeft(o, Color.White);
            lblShardId.Click += async (_, _) => {
                await this.Presenter.CopyToClipboard(this.Presenter.GetShardId());
            };

            buildPanel.ContentResized += (_, e) => {
                flowContainer.Size = new Point(e.CurrentRegion.Width, e.CurrentRegion.Height);
            };

            flowContainer.ContentResized += (_, e) => {
                pnlConfigs.Width       = e.CurrentRegion.Width - SCROLLBAR_WIDTH;
                pnlAvatar.Width        = e.CurrentRegion.Width - SCROLLBAR_WIDTH;
                pnlCamera.Width        = e.CurrentRegion.Width - SCROLLBAR_WIDTH;
                pnlUserInterface.Width = e.CurrentRegion.Width - SCROLLBAR_WIDTH;
                pnlMap.Width           = e.CurrentRegion.Width - SCROLLBAR_WIDTH;
                pnlInfo.Width          = e.CurrentRegion.Width - SCROLLBAR_WIDTH;

                // Fix: labels cutting off when window is opened smaller than their text requires width.
                var allDataRows = flowContainer.Children.Skip(1).SelectMany(x => ((Container)x).Children);
                foreach (var row in allDataRows) {
                    row.Width = row.Parent.ContentRegion.Width;
                }
            };
        }

        private void OnLabelEnter(object sender, MouseEventArgs e) {
            if (sender is Label lbl) {
                lbl.TextColor = Color.LightBlue;
                if (lbl is DynamicLabel dynLbl) {
                    dynLbl.TextDataColor = Color.LightBlue;
                }
            }
        }

        private void OnLabelLeft(object sender, Color textColor, Color textDataColor = default) {
            if (sender is Label lbl) {
                lbl.TextColor = textColor;
                if (lbl is DynamicLabel dynLbl) {
                    dynLbl.TextDataColor = textDataColor == default ? textColor : textDataColor;
                }
            }
        }
    }
}
