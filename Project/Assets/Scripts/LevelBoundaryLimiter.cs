using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class LevelBoundaryLimiter : MonoBehaviour
    {
        /// <summary>
        /// Start запускается перед первым кадром.
        /// </summary>
        private void Start()
        {
        
        }

        /// <summary>
        /// Update запускается каждый кадр.
        /// </summary>
        private void Update()
        {
            if(LevelBoundary.Instance == null) return;
            
            LevelBoundary levelBoundary = LevelBoundary.Instance;
            float radius = levelBoundary.Radius;
            Vector3 position = transform.position;

            if (position.magnitude > radius)
            {
                switch (levelBoundary.Mode)
                {
                    case BoundaryMode.Wall:
                        transform.position = position.normalized * radius;
                        break;
                    case BoundaryMode.Teleport:
                        transform.position = -position.normalized * radius;
                        break;
                }
            }
        }
    }
}
