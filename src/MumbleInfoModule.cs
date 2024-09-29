using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nekres.Mumble_Info.Core.UI;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Nekres.Mumble_Info {
    [Export(typeof(Module))]
    public class MumbleInfoModule : Module
    {

        internal static readonly Logger Logger = Logger.GetLogger(typeof(MumbleInfoModule));

        internal static MumbleInfoModule Instance;

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion


        [ImportingConstructor]
        public MumbleInfoModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { Instance = this; }

        private Texture2D _cornerIcon;
        private Texture2D _cornerIconHover;
        private Texture2D _emblem;

        private CornerIcon    _moduleIcon;
        private StandardWindow _moduleWindow;

        internal SettingEntry<MumbleConfig> MumbleConfig;

        protected override void DefineSettings(SettingCollection settings) {
            MumbleConfig = settings.DefineSetting("mumble_config", new MumbleConfig());
        }

        protected override void Initialize() {
        }

        protected override async Task LoadAsync()
        {
        }

        protected override void Update(GameTime gameTime) {

        }

        protected override void OnModuleLoaded(EventArgs e) {
            _cornerIcon      = ContentsManager.GetTexture("icon.png");
            _cornerIconHover = ContentsManager.GetTexture("hover_icon.png");
            _emblem          = ContentsManager.GetTexture("emblem.png");
            _moduleIcon      = new CornerIcon(_cornerIcon, _cornerIconHover, this.Name) {
                Priority = 4861143
            };

            _moduleIcon.Click += OnModuleIconClick;
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        public void OnModuleIconClick(object o, MouseEventArgs e) {
            if (_moduleWindow == null) {
                var windowRegion = new Rectangle(40, 26, 913, 691);
                _moduleWindow = new StandardWindow(GameService.Content.GetTexture("controls/window/502049"),
                                                  windowRegion,
                                                  new Rectangle(52, 36, 887, 605)) {
                    Parent        = GameService.Graphics.SpriteScreen,
                    Emblem        = _emblem,
                    Size          = new Point(300, 500),
                    CanResize     = true,
                    SavesPosition = true,
                    SavesSize     = true,
                    Title         = this.Name,
                    Id            = $"{nameof(MumbleInfoModule)}_MainWindow_aeabf2ad8a954af6a0d9c4b95f9682fe",
                    Left          = (GameService.Graphics.SpriteScreen.Width  - windowRegion.Width)  / 2,
                    Top           = (GameService.Graphics.SpriteScreen.Height - windowRegion.Height) / 2
                };
            }
            _moduleWindow.ToggleWindow(new MumbleView());
        }

        /// <inheritdoc />
        protected override void Unload() {
            _moduleIcon.Click        -= OnModuleIconClick;
            _moduleIcon?.Dispose();
            _moduleWindow?.Dispose();
            _cornerIcon?.Dispose();
            _cornerIconHover?.Dispose();
            _emblem?.Dispose();
            // All static members must be manually unset
            Instance = null;
        }
    }
}
