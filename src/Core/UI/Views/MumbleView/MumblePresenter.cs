using Blish_HUD;
using Blish_HUD.Extended;
using Blish_HUD.Graphics.UI;
using System;
using System.Threading.Tasks;

namespace Nekres.Mumble_Info.Core.UI.Views.MumbleView {
    internal class MumblePresenter : Presenter<MumbleView, Gw2MumbleService> {

        private string _eliteSpecializationName = string.Empty;
        private string _mapName                 = string.Empty;

        public MumblePresenter(MumbleView view, Gw2MumbleService model) : base(view, model) {
            this.Model.CurrentMap.MapChanged                 += OnMapChanged;
            this.Model.PlayerCharacter.SpecializationChanged += OnSpecializationChanged;
        }

        protected override Task<bool> Load(IProgress<string> progress) {
            progress.Report("Loading…");
            OnSpecializationChanged(this, new ValueEventArgs<int>(this.Model.PlayerCharacter.Specialization));
            OnMapChanged(this, new ValueEventArgs<int>(this.Model.CurrentMap.Id));
            progress.Report(null);
            return base.Load(progress);
        }

        private async void OnSpecializationChanged(object sender, ValueEventArgs<int> e) {
            if (!MumbleInfoModule.Instance.Gw2ApiManager.IsApiAvailable()) {
                _eliteSpecializationName = string.Empty;
                return;
            }

            var spec = await TaskUtil.TryAsync(() => MumbleInfoModule.Instance.Gw2ApiManager.Gw2ApiClient.V2.Specializations.GetAsync(e.Value), MumbleInfoModule.Logger);
            _eliteSpecializationName = spec is {Elite: true} ? spec.Name : string.Empty;
        }

        private async void OnMapChanged(object sender, ValueEventArgs<int> e) {
            if (!MumbleInfoModule.Instance.Gw2ApiManager.IsApiAvailable()) {
                _mapName = string.Empty;
                return;
            }

            var map = await TaskUtil.TryAsync(() => MumbleInfoModule.Instance.Gw2ApiManager.Gw2ApiClient.V2.Maps.GetAsync(e.Value), MumbleInfoModule.Logger);
            _mapName = map?.Name ?? string.Empty;
        }

        public string GetProfession() {
            return string.IsNullOrEmpty(_eliteSpecializationName) ? $"{this.Model.PlayerCharacter.Profession}" : $"{this.Model.PlayerCharacter.Profession} ({_eliteSpecializationName})";
        }

        public string GetMap() {
            return string.IsNullOrEmpty(_mapName) ? $"{this.Model.CurrentMap.Id}" : $"{_mapName} ({this.Model.CurrentMap.Id})";
        }

        public string GetPlayerPosition() {
            var pos = string.Empty;
            if (true) {
                pos += "XYZ: ";
                pos += this.Model.PlayerCharacter.Position.X;
                pos += " / " + this.Model.PlayerCharacter.Position.Y;
                pos += " / " + this.Model.PlayerCharacter.Position.Z;
            } else {
                pos += "XZY: ";
                pos += this.Model.PlayerCharacter.Position.X;
                pos += " / " + this.Model.PlayerCharacter.Position.Z;
                pos += " / " + this.Model.PlayerCharacter.Position.Y;
            }
            return pos;
        }

        public string GetPlayerDirection() {
            var dir = string.Empty;
            if (true) {
                dir += "XYZ: ";
                dir += this.Model.PlayerCharacter.Forward.X;
                dir += " / " + this.Model.PlayerCharacter.Forward.Y;
                dir += " / " + this.Model.PlayerCharacter.Forward.Z;
            } else {
                dir += "XZY: ";
                dir += this.Model.PlayerCharacter.Forward.X;
                dir += " / " + this.Model.PlayerCharacter.Forward.Z;
                dir += " / " + this.Model.PlayerCharacter.Forward.Y;
            }
            return $"{dir} ({DirectionUtil.IsFacing(this.Model.RawClient.AvatarFront).ToString().SplitCamelCase()})";
        }

        public string GetCameraPosition() {
            var pos = string.Empty;
            if (true) {
                pos += "XYZ: ";
                pos += this.Model.PlayerCamera.Position.X;
                pos += " / " + this.Model.PlayerCamera.Position.Y;
                pos += " / " + this.Model.PlayerCamera.Position.Z;
            } else {
                pos += "XZY: ";
                pos += this.Model.PlayerCamera.Position.X;
                pos += " / " + this.Model.PlayerCamera.Position.Z;
                pos += " / " + this.Model.PlayerCamera.Position.Y;
            }
            return pos;
        }

        public string GetCameraDirection() {
            var dir = string.Empty;
            if (true) {
                dir += "XYZ: ";
                dir += this.Model.PlayerCamera.Forward.X;
                dir += " / " + this.Model.PlayerCamera.Forward.Y;
                dir += " / " + this.Model.PlayerCamera.Forward.Z;
            } else {
                dir += "XZY: ";
                dir += this.Model.PlayerCamera.Forward.X;
                dir += " / " + this.Model.PlayerCamera.Forward.Z;
                dir += " / " + this.Model.PlayerCamera.Forward.Y;
            }
            return $"{dir} ({DirectionUtil.IsFacing(this.Model.RawClient.CameraFront).ToString().SplitCamelCase()})";
        }
        protected override void Unload() {
            this.Model.CurrentMap.MapChanged                 -= OnMapChanged;
            this.Model.PlayerCharacter.SpecializationChanged -= OnSpecializationChanged;
            base.Unload();
        }

    }
}
