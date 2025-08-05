using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Services
{
    public interface ICourtPositionService :IService
    {
        public List<Vector3> GetSpawnPoints(Team team);
    }
}