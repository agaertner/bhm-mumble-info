using Blish_HUD;
using Blish_HUD.Extended;
using Blish_HUD.Graphics.UI;

namespace Nekres.Mumble_Info.Core.UI {
    internal class MumblePresenter : Presenter<MumbleView, MumbleConfig> {

        private const string DECIMAL_FORMAT = "0.###";

        public MumblePresenter(MumbleView view, MumbleConfig model) : base(view, model) {

        }

        public string GetRace() {
            if (string.IsNullOrEmpty(GameService.Gw2Mumble.PlayerCharacter.Name)) {
                return string.Empty;
            }

            return $"{GameService.Gw2Mumble.PlayerCharacter.Name} ({MumbleInfoModule.Instance.Api.GetRaceName((int)GameService.Gw2Mumble.PlayerCharacter.Race)})";
        }

        public string GetPlayerProfession() {
            var elite = MumbleInfoModule.Instance.Api.GetSpecializationName(GameService.Gw2Mumble.PlayerCharacter.Specialization);
            var prof = MumbleInfoModule.Instance.Api.GetProfessionName((int)GameService.Gw2Mumble.PlayerCharacter.Profession);
            return string.IsNullOrEmpty(elite) ? prof : $"{elite} ({prof})";
        }

        public string GetMap() {
            var mapName = MumbleInfoModule.Instance.Api.Map?.Name ?? string.Empty;
            return string.IsNullOrEmpty(mapName) ? $"{GameService.Gw2Mumble.CurrentMap.Id}" : $"{mapName} ({GameService.Gw2Mumble.CurrentMap.Id})";
        }

        public string GetSector() {
            return MumbleInfoModule.Instance.Api.Sector?.Name ?? string.Empty;
        }

        public string GetClosestWaypoint() {
            return MumbleInfoModule.Instance.Api.ClosestWaypoint?.Name ?? string.Empty;
        }

        public string GetClosestPoi() {
            return MumbleInfoModule.Instance.Api.ClosestPoi?.Name ?? string.Empty;
        }

        public string GetPlayerPosition() {
            return string.Format(this.Model.SwapYZ ? "{0} / {2} / {1}" : "{0} / {1} / {2}",
                                 GameService.Gw2Mumble.PlayerCharacter.Position.X.ToString(DECIMAL_FORMAT), 
                                 GameService.Gw2Mumble.PlayerCharacter.Position.Y.ToString(DECIMAL_FORMAT),
                                 GameService.Gw2Mumble.PlayerCharacter.Position.Z.ToString(DECIMAL_FORMAT));
        }

        public string GetPlayerDirection() {
            var dir = string.Format(this.Model.SwapYZ ? "{0} / {2} / {1}" : "{0} / {1} / {2}",
                                    GameService.Gw2Mumble.PlayerCharacter.Forward.X.ToString(DECIMAL_FORMAT),
                                    GameService.Gw2Mumble.PlayerCharacter.Forward.Y.ToString(DECIMAL_FORMAT),
                                    GameService.Gw2Mumble.PlayerCharacter.Forward.Z.ToString(DECIMAL_FORMAT));
            return $"{dir} ({DirectionUtil.IsFacing(GameService.Gw2Mumble.RawClient.AvatarFront).ToString().SplitCamelCase()})";
        }

        public string GetCameraPosition() {
            return string.Format(this.Model.SwapYZ ? "{0} / {2} / {1}" : "{0} / {1} / {2}",
                                    GameService.Gw2Mumble.PlayerCamera.Position.X.ToString(DECIMAL_FORMAT),
                                    GameService.Gw2Mumble.PlayerCamera.Position.Y.ToString(DECIMAL_FORMAT),
                                    GameService.Gw2Mumble.PlayerCamera.Position.Z.ToString(DECIMAL_FORMAT));
        }

        public string GetCameraDirection() {
            var dir = string.Format(this.Model.SwapYZ ? "{0} / {2} / {1}" : "{0} / {1} / {2}",
                                    GameService.Gw2Mumble.PlayerCamera.Forward.X.ToString(DECIMAL_FORMAT),
                                    GameService.Gw2Mumble.PlayerCamera.Forward.Y.ToString(DECIMAL_FORMAT),
                                    GameService.Gw2Mumble.PlayerCamera.Forward.Z.ToString(DECIMAL_FORMAT));
            return $"{dir} ({DirectionUtil.IsFacing(GameService.Gw2Mumble.RawClient.CameraFront).ToString().SplitCamelCase()})";
        }

        protected override void Unload() {
            base.Unload();
        }
    }
}
