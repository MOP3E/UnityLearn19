using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class AiPatrolZone : MonoBehaviour
    {
        [SerializeField] private float _radius;

        public float Radius => _radius;

#if UNITY_EDITOR
        private static readonly Color _gizmoColor = new Color(1, 0, 0, .3f);

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, _radius);
        }
#endif
    }
}
