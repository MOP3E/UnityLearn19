using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceShooter
{
    /// <summary>
    /// Данные ячейки лабиринта.
    /// </summary>
    public class CellData
    {
        /// <summary>
        /// Позиция ячейки.
        /// </summary>
        public Vector2Int Position { private set; get; }

        /// <summary>
        /// Соседние ячейки, уже добавленные к лабиринту.
        /// </summary>
        private List<Vector2Int> UnvisitedNeighbours { set; get; }

        private bool _needRebuildWalls;

        /// <summary>
        /// Необходимо перестроить стенки визуального представления ячейки лабиринта.
        /// </summary>
        public bool NeedRebuildWalls => _needRebuildWalls;

        /// <summary>
        /// Событие перестроения стенок визуального представления ячейки лабиринта.
        /// </summary>
        public event Action OnNeedRebuildWalls;

        private bool _topWall, _bottomWall, _leftWall, _rightWall;

        /// <summary>
        /// Верхняя стена.
        /// </summary>
        public bool TopWall => _topWall;

        /// <summary>
        /// Нижняя стена.
        /// </summary>
        public bool BottomWall => _bottomWall;

        /// <summary>
        /// Левая стена.
        /// </summary>
        public bool LeftWall => _leftWall;

        /// <summary>
        /// Правая стена.
        /// </summary>
        public bool RightWall => _rightWall;

        /// <summary>
        /// У этой клетки есть непосещённые соседи.
        /// </summary>
        public bool HasUnvisitedNeighbour => UnvisitedNeighbours.Count > 0;

        public CellData(Vector2Int position)
        {
            Position = position;
            _topWall = true;
            _bottomWall = true;
            _leftWall = true;
            _rightWall = true;
            UnvisitedNeighbours = new List<Vector2Int>();
        }

        /// <summary>
        /// Пересчитать соседние ячейки.
        /// </summary>
        /// <param name="unvisitedCells">Непосещённые ячейки лаибиринта.</param>
        /// <param name="mazeWidth">Ширина лабиринта.</param>
        /// <param name="mazeHeight">Высота лабиринта.</param>
        public void EnumNeighbours(Dictionary<Vector2Int, CellData> unvisitedCells,  int mazeWidth, int mazeHeight)
        {
            UnvisitedNeighbours.Clear();
            //слева
            Vector2Int left = new Vector2Int(Position.x - 1, Position.y);
            if (Position.x > 0 && unvisitedCells.ContainsKey(left)) UnvisitedNeighbours.Add(left);
            //снизу
            Vector2Int bottom = new Vector2Int(Position.x, Position.y - 1);
            if (Position.y > 0 && unvisitedCells.ContainsKey(bottom)) UnvisitedNeighbours.Add(bottom);
            //справа
            Vector2Int right = new Vector2Int(Position.x + 1, Position.y);
            if (Position.x < mazeWidth - 1 && unvisitedCells.ContainsKey(right)) UnvisitedNeighbours.Add(right);
            //сверху
            Vector2Int top = new Vector2Int(Position.x, Position.y + 1);
            if (Position.y < mazeHeight - 1 && unvisitedCells.ContainsKey(top)) UnvisitedNeighbours.Add(top);
        }

        /// <summary>
        /// Получиыть случайного непосещённого соседа.
        /// </summary>
        public Vector2Int GetRandomNeighbour(System.Random rand)
        {
            return UnvisitedNeighbours[rand.Next(0, UnvisitedNeighbours.Count)];
        }

        /// <summary>
        /// Удалить стенку лабиринта.
        /// </summary>
        /// <param name="dir">Направление, в котором нужно удалять стенку.</param>
        public void RemoveWall(Vector2Int dir)
        {
            if (dir.x > Position.x)
                _rightWall = false;
            else if (dir.x < Position.x)
                _leftWall = false;
            else if (dir.y > Position.y)
                _topWall = false;
            else if (dir.y < Position.y)
                _bottomWall = false;
        }

        /// <summary>
        /// Перестроить стенки визуального представления ячейки. 
        /// </summary>
        /// <param name="v"></param>
        public void RebuildWalls(bool v)
        {
            _needRebuildWalls = v;
            OnNeedRebuildWalls?.Invoke();
        }
    }
}
