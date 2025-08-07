using Data;
using Gameplay.AbilitySystem;
using Gameplay.AbilitySystem.Effects;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    /// <summary>
    /// Possessable game object that can be controlled by a player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Pawn : MonoBehaviour, IAbilitySystemController, IPawnController
    {
        #region Fields
        [Header("Ability System")]
        [SerializeField] protected AbilitySystemController abilitySystemController;
        [SerializeField] private AbilityInputBindingData abilityInputBindingData;
        
        
        // TODO:: Implement Gameplay Attributes
        [Header("Movement Settings:")]
        [SerializeField] private float movementSpeed = 7f;
        [SerializeField] private float groundFriction = 100f;
        [SerializeField] private float gravity = 30f;

        [Header("References")] 
        [SerializeField] private CapsuleCollider interactionCollider;
        [SerializeField] private Transform overHeadSocket;
        [SerializeField] private Transform handRightSocket;
        [SerializeField] private Transform handLeftSocket;
        
        private Rigidbody _rigidbody;
        private Vector3 _currentGroundAdjustmentVelocity; // Velocity to adjust player position to maintain ground contact
        private bool _isGrounded;
        private int _currentLayer;
        private float _baseSensorRange;
        private RaycastSensor _sensor;
        private Vector3 _direction;
        private Vector3 _savedVelocity;
        private Vector3 _savedMovementVelocity;
        private IPlayerController _controller;
        private IBallController _ballController;
        private IGameplayEventService _gameplayEventService;

        #endregion // Fields
        
        private void Awake()
        {
            Setup();
            InitializeAbilitySystem();
        }

        public void InitializeAbilitySystem()
        {
            var data = new AbilitySystemControllerSetup
            {
                OwnerActor = gameObject,
                InputBindingData = abilityInputBindingData.ConvertToGuidBindings()
            };
            
            abilitySystemController.Setup(data);
            abilitySystemController.Initialize();
        }
        
        void OnValidate() {
            if (gameObject.activeInHierarchy) {
            }
        }

        private void FixedUpdate()
        {
            Vector3 velocity = CalculateMovementVelocity();
            SetVelocity(velocity);
            _savedVelocity = velocity;
            _savedMovementVelocity = CalculateMovementVelocity();
            
            UpdateLookAt();
        }
        
        private Vector3 CalculateMovementVelocity()
        {
            return _direction * movementSpeed;
        }

        public void SetMovementDirection(Vector3 direction)
        {
            _direction = direction;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity + _currentGroundAdjustmentVelocity;
        }

        public void UpdateLookAt()
        {
            if (_direction.sqrMagnitude > 0.01f) // Avoid rotating when not moving
            {
                Quaternion targetRotation = Quaternion.LookRotation(_direction, transform.up);
                _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, 10f * Time.fixedDeltaTime);
            }
        }
        
        private void Setup() {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;
            _rigidbody.useGravity = false;
            
            _gameplayEventService = ServiceLocator.Get<IGameplayEventService>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IBallController>(out var ballController))
            {
                // TODO: Create Catch ability
                if (!ballController.TryPossess(gameObject)) return;
                SetInteractionColliderTrigger(false);
                _ballController = ballController;
                ballController.AttachToSocket(handRightSocket);
            }
        }

        public void SetController(IPlayerController controller)
        {
            _controller = controller;
        }
        
#region Input Actions
    public void Pass(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            abilitySystemController.TryActivateAbilityOnInput(context.action.id);
        }
        if (context.canceled)
        {
            _gameplayEventService.Broadcast(GameplayTags.Event_PassBall, _ballController);
        }
    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            abilitySystemController.TryActivateAbilityOnInput(context.action.id);
        }
        
        if (context.canceled)
        {
            _gameplayEventService.Broadcast(GameplayTags.Event_ShootBall, _ballController);
        }
    }
#endregion // Input Actions

#region Ability System Controller Interface

        public void UpdateTagMap(GameplayTag gameplayTag, int count)
        {
            abilitySystemController.UpdateTagMap(gameplayTag, count);
        }

        public bool HasTag(GameplayTag gameplayTag)
        {
            return abilitySystemController.HasTag(gameplayTag);
        }

        public GameObject GetAbilitySystemOwner()
        {
            return this.gameObject;
        }

        public AbilitySystemController GetAbilitySystemController()
        {
            return abilitySystemController;
        }

        public void ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)
        {
            abilitySystemController.ApplyGameplayEffectToSelf(gameplayEffect);
        }

        public void ApplyGameplayEffectToTarget(GameplayEffect gameplayEffect, GameObject target)
        {
            abilitySystemController.ApplyGameplayEffectToTarget(gameplayEffect, target);
        }

        #endregion

#region Pawn Controller Interface
        public void SetInteractionColliderTrigger(bool isTrigger)
        {
            // interactionCollider.isTrigger = isTrigger;
            interactionCollider.enabled = isTrigger;
        }
#endregion // Pawn Controller Interface
    }
}