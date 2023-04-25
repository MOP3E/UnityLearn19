using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BackgroundElement : MonoBehaviour
    {
        /// <summary>
        /// Сила параллакс-эффекта.
        /// </summary>
        [SerializeField] [Range(0f, 4f)] private float _parallaxStrength;

        /// <summary>
        /// Масштаб текстуры.
        /// </summary>
        [SerializeField] private float _textureScale;

        /// <summary>
        /// Материал для эффекта.
        /// </summary>
        private Material _material;

        /// <summary>
        /// Начальное смещение текстуры.
        /// </summary>
        private Vector2 _startOffset;

        /// <summary> 
        /// Start запускается перед первым кадром.
        /// </summary>
        private void Start()
        {
            _material = GetComponent<MeshRenderer>().material;
            _startOffset = Random.insideUnitCircle;
            _material.mainTextureScale = new Vector2(_textureScale, _textureScale);
        }

        /// <summary> 
        /// Update запускается каждый кадр.
        /// </summary>
        void Update()
        {
            Vector2 offset = _startOffset;

            offset.x = transform.position.x / transform.localScale.x / _parallaxStrength;
            offset.y = transform.position.y / transform.localScale.y / _parallaxStrength;

            _material.mainTextureOffset = offset;
        }
    }
}
