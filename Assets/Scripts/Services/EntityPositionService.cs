using System.Collections.Generic;
using Data;
using Gameplay;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// The EntityPositionService is responsible for managing and providing positional data of game entities
    /// used in the game world. This includes spawn locations for teams, the basket position,
    /// and the center of the court.
    /// </summary>
    public class EntityPositionService : MonoBehaviour, IEntityPositionService
    {
        [SerializeField] private List<SpawnPoint> teamASpawnPoints;
        [SerializeField] private List<SpawnPoint> teamBSpawnPoints;
        [SerializeField] private Transform basket;
        [SerializeField] private Transform centerCourt;
        
        private Dictionary<Team, List<Vector3>> _spawnPoints;
        
        public void Initialize()
        {
            var spawnPointA = new List<Vector3>();
            var spawnPointB = new List<Vector3>();

            foreach (var spawnPoint in teamASpawnPoints)
            {
                spawnPointA.Add(spawnPoint.transform.position);
            }

            foreach (var spawnPoint in teamBSpawnPoints)
            {
                spawnPointB.Add(spawnPoint.transform.position);
            }
                
            _spawnPoints = new Dictionary<Team, List<Vector3>>
            {
                {Team.A, spawnPointA},
                {Team.B, spawnPointB}
            };
        }

        public void PostInitialize()
        {
        }

        public void Shutdown()
        {
        }

        public string GetServiceName()
        {
            return nameof(EntityPositionService);
        }

        public List<Vector3> GetPawnSpawnPosition(Team team)
        {
            return _spawnPoints[team];
        }

        public Vector3 GetBasketPosition()
        {
            return basket.position;
        }

        public Vector3 GetCenterCourtPosition()
        {
            return centerCourt.position;
        }
    }
}