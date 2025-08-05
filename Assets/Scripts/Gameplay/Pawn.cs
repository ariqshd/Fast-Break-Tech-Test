using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    /// <summary>
    /// Possessable game object that can be controlled by a player.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Pawn : MonoBehaviour
    {
        #region Fields
        [Header("Collider Settings:")]
        [Range(0f, 1f)] [SerializeField] private float stepHeightRatio = 0.1f;
        [SerializeField] private float colliderHeight = 2f;
        [SerializeField] private float colliderThickness = 1f;
        [SerializeField] private Vector3 colliderOffset = Vector3.zero;
        
        [Header("Sensor Settings:")]
        [SerializeField] bool isInDebugMode;
        private bool _isUsingExtendedSensorRange = true; // Use extended range for smoother ground transitions
        
        [Header("Movement Settings:")]
        [SerializeField] private float movementSpeed = 7f;
        [SerializeField] private float groundFriction = 100f;
        [SerializeField] private float gravity = 30f;
        
        private Rigidbody _rigidbody;
        private CapsuleCollider _collider;
        private Vector3 _currentGroundAdjustmentVelocity; // Velocity to adjust player position to maintain ground contact
        private bool _isGrounded;
        private int _currentLayer;
        private float _baseSensorRange;
        private RaycastSensor _sensor;
        private Vector3 _direction;
        private Vector3 _savedVelocity;
        private Vector3 _savedMovementVelocity;

        #endregion // Fields
        
        private void Awake()
        {
            Setup();
            RecalculateColliderDimensions();
        }
        
        void OnValidate() {
            if (gameObject.activeInHierarchy) {
                RecalculateColliderDimensions();
            }
        }
        
        void LateUpdate() {
            if (isInDebugMode) {
                _sensor.DrawDebug();
            }
        }

        private void FixedUpdate()
        {
            CheckForGround();
            Vector3 velocity = CalculateMovementVelocity();
            SetVelocity(velocity);
            
            _savedVelocity = velocity;
            _savedMovementVelocity = CalculateMovementVelocity();
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
        
        public void CheckForGround() {
            if (_currentLayer != gameObject.layer) {
                RecalculateSensorLayerMask();
            }
            
            _currentGroundAdjustmentVelocity = Vector3.zero;
            _sensor.castLength = _isUsingExtendedSensorRange 
                ? _baseSensorRange + colliderHeight * transform.localScale.x * stepHeightRatio
                : _baseSensorRange;
            _sensor.Cast();
            
            _isGrounded = _sensor.HasDetectedHit();
            if (!_isGrounded) return;
            
            float distance = _sensor.GetDistance();
            float upperLimit = colliderHeight * transform.localScale.x * (1f - stepHeightRatio) * 0.5f;
            float middle = upperLimit + colliderHeight * transform.localScale.x * stepHeightRatio;
            float distanceToGo = middle - distance;
            
            _currentGroundAdjustmentVelocity = transform.up * (distanceToGo / Time.fixedDeltaTime);
        }
        
        private void Setup() {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
            
            _rigidbody.freezeRotation = true;
            _rigidbody.useGravity = false;
        }
        
        private void RecalculateColliderDimensions() {
            if (_collider == null) {
                Setup();
            }
            
            _collider.height = colliderHeight * (1f - stepHeightRatio);
            _collider.radius = colliderThickness / 2f;
            _collider.center = colliderOffset * colliderHeight + new Vector3(0f, stepHeightRatio * _collider.height / 2f, 0f);

            if (_collider.height / 2f < _collider.radius) {
                _collider.radius = _collider.height / 2f;
            }
            
            RecalibrateSensor();
        }
        
        private void RecalibrateSensor() {
            _sensor ??= new RaycastSensor(transform);
            
            _sensor.SetCastOrigin(_collider.bounds.center);
            _sensor.SetCastDirection(RaycastSensor.CastDirection.Down);
            RecalculateSensorLayerMask();
            
            const float safetyDistanceFactor = 0.001f; // Small factor added to prevent clipping issues when the sensor range is calculated
            
            float length = colliderHeight * (1f - stepHeightRatio) * 0.5f + colliderHeight * stepHeightRatio;
            _baseSensorRange = length * (1f + safetyDistanceFactor) * transform.localScale.x;
            _sensor.castLength = length * transform.localScale.x;
        }
        
        private void RecalculateSensorLayerMask() {
            int objectLayer = gameObject.layer;
            int layerMask = Physics.AllLayers;

            for (int i = 0; i < 32; i++) {
                if (Physics.GetIgnoreLayerCollision(objectLayer, i)) {
                    layerMask &= ~(1 << i);
                }
            }
            
            int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            layerMask &= ~(1 << ignoreRaycastLayer);
            
            _sensor.layermask = layerMask;
            _currentLayer = objectLayer;
        }
    }
}