using UnityEngine;

namespace Gameplay
{
    public interface IBallController
    {
        public void UnPossess(GameObject pawn);
        public bool TryPossess(GameObject pawn);
        public void AttachToSocket(Transform socket);
        public void Move(Vector3 origin, Vector3 target, float speedMultiplier);
        public void MovePhysics(Vector3 origin, Vector3 target, float angle = 45f, float forceMultiplier = 1f);
    }
}