using Blish_HUD;
using Blish_HUD.Extended;
using Blish_HUD.Graphics.UI;
using System;
using System.Threading.Tasks;

namespace Nekres.Mumble_Info.Core.UI {
    internal class MumblePresenter : Presenter<MumbleView, MumbleConfig> {

        private const string _decimalFormat = "0.###";

        private       string _eliteSpecializationName = string.Empty;
        private       string _mapName                 = string.Empty;

        public MumblePresenter(MumbleView view, MumbleConfig model) : base(view, model) {
            GameService.Gw2Mumble.CurrentMap.MapChanged                 += OnMapChanged;
            GameService.Gw2Mumble.PlayerCharacter.SpecializationChanged += OnSpecializationChanged;
        }

        protected override async Task<bool> Load(IProgress<string> progress) {
            progress.Report("Loading…");
            await GetSpecialization(GameService.Gw2Mumble.PlayerCharacter.Specialization);
            await GetMap(GameService.Gw2Mumble.CurrentMap.Id);
            progress.Report(null);
            return true;
        }

        private async void OnSpecializationChanged(object sender, ValueEventArgs<int> e) {
            await GetSpecialization(e.Value);
        }

        private async Task GetSpecialization(int specId) {
            if (!MumbleInfoModule.Instance.Gw2ApiManager.IsApiAvailable()) {
                _eliteSpecializationName = string.Empty;
                return;
            }

            var spec = await TaskUtil.TryAsync(() => MumbleInfoModule.Instance.Gw2ApiManager.Gw2ApiClient.V2.Specializations.GetAsync(specId), MumbleInfoModule.Logger);
            _eliteSpecializationName = spec is { Elite: true } ? spec.Name : string.Empty;
        }

        private async void OnMapChanged(object sender, ValueEventArgs<int> e) {
            await GetMap(e.Value);
        }

        private async Task GetMap(int mapId) {
            if (!MumbleInfoModule.Instance.Gw2ApiManager.IsApiAvailable()) {
                _mapName = string.Empty;
                return;
            }

            var map = await TaskUtil.TryAsync(() => MumbleInfoModule.Instance.Gw2ApiManager.Gw2ApiClient.V2.Maps.GetAsync(mapId), MumbleInfoModule.Logger);
            _mapName = map?.Name ?? string.Empty;
        }

        public string GetPlayerProfession() {
            return string.IsNullOrEmpty(_eliteSpecializationName) ? $"{GameService.Gw2Mumble.PlayerCharacter.Profession}" : $"{GameService.Gw2Mumble.PlayerCharacter.Profession} ({_eliteSpecializationName})";
        }

        public string GetMap() {
            return string.IsNullOrEmpty(_mapName) ? $"{GameService.Gw2Mumble.CurrentMap.Id}" : $"{_mapName} ({GameService.Gw2Mumble.CurrentMap.Id})";
        }

        public string GetPlayerPosition() {
            var pos = string.Format(this.Model.SwapYZ ? "{0} / {2} / {1}" : "{0} / {1} / {2}",
                                 GameService.Gw2Mumble.PlayerCharacter.Position.X.ToString(_decimalFormat), 
                                 GameService.Gw2Mumble.PlayerCharacter.Position.Y.ToString(_decimalFormat),
                                 GameService.Gw2Mumble.PlayerCharacter.Position.Z.ToString(_decimalFormat));

            return $"Position: {pos}";
        }

        public string GetPlayerDirection() {
            var dir = string.Format(this.Model.SwapYZ ? "{0} / {2} / {1}" : "{0} / {1} / {2}",
                                    GameService.Gw2Mumble.PlayerCharacter.Forward.X.ToString(_decimalFormat),
                                    GameService.Gw2Mumble.PlayerCharacter.Forward.Y.ToString(_decimalFormat),
                                    GameService.Gw2Mumble.PlayerCharacter.Forward.Z.ToString(_decimalFormat));
            return $"Direction: {dir} ({DirectionUtil.IsFacing(GameService.Gw2Mumble.RawClient.AvatarFront).ToString().SplitCamelCase()})";
        }

        public string GetCameraPosition() {
            var pos = string.Format(this.Model.SwapYZ ? "{0} / {2} / {1}" : "{0} / {1} / {2}",
                                    GameService.Gw2Mumble.PlayerCamera.Position.X.ToString(_decimalFormat),
                                    GameService.Gw2Mumble.PlayerCamera.Position.Y.ToString(_decimalFormat),
                                    GameService.Gw2Mumble.PlayerCamera.Position.Z.ToString(_decimalFormat));
            return $"Position: {pos}";
        }

        public string GetCameraDirection() {
            var dir = string.Format(this.Model.SwapYZ ? "{0} / {2} / {1}" : "{0} / {1} / {2}",
                                    GameService.Gw2Mumble.PlayerCamera.Forward.X.ToString(_decimalFormat),
                                    GameService.Gw2Mumble.PlayerCamera.Forward.Y.ToString(_decimalFormat),
                                    GameService.Gw2Mumble.PlayerCamera.Forward.Z.ToString(_decimalFormat));
            return $"Direction: {dir} ({DirectionUtil.IsFacing(GameService.Gw2Mumble.RawClient.CameraFront).ToString().SplitCamelCase()})";
        }

        protected override void Unload() {
            GameService.Gw2Mumble.CurrentMap.MapChanged                 -= OnMapChanged;
            GameService.Gw2Mumble.PlayerCharacter.SpecializationChanged -= OnSpecializationChanged;
            base.Unload();
        }
    }
}
