using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    /// <summary>
    /// The interface between the Pawn and the human player controlling it. The PlayerController essentially represents the human player's will.
    /// </summary>
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] Transform cameraTransform;
        
        private Pawn _possessedPawn;
        private Vector2 _savedMoveInput;

        #region Input Actions
        public void Movement(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _savedMoveInput = context.ReadValue<Vector2>();
            }
            
            if (context.canceled)
            {
                _savedMoveInput = Vector2.zero;
            }
        }

        public void Pass(InputAction.CallbackContext context)
        {
            Debug.Log("Pass");
        }
        
        public void Shoot(InputAction.CallbackContext context)
        {
            Debug.Log("Shoot");
        }
        
        #endregion // Input Actions
        
        private void FixedUpdate()
        {
            if(_possessedPawn == null) return;
            
            Vector3 direction = CalculateMovementDirection();
            _possessedPawn.SetMovementDirection(direction);
        }

        private Vector3 CalculateMovementDirection() {
            Vector3 direction = cameraTransform == null 
                ? transform.right * _savedMoveInput.x + transform.forward * _savedMoveInput.y 
                : Vector3.ProjectOnPlane(cameraTransform.right, transform.up).normalized * _savedMoveInput.x + 
                  Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized * _savedMoveInput.y;
            
            return direction.magnitude > 1f ? direction.normalized : direction;
        }

        public void Possess(Pawn pawn)
        {
            _possessedPawn = pawn;
        }

        public void UnPossess()
        {
            _possessedPawn = null;
        }

        public bool TryPossess(GameObject pawn)
        {
            if (!pawn.TryGetComponent<Pawn>(out Pawn pawnComponent)) return false;
            Possess(pawnComponent);
            return true;
        }
    }
}