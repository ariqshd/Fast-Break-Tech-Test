using System;
using Data;
using UnityEngine.InputSystem;

namespace Services
{
    public class LocalMultiplayerGameManagerService : GameManager
    {
        private ICourtPositionService _courtPositionService;
        private IPlayerSpawnerService _playerSpawnerService;
        private ITeamService _teamService;
        private IPlayerInputManagerService _playerInputManagerService;

        private string[] _controlSchemes = 
        {
            "WASD","Arrows"
        };
        
        public event Action OnGameStateChanged;

        public override void Initialize()
        {
            _courtPositionService = ServiceLocator.Get<ICourtPositionService>();
            _playerSpawnerService = ServiceLocator.Get<IPlayerSpawnerService>();
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
            var position = _courtPositionService.GetSpawnPoints(Team.A);
            var opponentPositions = _courtPositionService.GetSpawnPoints(Team.B);
            for (int i = 0; i < 2; i++)
            {
                var playerInput = _playerInputManagerService.JoinPlayer(playerIndex: i, controlScheme:_controlSchemes[i], pairWithDevice: Keyboard.current);
                _playerSpawnerService.TrySpawnPlayerPawn(playerInput.gameObject, position[i], out var pawn);
                _teamService.AddPlayerToTeam(pawn, Team.A);
                
                _playerSpawnerService.SpawnOpponentPawn(opponentPositions[i], out var opponentPawn);
                _teamService.AddPlayerToTeam(opponentPawn, Team.B);
            }
            
            var ballPosition = _courtPositionService.GetCenterCourtPosition();
            _playerSpawnerService.SpawnBall(ballPosition, out var ball);
        }
    }
}