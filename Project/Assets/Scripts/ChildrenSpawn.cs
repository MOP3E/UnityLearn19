using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Описание генерации потомков при уничтожении этого объекта.
    /// </summary>
    public class ChildrenSpawn : MonoBehaviour
    {
        /// <summary>
        /// Количество потомков.
        /// </summary>
        [SerializeField] private int _childrenCount;

        /// <summary>
        /// Количество потомков.
        /// </summary>
        public int ChildrenCount => _childrenCount;

        /// <summary>
        /// Префабы для генерации потомков.
        /// </summary>
        [SerializeField] private Destructible[] _childrenPrefabs;

        /// <summary>
        /// Префабы для генерации потомков.
        /// </summary>
        public Destructible[] ChildrenPrefabs => _childrenPrefabs;
    }
}
