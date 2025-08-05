using UnityEngine;
using UnityEngine.InputSystem;

namespace Services
{
    public interface IPlayerSpawnerService : IService
    {
        public bool TrySpawnPlayerPawn(GameObject controller, Vector3 position, out GameObject pawn);
        public void SpawnOpponentPawn(Vector3 position, out GameObject pawn);
    }
}