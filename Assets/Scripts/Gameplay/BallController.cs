using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.AbilitySystem;
using Gameplay.AbilitySystem.Effects;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class BallController : MonoBehaviour, IBallController, IGameplayEffectObject
    {
        [Header("Gameplay Effects")]
        [SerializeField] private List<GameplayEffect> effects;
        
        private bool _isPossessed;
        private GameObject _possessingPawn;
        private Coroutine _moveCoroutine;
        private Rigidbody _rigidBody;

        [HideInInspector] public UnityEvent onPossess;
        [HideInInspector] public UnityEvent onUnPossess;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }
        
        public bool TryPossess(GameObject pawn)
        {
            if (pawn == null)
            {
                Debug.LogWarning("Cannot possess: pawn is null");
                return false;
            }
        
            if (_isPossessed)
            {
                Debug.Log($"Cannot possess: ball already possessed by {_possessingPawn?.name}");
                return false;
            }
            
            Possess(pawn);
        
            return true;
        }
        
        private void Possess(GameObject pawn)
        {
            _isPossessed = true;
            _possessingPawn = pawn;
        
            foreach (var effect in effects)
            {
                ApplyEffectToTarget(pawn, effect);
            }
            
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }

            _rigidBody.isKinematic = true;
            _rigidBody.useGravity = false;
            _rigidBody.detectCollisions = false;

            onPossess?.Invoke();
            Debug.Log($"Ball possessed by {pawn.name}");
        }
    
        public void UnPossess(GameObject pawn)
        {
            if (!_isPossessed)
            {
                Debug.LogWarning("Cannot unpossess: ball is not possessed");
                return;
            }
            
            if (_possessingPawn != pawn)
            {
                Debug.LogWarning($"Cannot unpossess: {pawn?.name} doesn't possess this ball");
                return;
            }
            
            _isPossessed = false;
            _possessingPawn = null;
            
            // Detach from parent (external classes handle attachment)
            transform.SetParent(null, true);
            transform.parent = null;
            onUnPossess?.Invoke();
            Debug.Log($"Ball unpossessed by {pawn?.name}");
        }
        
        public void Move(Vector3 origin, Vector3 target, float speedMultiplier = 10f)
        {
            if (!_isPossessed) return;
    
            float distance = Vector3.Distance(origin, target);
            float duration = distance / speedMultiplier; // Natural timing based on distance
    
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        
            _moveCoroutine = StartCoroutine(ExecuteMove(origin, target, duration));
        }
        
        private IEnumerator ExecuteMove(Vector3 origin, Vector3 target, float moveDuration)
        {
            UnPossess(_possessingPawn);

            float elapsedTime = 0f;
    
            // Calculate distance for dynamic arc height
            float distance = Vector3.Distance(origin, target);
            float arcHeight = Mathf.Clamp(distance * 0.3f, 1f, 5f); // Dynamic arc based on distance

            while (elapsedTime < moveDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / moveDuration);
        
                // More natural easing curve
                float curvedT = Mathf.SmoothStep(0, 1, t);
        
                // Straight line movement
                Vector3 newPosition = Vector3.Lerp(origin, target, curvedT);
        
                // Parabolic arc (more realistic than sine wave)
                float arc = arcHeight * Mathf.Sin(t * Mathf.PI);
                newPosition += Vector3.up * arc;
        
                // Add slight sidespin/horizontal movement for realism
                float sidespin = Mathf.Sin(t * Mathf.PI * 2) * 0.2f;
                newPosition += transform.right * sidespin;
        
                transform.position = newPosition;
        
                // Optional: Rotate the ball for visual effect
                transform.Rotate(Vector3.forward, 360f * Time.deltaTime / moveDuration, Space.Self);
        
                yield return null;
            }
    
            // Ensure exact final position
            transform.position = target;
            _rigidBody.isKinematic = false;
            _rigidBody.useGravity = true;
            _rigidBody.detectCollisions = true;
            _moveCoroutine = null;
        }
        
        public void MovePhysics(Vector3 origin, Vector3 target, float angle = 45f, float forceMultiplier = 1f)
        {
            if (!_isPossessed) return;
    
            UnPossess(_possessingPawn);
    
            // Enable physics
            _rigidBody.isKinematic = false;
            _rigidBody.useGravity = true;
            _rigidBody.detectCollisions = true;
    
            // Calculate realistic velocity
            Vector3 direction = (target - origin).normalized;
            float distance = Vector3.Distance(origin, target);
    
            // Basketball-like force calculation
            float gravity = Mathf.Abs(Physics.gravity.y);
    
            // Calculate velocity needed for parabolic trajectory
            float velocity = Mathf.Sqrt((distance * gravity) / Mathf.Sin(2 * angle * Mathf.Deg2Rad));
    
            // Apply force
            Vector3 throwVelocity = direction * velocity * forceMultiplier;
            throwVelocity.y = velocity * Mathf.Sin(angle * Mathf.Deg2Rad) * forceMultiplier;
    
            _rigidBody.linearVelocity = throwVelocity;
    
            // Handle cleanup when ball stops
            StartCoroutine(WaitForBallToStop());
        }

        private IEnumerator WaitForBallToStop()
        {
            yield return new WaitForSeconds(0.1f); // Let it start moving
    
            Vector3 lastPosition = transform.position;
            while (_rigidBody.linearVelocity.magnitude > 0.1f || 
                   Vector3.Distance(transform.position, lastPosition) > 0.01f)
            {
                lastPosition = transform.position;
                yield return null;
            }
    
            _rigidBody.isKinematic = true;
            _moveCoroutine = null;
        }

        public void AttachToSocket(Transform socket)
        {
            if (socket == null)
            {
                Debug.LogWarning("Cannot attach: socket is null");
                return;
            }
        
            transform.SetParent(_possessingPawn.transform);
            transform.position = _possessingPawn.transform.position + Vector3.up * 1.5f;
            // transform.position = Vector3.zero;
            // transform.localPosition = Vector3.zero;
        }
        
        public void ApplyEffectToTarget(GameObject target, GameplayEffect effect)
        {
            if (target.TryGetComponent<IAbilitySystemController>(out var abilitySystemController))
            {
                abilitySystemController.ApplyGameplayEffectToSelf(effect.CreateInstanceCopy());
            }
        }
    }
}