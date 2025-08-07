using System;
using System.Collections.Generic;
using Data;
using Gameplay.AbilitySystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    /// <summary>
    /// The interface between the Pawn and the human player controlling it. The PlayerController essentially represents the human player's will.
    /// </summary>
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private Transform cameraTransform;
        
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
            _possessedPawn.Pass(context);
        }
        
        public void Shoot(InputAction.CallbackContext context)
        {
            _possessedPawn.Shoot(context);
        }
        
        #endregion // Input Actions

        private void Awake()
        {
            
        }

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
            pawn.SetController(this);
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