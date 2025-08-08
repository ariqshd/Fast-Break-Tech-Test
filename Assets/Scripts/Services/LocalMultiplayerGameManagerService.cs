using System;
using Core;
using Data;
using UnityEngine.InputSystem;

namespace Services
{
    /// <summary>
    /// Service responsible for managing local multiplayer game sessions. It handles the initialization,
    /// game state management, spawning of players and opponents, team assignment, and the overall flow
    /// of a local multiplayer game.
    /// </summary>
    public class LocalMultiplayerGameManagerService : GameManager
    {
        private IEntityPositionService _entityPositionService;
        private IEntitySpawnerService _entitySpawnerService;
        private ITeamService _teamService;
        private IPlayerInputManagerService _playerInputManagerService;

        // TODO: Find a better way to handle this
        private readonly string[] _controlSchemes = 
        {
            "WASD","Arrows"
        };
        
        public override void Initialize()
        {
            _entityPositionService = ServiceLocator.Get<IEntityPositionService>();
            _entitySpawnerService = ServiceLocator.Get<IEntitySpawnerService>();
            _teamService = ServiceLocator.Get<ITeamService>();
            _playerInputManagerService = ServiceLocator.Get<IPlayerInputManagerService>();
            
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
            StartGame();
        }
        
        public override void Shutdown()
        {
        }
        
        public override string GetServiceName()
        {
            return nameof(LocalMultiplayerGameManagerService);
        }
        
        public override void StartGame()
        {
            var position = _entityPositionService.GetPawnSpawnPosition(Team.A);
            var opponentPositions = _entityPositionService.GetPawnSpawnPosition(Team.B);
            for (int i = 0; i < 2; i++)
            {
                var playerInput = _playerInputManagerService.JoinPlayer(playerIndex: i, controlScheme:_controlSchemes[i], pairWithDevice: Keyboard.current);
                _entitySpawnerService.TrySpawnPlayerPawn(playerInput.gameObject, position[i], out var pawn);
                _teamService.AddPlayerToTeam(pawn, Team.A);
                
                _entitySpawnerService.SpawnOpponentPawn(opponentPositions[i], out var opponentPawn);
                _teamService.AddPlayerToTeam(opponentPawn, Team.B);
            }
            
            var ballPosition = _entityPositionService.GetCenterCourtPosition();
            _entitySpawnerService.SpawnBall(ballPosition, out var ball);
        }
    }
}