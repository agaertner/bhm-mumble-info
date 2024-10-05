using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Extended;
using Blish_HUD.Graphics.UI;
using Gw2Sharp.Models;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Nekres.Mumble_Info.Core.UI {
    internal class MumblePresenter : Presenter<MumbleView, MumbleConfig> {
        private const string FORMAT_2D      = "xpos=\"{0}\" ypos=\"{1}\"";
        private const string FORMAT_3D      = FORMAT_2D + " zpos=\"{2}\"";
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
            return string.IsNullOrEmpty(elite)  ? prof :
                   string.IsNullOrEmpty(prof) ? elite : $"{elite} ({prof})";
        }

        public string GetMap() {
            if (GameService.Gw2Mumble.CurrentMap.Id <= 0) {
                return string.Empty;
            }
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

        private string Vec3ToStr(Vector3 vec, bool markerPackFormat) {
            var format = markerPackFormat ? FORMAT_3D : "{0} / {1} / {2}";
            return string.Format(format,
                                 vec.X.ToString(DECIMAL_FORMAT, NumberFormatInfo.InvariantInfo),
                                 vec.Y.ToString(DECIMAL_FORMAT, NumberFormatInfo.InvariantInfo),
                                 vec.Z.ToString(DECIMAL_FORMAT, NumberFormatInfo.InvariantInfo));
        }
        private string Coords2ToStr(Coordinates2 vec, bool markerPackFormat) {
            var format = markerPackFormat ? FORMAT_2D : "{0} / {1}";
            return string.Format(format,
                                 vec.X.ToString(DECIMAL_FORMAT, NumberFormatInfo.InvariantInfo),
                                 vec.Y.ToString(DECIMAL_FORMAT, NumberFormatInfo.InvariantInfo));
        }

        public string GetPlayerPosition(bool markerPackFormat) {
            return Vec3ToStr(GameService.Gw2Mumble.PlayerCharacter.Position(this.Model.SwapYZ), markerPackFormat);
        }
        public string GetPlayerPosition() => GetPlayerPosition(false);

        public string GetPlayerDirection(bool markerPackFormat) {
            var dir = Vec3ToStr(GameService.Gw2Mumble.PlayerCharacter.Forward(this.Model.SwapYZ), markerPackFormat);
            return markerPackFormat ? dir : $"{dir} ({DirectionUtil.IsFacing(GameService.Gw2Mumble.RawClient.AvatarFront.SwapYZ()).ToString().SplitCamelCase()})";
        }
        public string GetPlayerDirection() => GetPlayerDirection(false);

        public string GetCameraPosition(bool markerPackFormat) {
            return Vec3ToStr(GameService.Gw2Mumble.PlayerCamera.Position(this.Model.SwapYZ), markerPackFormat);
        }
        public string GetCameraPosition() => GetCameraPosition(false);

        public string GetCameraDirection(bool markerPackFormat) {
            var dir = Vec3ToStr(GameService.Gw2Mumble.PlayerCamera.Forward(this.Model.SwapYZ), markerPackFormat);
            return markerPackFormat ? dir : $"{dir} ({DirectionUtil.IsFacing(GameService.Gw2Mumble.RawClient.CameraFront.SwapYZ()).ToString().SplitCamelCase()})";
        }
        public string GetCameraDirection() => GetCameraDirection(false);

        protected override void Unload() {
            base.Unload();
        }

        public string GetProcessId() {
            return $"{GameService.Gw2Mumble.Info.ProcessId}";
        }

        public string GetServerAddress() {
            return $"{GameService.Gw2Mumble.Info.ServerAddress} : {GameService.Gw2Mumble.Info.ServerPort}";
        }

        public string GetShardId() {
            return $"{GameService.Gw2Mumble.Info.ShardId}";
        }

        public string GetUiSize() {
            return $"{GameService.Gw2Mumble.UI.UISize}";
        }

        public string GetCompassBounds() {
            var compass = GameService.Gw2Mumble.UI.CompassBounds();
            return $"{compass.X} X / {compass.Y} Y / {compass.Width} W / {compass.Height} H";
        }

        public async Task CopyToClipboard(string text) {
            bool copied = false;
            try {
                copied = await ClipboardUtil.WindowsClipboardService.SetTextAsync(text);
            } catch (Exception e) {
                MumbleInfoModule.Logger.Warn(e, e.Message);
            }
            if (copied) {
                ScreenNotification.ShowNotification("Copied to Clipboard");
                GameService.Content.PlaySoundEffectByName("color-change");
            } else {
                ScreenNotification.ShowNotification("Unable to Copy. Please try again.", ScreenNotification.NotificationType.Warning);
                GameService.Content.PlaySoundEffectByName("error");
            }
        }

        public string GetMapPosition(bool markerPackFormat) {
            return $"{Coords2ToStr(GameService.Gw2Mumble.UI.MapPosition, markerPackFormat)}";
        }
        public string GetMapPosition() => GetMapPosition(false);

        public string GetMapType() {
            var comp = GameService.Gw2Mumble.CurrentMap.IsCompetitiveMode ? " (Competitive)" : string.Empty;
            return $"{GameService.Gw2Mumble.CurrentMap.Type}" + comp;
        }

        public string GetContinent() {
            var continent = MumbleInfoModule.Instance.Api.Map?.ContinentName ?? string.Empty;
            var region    = MumbleInfoModule.Instance.Api.Map?.RegionName    ?? string.Empty;
            if (string.IsNullOrEmpty(continent) || string.IsNullOrEmpty(region)) {
                return string.Empty;
            }
            return $"{continent} - {region}";
        }

        public string GetMapHash(bool discordRichPresenceFormat) {
            if (MumbleInfoModule.Instance.Api.Map == null) {
                return string.Empty;
            }

            var format = discordRichPresenceFormat ? "\"{0}\": {1}, // {2} ({1})" : "{0}";
            return string.Format(format, 
                                 MumbleInfoModule.Instance.Api.Map.GetHash(), 
                                 MumbleInfoModule.Instance.Api.Map.Id, 
                                 MumbleInfoModule.Instance.Api.Map.Name);
        }

        public string GetMapHash() => GetMapHash(false);

    }
}
