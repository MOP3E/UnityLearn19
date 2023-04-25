using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    public class CameraContoller : MonoBehaviour
    {
        /// <summary>
        /// Игровая камера.
        /// </summary>
        [SerializeField] private Camera _camera;

        /// <summary>
        /// Цель, которую должна отслеживать цифорвая камера.
        /// </summary>
        [SerializeField] private Transform _target;

        /// <summary>
        /// Цель, которую должна отслеживать цифорвая камера.
        /// </summary>
        public Transform Tagret
        {
            get => _target;
            set => _target = value;
        }

        /// <summary>
        /// Скорость линейной интерполяции.
        /// </summary>
        [SerializeField] private float _interpolationSpeed;

        /// <summary>
        /// Скорость угловой интерполяции.
        /// </summary>
        [SerializeField] private float _angularInterpolationSpeed;

        /// <summary>
        /// Смещение камеры по оси Z.
        /// </summary>
        [SerializeField] private float _cameraZOffset;

        /// <summary>
        /// Смещение камеры по оси движения.
        /// </summary>
        [SerializeField] private float _cameraForwardOffset;
        
        /// <summary> 
        /// Start запускается перед первым кадром.
        /// </summary>
        private void Start()
        {
            if (_camera == null || _target == null) return;
            _cameraZOffset = _camera.transform.position.z;
        }

        /// <summary> 
        /// Update запускается каждый кадр.
        /// </summary>
        void FixedUpdate()
        {
            if(_camera == null || _target == null) return;

            //получить текущую позицию камеры
            Vector2 cameraPosition = _camera.transform.position;
            //рассчитать желаемую позицию камеры
            Vector2 targetPosiotion = _target.position + _target.transform.up * _cameraForwardOffset;
            //интерполировать камеру из текущей позиции в желаемую
            Vector2 newCameraPosition = Vector2.Lerp(cameraPosition, targetPosiotion, _interpolationSpeed * Time.deltaTime);
            _camera.transform.position = new Vector3(newCameraPosition.x, newCameraPosition.y, _cameraZOffset);

            if(_angularInterpolationSpeed == 0) return;
            _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, _target.rotation, _angularInterpolationSpeed * Time.deltaTime);
        }
    }
}
