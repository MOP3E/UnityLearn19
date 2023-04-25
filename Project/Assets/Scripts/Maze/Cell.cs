using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Графическое представление ячейки лабиринта.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        /// <summary>
        /// Данные ячейки лаибиринта.
        /// </summary>
        private CellData _data;

        /// <summary>
        /// Данные ячейки лаибиринта.
        /// </summary>
        public CellData Data
        {
            get => _data;
            set
            {
                if (_data != null) _data.OnNeedRebuildWalls -= RebuildWalls;
                _data = value;
                _data.OnNeedRebuildWalls += RebuildWalls;
            }
        }

        /// <summary>
        /// Верхняя стенка.
        /// </summary>
        [SerializeField] private GameObject _topWall;

        /// <summary>
        /// Нижняя стенка.
        /// </summary>
        [SerializeField] private GameObject _bottomWall;

        /// <summary>
        /// Левая стенка.
        /// </summary>
        [SerializeField] private GameObject _leftWall;

        /// <summary>
        /// Правая стенка.
        /// </summary>
        [SerializeField] private GameObject _rightWall;

        /// <summary>
        /// Перестроение стенок ячейки лабиринта.
        /// </summary>
        private void RebuildWalls()
        {
            if (_data.NeedRebuildWalls)
            {
                _topWall.SetActive(_data.TopWall);
                _bottomWall.SetActive(_data.BottomWall);
                _leftWall.SetActive(_data.LeftWall);
                _rightWall.SetActive(_data.RightWall);
                _data.RebuildWalls(false);
            }
        }
    }
}
