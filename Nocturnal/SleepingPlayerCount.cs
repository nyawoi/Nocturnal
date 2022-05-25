using System.Diagnostics;
using System.Linq;
using Plukit.Base;
using Staxel;
using Staxel.Items;
using Staxel.Logic;
using Staxel.Modding;
using Staxel.Notifications;
using Staxel.Tiles;

namespace Nocturnal
{
    public class SleepingPlayerCount : IModHookV4
    {
        private static readonly bool IsServer = Process.GetCurrentProcess().ProcessName.Contains("Server");
        private readonly Lyst<Entity> _players = new Lyst<Entity>();
        private int _lastKnownSleepingCount;
        
        public void UniverseUpdateBefore(Universe universe, Timestep step)
        {
            if (!IsServer) return;
            
            _players.Clear();
            universe.GetPlayers(_players);

            if (!universe.DayNightCycle().PastSleepTime() || _players.Count < 2) return;
            
            var sleepingPlayerCount = _players.Count(p => p.PlayerEntityLogic.IsSleeping());
            
            if (sleepingPlayerCount == _lastKnownSleepingCount) return;
            
            _lastKnownSleepingCount = sleepingPlayerCount;

            foreach (var player in _players)
            {
                if (sleepingPlayerCount == _players.Count)
                {
                    var sleepingThroughNotification = GameContext.NotificationDatabase.CreateNotificationFromCode("mods.Nocturnal.notification.SleepingThroughNight", step, NotificationParams.EmptyParams);
                    player.PlayerEntityLogic.ShowNotification(sleepingThroughNotification);
                }
                else
                {
                    var notificationParams = new NotificationParams(2)
                    {
                        Ints = new[] {sleepingPlayerCount, _players.Count}
                    };
                    var sleepingCountNotification = GameContext.NotificationDatabase.CreateNotificationFromCode("mods.Nocturnal.notification.SleepingPlayerCount", step, notificationParams);
                    player.PlayerEntityLogic.ShowNotification(sleepingCountNotification);
                }
            }
        }
        public void UniverseUpdateAfter() {}

        public void Dispose() {}
        public void CleanupOldSession() {}
        
        public void GameContextInitializeInit() {}
        public void GameContextInitializeBefore() {}
        public void GameContextInitializeAfter() {}
        public void GameContextDeinitialize() {}
        public void GameContextReloadBefore() {}
        public void GameContextReloadAfter() {}
        
        public void ClientContextInitializeInit() {}
        public void ClientContextInitializeBefore() {}
        public void ClientContextInitializeAfter() {}
        public void ClientContextDeinitialize() {}
        public void ClientContextReloadBefore() {}
        public void ClientContextReloadAfter() {}
        
        public bool CanPlaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) => true;
        public bool CanReplaceTile(Entity entity, Vector3I location, Tile tile, TileAccessFlags accessFlags) => true;
        public bool CanRemoveTile(Entity entity, Vector3I location, TileAccessFlags accessFlags) => true;
        
        public bool CanInteractWithTile(Entity entity, Vector3F location, Tile tile) => true;
        public bool CanInteractWithEntity(Entity entity, Entity lookingAtEntity) => true;
    }
}