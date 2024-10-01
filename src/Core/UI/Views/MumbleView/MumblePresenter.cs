using Blish_HUD;
using Blish_HUD.Extended;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using System;

namespace Nekres.Mumble_Info.Core.UI {
    internal class MumblePresenter : Presenter<MumbleView, MumbleConfig> {
        private const string FORMAT_2D      = "xpos=\"{0}\" ypos=\"{1}\"";
        private const string FORMAT_3D      = FORMAT_2D + " zpos=\"{2}\"";
        private const string DECIMAL_FORMAT = "0.###";
        private const int    MAPWIDTH_MAX   = 362;
        private const int    MAPHEIGHT_MAX  = 338;
        private const int    MAPWIDTH_MIN   = 170;
        private const int    MAPHEIGHT_MIN  = 170;
        private const int    MAPOFFSET_MIN  = 19;

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
            return string.IsNullOrEmpty(elite)  ? prof :
                   string.IsNullOrEmpty(prof) ? elite : $"{elite} ({prof})";
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

        public string GetProcessId() {
            return $"PID: {GameService.Gw2Mumble.Info.ProcessId}";
        }

        public string GetServerAddress() {
            return $"Server Addr.: {GameService.Gw2Mumble.Info.ServerAddress} : {GameService.Gw2Mumble.Info.ServerPort}";
        }

        public string GetShardId() {
            return $"Shard ID: {GameService.Gw2Mumble.Info.ShardId}";
        }

        public string GetUiSize() {
            return $"UI Size: {GameService.Gw2Mumble.UI.UISize}";
        }

        private int GetOffset(float curr, float max, float min, float val) {
            return (int)Math.Round((curr - min) / (max - min) * (val - MAPOFFSET_MIN) + MAPOFFSET_MIN, 0);
        }

        public string GetCompassBounds() {
            int offsetWidth  = GetOffset(GameService.Gw2Mumble.UI.CompassSize.Width,  MAPWIDTH_MAX,  MAPWIDTH_MIN,  40);
            int offsetHeight = GetOffset(GameService.Gw2Mumble.UI.CompassSize.Height, MAPHEIGHT_MAX, MAPHEIGHT_MIN, 40);

            int width  = GameService.Gw2Mumble.UI.CompassSize.Width            + offsetWidth;
            int height = GameService.Gw2Mumble.UI.CompassSize.Height           + offsetHeight;
            int x      = GameService.Graphics.SpriteScreen.ContentRegion.Width - width;
            int y      = 0;

            if (!GameService.Gw2Mumble.UI.IsCompassTopRight) {
                y += GameService.Graphics.SpriteScreen.ContentRegion.Height - height - 40;
            }
            var compass = new Rectangle(x, y, width, height);
            return $"Compass: {compass.X} X / {compass.Y} Y / {compass.Width} W / {compass.Height} H";
        }

    }
}
