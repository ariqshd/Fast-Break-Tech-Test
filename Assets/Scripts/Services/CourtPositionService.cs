using System.Collections.Generic;
using Data;
using Gameplay;
using UnityEngine;

namespace Services
{
    public class CourtPositionService : MonoBehaviour, ICourtPositionService
    {
        [SerializeField] private List<SpawnPoint> teamASpawnPoints;
        [SerializeField] private List<SpawnPoint> teamBSpawnPoints;
        
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
            return nameof(CourtPositionService);
        }

        public List<Vector3> GetSpawnPoints(Team team)
        {
            return _spawnPoints[team];
        }
    }
}