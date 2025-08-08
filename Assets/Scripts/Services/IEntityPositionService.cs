using System.Collections.Generic;
using Core;
using Data;
using UnityEngine;

namespace Services
{
    public interface IEntityPositionService :IService
    {
        public List<Vector3> GetPawnSpawnPosition(Team team);
        public Vector3 GetBasketPosition();
        public Vector3 GetCenterCourtPosition();
    }
}