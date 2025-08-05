using System;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Services
{
    public class PlayerSpawnerService : MonoBehaviour, IPlayerSpawnerService
    {
        [SerializeField] private GameObject _pawnPrefab;

        public void Initialize()
        {
        }

        public void PostInitialize()
        {
        }

        public void Shutdown()
        {
        }

        public string GetServiceName()
        {
            return nameof(PlayerSpawnerService);
        }

        public bool TrySpawnPlayerPawn(GameObject controller, Vector3 position, out GameObject pawn)
        {
            pawn = null;
            if (!controller.TryGetComponent<IPlayerController>(out var playerControllerInterface))
            {
                Debug.LogError("Player controller not found");
                return false;
            }
            
            pawn = Instantiate(_pawnPrefab, position, Quaternion.identity);
            if (playerControllerInterface.TryPossess(pawn)) return true;
            
            Destroy(pawn);
            Debug.LogError("Failed to possess player pawn");
            return false;
        }

        public void SpawnOpponentPawn(Vector3 position, out GameObject pawn)
        {
            pawn = Instantiate(_pawnPrefab, position, Quaternion.identity);
        }
    }
}