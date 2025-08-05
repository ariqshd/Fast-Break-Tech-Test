using UnityEngine;

namespace Gameplay
{
    public class SpawnPoint : MonoBehaviour
    {
        private bool _isOccupied;
        
        public bool IsOccupied => _isOccupied;
        public void SetOccupied(bool isOccupied) => _isOccupied = isOccupied;
    }
}