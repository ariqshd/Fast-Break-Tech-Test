using System.Collections.Generic;
using System;
using UnityEngine;
namespace Gameplay
{
public class RaycastSensor {
        public float castLength = 1f;
        public LayerMask layermask = 255;
        
        private Vector3 _origin = Vector3.zero;
        private Transform _transform;
        
        public enum CastDirection { Forward, Right, Up, Backward, Left, Down }
        private CastDirection _castDirection;
        
        private RaycastHit _hitInfo;

        public RaycastSensor(Transform playerTransform) {
            _transform = playerTransform;
        }

        public void Cast() {
            Vector3 worldOrigin = _transform.TransformPoint(_origin);
            Vector3 worldDirection = GetCastDirection();
            
            Physics.Raycast(worldOrigin, worldDirection, out _hitInfo, castLength, layermask, QueryTriggerInteraction.Ignore);
        }
        
        public bool HasDetectedHit() => _hitInfo.collider != null;
        public float GetDistance() => _hitInfo.distance;
        public Vector3 GetNormal() => _hitInfo.normal;
        public Vector3 GetPosition() => _hitInfo.point;
        public Collider GetCollider() => _hitInfo.collider;
        public Transform GetTransform() => _hitInfo.transform;
        
        public void SetCastDirection(CastDirection direction) => _castDirection = direction;
        public void SetCastOrigin(Vector3 pos) => _origin = _transform.InverseTransformPoint(pos);

        Vector3 GetCastDirection() {
            return _castDirection switch {
                CastDirection.Forward => _transform.forward,
                CastDirection.Right => _transform.right,
                CastDirection.Up => _transform.up,
                CastDirection.Backward => -_transform.forward,
                CastDirection.Left => -_transform.right,
                CastDirection.Down => -_transform.up,
                _ => Vector3.one
            };
        }
        
        public void DrawDebug() {
            if (!HasDetectedHit()) return;

            Debug.DrawRay(_hitInfo.point, _hitInfo.normal, Color.red, Time.deltaTime);
            float markerSize = 0.2f;
            Debug.DrawLine(_hitInfo.point + Vector3.up * markerSize, _hitInfo.point - Vector3.up * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(_hitInfo.point + Vector3.right * markerSize, _hitInfo.point - Vector3.right * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(_hitInfo.point + Vector3.forward * markerSize, _hitInfo.point - Vector3.forward * markerSize, Color.green, Time.deltaTime);
        }
    }
}