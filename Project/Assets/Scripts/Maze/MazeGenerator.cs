using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceShooter
{
    public class MazeGenerator : MonoBehaviour
    {
        /// <summary>
        /// Ширина лабиринта, ячеек.
        /// </summary>
        [SerializeField] private int _width;

        /// <summary>
        /// Высота лабиринта, ячеек.
        /// </summary>
        [SerializeField] private int _height;

        /// <summary>
        /// Размер ячейки лабиринта.
        /// </summary>
        [SerializeField] private int _cellSize;

        /// <summary>
        /// Префаб клетки лаибиринта.
        /// </summary>
        [SerializeField] private Cell _cellPrefab;

        /// <summary>
        /// Префаб финиша лаибиринта.
        /// </summary>
        [SerializeField] private MazeFinish _finishPrefab;

        /// <summary>
        /// Словарь посещённых клеток лабиринта.
        /// </summary>
        public Dictionary<Vector2Int, CellData> UnvisitedCells;

        /// <summary>
        /// Словарь непосещённых клеток лабиринта.
        /// </summary>
        public Dictionary<Vector2Int, CellData> VisitedCells;

        private void Start()
        {
            //проинициализировать словари
            UnvisitedCells = new Dictionary<Vector2Int, CellData>();
            VisitedCells = new Dictionary<Vector2Int, CellData>();

            //создать словарь всех ячеек лабиринта
            Vector2Int position = Vector2Int.zero; //начать из позиции (0, 0)
            CellData start = null;
            int center = (_width * _height + _width) / 2;
            for (int i = 0; i < _width * _height; i++)
            {
                //создать ячейку лабиринта
                UnvisitedCells.Add(position, new CellData(position));
                //пересчитать соседние ячейки
                UnvisitedCells[position].EnumNeighbours(VisitedCells, _width, _height);
                //создать визуальное представление ячейки лабиринта
                GameObject newGo = Instantiate(_cellPrefab.gameObject, new Vector3(position.x * _cellSize, position.y * _cellSize, 0), Quaternion.identity, transform);
                Vector3 scale = newGo.transform.localScale;
                newGo.transform.localScale = new Vector3(scale.x * _cellSize, scale.y * _cellSize, scale.z);
                Cell newCell = newGo.GetComponent<Cell>();
                newCell.Data = UnvisitedCells[position];
                if(i == center) start = UnvisitedCells[position];
                //перейти к следующей ячейке
                if (position.x == _width - 1)
                {
                    position.x = 0;
                    position.y++;
                }
                else position.x++;
            }

            GameObject finishGo = Instantiate(_finishPrefab.gameObject, new Vector3((_width - 1) * _cellSize, (_height - 1) * _cellSize, 0), Quaternion.identity, transform);
            Vector3 finishScale = finishGo.transform.localScale;
            finishGo.transform.localScale = new Vector3(finishScale.x * _cellSize, finishScale.y * _cellSize, finishScale.z);

            //сгенерировать лабиринт
            //GenerateMaze(UnvisitedCells.Values.First());
            GenerateMaze(start);
        }

        /// <summary>
        /// Генерация лабиринта.
        /// </summary>
        /// <param name="start">Начальная ячейка, с которой будет сгенерирован лабиринт.</param>
        private void GenerateMaze(CellData start)
        {
            if (start == null)
            {
                Debug.LogError("Ошибка! Не задана стартовая ячейка лабиринта.");
                return;
            }

            CellData currentCell = start;
            CellData previousCell = new CellData(new Vector2Int(-1, -1));
            currentCell.EnumNeighbours(UnvisitedCells, _width, _height);
            System.Random random = new System.Random();

            while (UnvisitedCells.Count > 0)
            {
                UnvisitedCells.Remove(currentCell.Position);
                VisitedCells.Add(currentCell.Position, currentCell);

                //пересчитать соседние клетки
                currentCell.EnumNeighbours(UnvisitedCells, _width, _height);

                //если предыдущая клетка существует - убрать стенку между ней и текущей клеткой
                if (previousCell.Position != new Vector2Int(-1, -1))
                {
                    currentCell.RemoveWall(previousCell.Position);
                    currentCell.RebuildWalls(true);
                }

                if (currentCell.HasUnvisitedNeighbour)
                {
                    //у текущей клетки есть непосещённые соседи
                    //выбрать случайного непосещённого соседа в качестве следующей клетки
                    CellData nextCell = UnvisitedCells[currentCell.GetRandomNeighbour(random)];
                    nextCell.EnumNeighbours(UnvisitedCells, _width, _height);
                    //удалить стенку между текущей клеткой и следующей
                    currentCell.RemoveWall(nextCell.Position);
                    currentCell.RebuildWalls(true);
                    //сделать текущую клетку предыдущей
                    previousCell = currentCell;
                    //сделать следующую клетку текущей
                    currentCell = nextCell;
                }
                else if (UnvisitedCells.Count > 0)
                {
                    //у текущей клетки больше нет непосещённых соседей, следовательно для дальнейшей генерации она бесполезна
                    VisitedCells.Remove(currentCell.Position);
                    //просмотреть уже посещённые клетки
                    foreach (KeyValuePair<Vector2Int, CellData> pair in VisitedCells)
                    {
                        //пересчитать соседей
                        pair.Value.EnumNeighbours(UnvisitedCells, _width, _height);
                        //если среди соседей нет непосещённых - перейти к следующей клетке
                        if(!pair.Value.HasUnvisitedNeighbour) continue;
                        //выбрать найденную клетку с непосещённым соседом в качестве текущей
                        currentCell = pair.Value;
                        //выбрать случайного непосещённого соседа в качестве следующей клетки
                        CellData nextCell = UnvisitedCells[currentCell.GetRandomNeighbour(random)];
                        nextCell.EnumNeighbours(UnvisitedCells, _width, _height);
                        //удалить стенку между текущей клеткой и следующей
                        currentCell.RemoveWall(nextCell.Position);
                        currentCell.RebuildWalls(true);
                        //сделать текущую клетку предыдущей
                        previousCell = currentCell;
                        //сделать следующую клетку текущей
                        currentCell = nextCell;
                    }
                }
            }
        }
    }
}
