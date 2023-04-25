using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class SyncTransform : MonoBehaviour
    {
        /// <summary>
        /// Цель, с которой нужно синхронизировать положение.
        /// </summary>
        [SerializeField] private Transform _target;

        /// <summary> 
        /// Update запускается каждый кадр.
        /// </summary>
        void Update()
        {
            transform.position = new Vector3(_target.position.x, _target.position.y, transform.position.z);
        }
    }
}
