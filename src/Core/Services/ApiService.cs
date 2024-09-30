using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Extended;
using Gw2Sharp.Models;
using Gw2Sharp.WebApi.V2.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Nekres.Mumble_Info.Core.Services {
    internal class ApiService : IDisposable {
        public Map                           Map             { get; private set; }
        public ContinentFloorRegionMapSector Sector          { get; private set; }
        public ContinentFloorRegionMapPoi    ClosestWaypoint { get; private set; }
        public ContinentFloorRegionMapPoi    ClosestPoi      { get; private set; }

        public AsyncTexture2D WaypointIcon   { get; private set; }
        public AsyncTexture2D PoiIcon        { get; private set; }
        public AsyncTexture2D ProfessionIcon { get; private set; }

        private IReadOnlyList<ContinentFloorRegionMap>       _regionMaps;
        private IReadOnlyList<ContinentFloorRegionMapSector> _mapSectors;

        private Dictionary<int, string>         _raceNames;
        private Dictionary<int, string>         _profNames;
        private Dictionary<int, AsyncTexture2D> _profIcons;
        private Dictionary<int, string>         _eliteNames;
        private Dictionary<int, AsyncTexture2D> _eliteIcons;

        public ApiService() {
            this.ProfessionIcon =  new AsyncTexture2D();
            GameService.Gw2Mumble.CurrentMap.MapChanged += OnMapChanged;
            GameService.Overlay.UserLocaleChanged       += OnUserLocaleChanged;
        }

        public async Task Init() {
            await RequestMap(GameService.Gw2Mumble.CurrentMap.Id);
            await RequestProfessions();
            WaypointIcon = GameService.Content.DatAssetCache.GetTextureFromAssetId(733330);
            PoiIcon      = GameService.Content.DatAssetCache.GetTextureFromAssetId(156624);
        }

        public void Update(GameTime gameTime) {
            FindClosestPoints();
        }

        private async void OnUserLocaleChanged(object sender, ValueEventArgs<CultureInfo> e) {
            await Init();
        }

        public string GetRaceName(int raceId) {
            return _raceNames.TryGetValue(raceId, out var name) ? name : string.Empty;
        }

        public string GetProfessionName(int professionId) {
            return _profNames.TryGetValue(professionId, out var name) ? name : string.Empty;
        }

        public string GetSpecializationName(int specializationId) {
            return _eliteNames.TryGetValue(specializationId, out var name) ? name : string.Empty;
        }

        public AsyncTexture2D GetClassIcon(int profession, int elite) {
            return _eliteIcons.TryGetValue(elite, out var icon) ? icon :
                   _profIcons.TryGetValue(profession, out icon) ? icon : ContentService.Textures.TransparentPixel;
        }

        private async void OnMapChanged(object sender, ValueEventArgs<int> e) {
            await RequestMap(e.Value);
        }

        private async Task RequestMap(int mapId) {
            if (!MumbleInfoModule.Instance.Gw2ApiManager.IsApiAvailable()) {
                this.Map = null;
                return;
            }
            this.Map    = await TaskUtil.TryAsync(() => MumbleInfoModule.Instance.Gw2ApiManager.Gw2ApiClient.V2.Maps.GetAsync(mapId), MumbleInfoModule.Logger);
            if (this.Map != null) {
                _regionMaps = await RequestRegionMap(this.Map);
                _mapSectors = await RequestMapSectors(this.Map);
            }
        }

        private async Task RequestProfessions() {
            var races = await TaskUtil.TryAsync(() => GameService.Gw2WebApi.AnonymousConnection.Client.V2.Races.AllAsync());
            if (races != null) {
                _raceNames = races.ToDictionary(x => (int)(RaceType)Enum.Parse(typeof(RaceType), x.Id, true), x => x.Name);
            }
            
            var professions = await TaskUtil.TryAsync(() => GameService.Gw2WebApi.AnonymousConnection.Client.V2.Professions.AllAsync());
            if (professions != null) {
                _profNames = professions.ToDictionary(x => (int)(ProfessionType)Enum.Parse(typeof(ProfessionType), x.Id, true), x => x.Name);
                _profIcons = professions.ToDictionary(x => (int)(ProfessionType)Enum.Parse(typeof(ProfessionType), x.Id, true), x => GameService.Content.GetRenderServiceTexture(x.IconBig));
            }

            var specializations = await TaskUtil.TryAsync(() => GameService.Gw2WebApi.AnonymousConnection.Client.V2.Specializations.AllAsync());
            if (specializations != null) {
                var elites = specializations.Where(x => x.Elite).ToList();
                _eliteNames = elites.ToDictionary(x => x.Id, x => x.Name);
                _eliteIcons = elites.ToDictionary(x => x.Id, x => GameService.Content.GetRenderServiceTexture(x.ProfessionIconBig));
            }
        }

        private async Task<IReadOnlyList<ContinentFloorRegionMap>> RequestRegionMap(Map map) {
            var regionMaps = new List<ContinentFloorRegionMap>();
            foreach (int floor in map.Floors) {
                var regionMap = await TaskUtil.TryAsync(() => MumbleInfoModule.Instance.Gw2ApiManager.Gw2ApiClient.V2
                                                                        .Continents[map.ContinentId]
                                                                        .Floors[floor]
                                                                        .Regions[map.RegionId]
                                                                        .Maps[map.Id].GetAsync());
                regionMaps.Add(regionMap);
            }
            return regionMaps;
        }

        private async Task<List<ContinentFloorRegionMapSector>> RequestMapSectors(Map map) {
            var result = new List<ContinentFloorRegionMapSector>();
            foreach (var floor in map.Floors) {
                var sectors = await TaskUtil.RetryAsync(() => MumbleInfoModule.Instance.Gw2ApiManager.Gw2ApiClient.V2.Continents[map.ContinentId].Floors[floor].Regions[map.RegionId].Maps[map.Id].Sectors.AllAsync());
                if (sectors != null && sectors.Any()) {
                    result.AddRange(sectors.DistinctBy(sector => sector.Id));
                }
            }
            return result;
        }

        private void FindClosestPoints() {
            if (Map == null) {
                return;
            }

            var pois = _regionMaps?.Where(x => x != null).SelectMany(x => x.PointsOfInterest.Values.Distinct()).ToList();
            if (!pois.IsNullOrEmpty()) {
                var continentPosition = GameService.Gw2Mumble.RawClient.AvatarPosition.ToContinentCoords(CoordsUnit.Mumble, this.Map.MapRect, this.Map.ContinentRect);

                double closestPoiDistance = double.MaxValue;
                double closestWaypointDistance = double.MaxValue;

                ContinentFloorRegionMapPoi closestPoi = null;
                ContinentFloorRegionMapPoi closestWaypoint = null;
                // ReSharper disable once PossibleNullReferenceException
                foreach (var poi in pois) {
                    double distanceX = Math.Abs(continentPosition.X - poi.Coord.X);
                    double distanceZ = Math.Abs(continentPosition.Z - poi.Coord.Y);
                    double distance = Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceZ, 2));

                    switch (poi.Type.Value) {
                        case PoiType.Waypoint when distance < closestWaypointDistance:
                            closestWaypointDistance = distance;
                            closestWaypoint = poi;
                            break;
                        case PoiType.Landmark when distance < closestPoiDistance:
                            closestPoiDistance = distance;
                            closestPoi = poi;
                            break;
                    }
                }
                this.ClosestWaypoint = closestWaypoint;
                this.ClosestPoi = closestPoi;
            } else {
                this.ClosestWaypoint = null;
                this.ClosestPoi = null;
            }

            // Some maps consist of just a single sector and hide their actual name in it.
            //CurrentMapName = _mapSectors is { Count: 1 } ? _mapSectors[0].Name : this.Map.Name;

            var playerLocation = GameService.Gw2Mumble.RawClient.AvatarPosition.ToContinentCoords(CoordsUnit.Mumble, this.Map.MapRect, this.Map.ContinentRect).SwapYZ().ToPlane();
            this.Sector = _mapSectors?.FirstOrDefault(sector => playerLocation.Inside(sector.Bounds));
        }

        public void Dispose() {
            this.ProfessionIcon?.Dispose();
            GameService.Gw2Mumble.CurrentMap.MapChanged -= OnMapChanged;
            GameService.Overlay.UserLocaleChanged       -= OnUserLocaleChanged;
        }

    }
}
