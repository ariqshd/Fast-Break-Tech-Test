using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Services
{
    public interface IEntitySpawnerService : IService
    {
        public bool TrySpawnPlayerPawn(GameObject controller, Vector3 position, out GameObject pawn);
        public void SpawnOpponentPawn(Vector3 position, out GameObject pawn);
        public void SpawnBall(Vector3 position, out GameObject ball);
    }
}