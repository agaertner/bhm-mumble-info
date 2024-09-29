using Blish_HUD;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
       
        #region Settings

        private  SettingEntry<KeyBinding> _toggleInfoBinding;
        internal SettingEntry<bool>       SwapYZAxes;

        #endregion

        protected override void DefineSettings(SettingCollection settings) {
            _toggleInfoBinding = settings.DefineSetting("ToggleInfoBinding", new KeyBinding(Keys.OemPlus),
                                                        () => "Toggle display",
                                                        () => "Toggles the display of data.");


            SwapYZAxes = settings.DefineSetting("SwapYZAxes", true,
                                                () => "Swap YZ Axes",
                                                () => "Swaps the values of the Y and Z axes if enabled.");
            //TODO: Create UI configs in class
        }

        protected override void Initialize() {
        }

        protected override async Task LoadAsync()
        {
        }

        protected override void Update(GameTime gameTime) {

        }

        protected override void OnModuleLoaded(EventArgs e) {
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        /// <inheritdoc />
        protected override void Unload() {
            // All static members must be manually unset
            Instance = null;
        }
    }
}
