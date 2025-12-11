using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    internal class Maze : MonoBehaviour
    {
        [SerializeField] private int _width = 10;
        [SerializeField] private int _height = 10;
        [SerializeField] private float _cellSize = 2f;
        [SerializeField] private float _wallHeight = 2f;
        [Space]
        [SerializeField] private TargetGoal _goal;
        [SerializeField] private Player _player;

        private int[,] _mazeGrid;
        private Vector2Int _goalPosition;
        private List<Vector2Int> _freeCells = new();

        private void Start()
        {
            GenerateMaze();
            SpawnMaze();
            PlaceGoal();
        }

        private void GenerateMaze()
        {
            _mazeGrid = new int[_width, _height];

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    _mazeGrid[x, y] = 1;

            int startX = 1;
            int startY = 1;
            _mazeGrid[startX, startY] = 0;
            List<Edge> walls = new();

            if (startX > 1)
                walls.Add(new Edge(startX, startY, startX - 1, startY));

            if (startX < _width - 2)
                walls.Add(new Edge(startX, startY, startX + 1, startY));

            if (startY > 1)
                walls.Add(new Edge(startX, startY, startX, startY - 1));

            if (startY < _height - 2)
                walls.Add(new Edge(startX, startY, startX, startY + 1));

            while (walls.Count > 0)
            {
                int randomIndex = Random.Range(0, walls.Count);
                Edge wall = walls[randomIndex];
                walls.RemoveAt(randomIndex);

                int oppositeX = wall.x2 + (wall.x2 - wall.x1);
                int oppositeY = wall.y2 + (wall.y2 - wall.y1);

                if (oppositeX >= 0 && oppositeX < _width && oppositeY >= 0 && oppositeY < _height)
                {
                    if (_mazeGrid[oppositeX, oppositeY] == 1)
                    {
                        _mazeGrid[wall.x2, wall.y2] = 0;
                        _mazeGrid[oppositeX, oppositeY] = 0;

                        if (oppositeX > 1 && _mazeGrid[oppositeX - 2, oppositeY] == 1)
                            walls.Add(new Edge(oppositeX, oppositeY, oppositeX - 1, oppositeY));

                        if (oppositeX < _width - 2 && _mazeGrid[oppositeX + 2, oppositeY] == 1)
                            walls.Add(new Edge(oppositeX, oppositeY, oppositeX + 1, oppositeY));

                        if (oppositeY > 1 && _mazeGrid[oppositeX, oppositeY - 2] == 1)
                            walls.Add(new Edge(oppositeX, oppositeY, oppositeX, oppositeY - 1));

                        if (oppositeY < _height - 2 && _mazeGrid[oppositeX, oppositeY + 2] == 1)
                            walls.Add(new Edge(oppositeX, oppositeY, oppositeX, oppositeY + 1));
                    }
                }
            }

            _mazeGrid[1, 0] = 0;
            _mazeGrid[_width - 2, _height - 1] = 0;

            UpdateFreeCellsList();
        }

        private void SpawnMaze()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            _player.transform.position = Vector3.right;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    GameObject mazePart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mazePart.transform.parent = transform;
                    mazePart.transform.position = new(x * _cellSize, 0, y * _cellSize);

                    if (_mazeGrid[x, y] == 1)
                    {
                        mazePart.transform.localScale = new Vector3(_cellSize, _wallHeight, _cellSize);
                        mazePart.name = $"Wall_{x}_{y}";
                    }
                    else
                    {
                        mazePart.transform.localPosition = new(mazePart.transform.localPosition.x, -_wallHeight / 2, mazePart.transform.localPosition.z);
                        mazePart.transform.localScale = new Vector3(_cellSize, 0.1f, _cellSize);
                        mazePart.name = $"Floor_{x}_{y}";
                    }
                }
            }
        }

        private void PlaceGoal()
        {
            int randomIndex = Random.Range(0, _freeCells.Count);
            _goalPosition = _freeCells[randomIndex];
            _mazeGrid[_goalPosition.x, _goalPosition.y] = 2;
            Vector3 position = new(_goalPosition.x * _cellSize, 0, _goalPosition.y * _cellSize);
            _goal.transform.position = position;

            UpdateFreeCellsList();
        }

        private void UpdateFreeCellsList()
        {
            _freeCells.Clear();

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    if (_mazeGrid[x, y] == 0)
                        _freeCells.Add(new Vector2Int(x, y));
        }

        public void MoveGoalToNewPosition() => PlaceGoal();
        public int[,] GetMazeGrid() => _mazeGrid;
        public Transform GetGoalTransform() => _goal.transform;

        private class Edge
        {
            public int x1, y1; // Клетка с проходом
            public int x2, y2; // Стенка

            public Edge(int x1, int y1, int x2, int y2)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
            }
        }
    }
}