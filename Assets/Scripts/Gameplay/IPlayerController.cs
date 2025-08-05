using UnityEngine;

namespace Gameplay
{
    public interface IPlayerController
    {
        public bool TryPossess(GameObject pawn);
        public void Possess(Pawn pawn);
        public void UnPossess();
    }
}