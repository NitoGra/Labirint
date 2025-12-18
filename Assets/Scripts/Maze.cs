using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    internal class Maze : MonoBehaviour
    {
        private const float FloorHeight = 0.05f;

        [SerializeField] private int _width = 10;
        [SerializeField] private int _height = 10;
        [SerializeField] private float _wallHeight = 2f;
        [SerializeField] private float _cellSize = 2;

        private int[,] _mazeGrid;
        private Vector2Int _goalPosition;
        private MazePathfinder _mazePathfinder;
        private readonly List<Vector2Int> _freeCells = new();

        public void Init(Transform playerTransform, TargetGoal goal)
        {
            goal.FoundTarget += PlaceGoal;
            _mazePathfinder = new(_cellSize, playerTransform.transform, goal.transform);
            _mazeGrid = Algorithms.GenerateMazeByPrim(_width, _height);

            UpdateFreeCellsList();
            SpawnMaze(playerTransform);
            PlaceGoal(goal.transform);
        }

        public Vector3 FindClosestPoint() =>
            _mazePathfinder.FindClosestPoint(_mazeGrid);

        private void SpawnMaze(Transform playerTransform)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            playerTransform.position = Vector3.right * _cellSize;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    GameObject mazePart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mazePart.transform.parent = transform;
                    mazePart.transform.position = new(x * _cellSize, 0, y * _cellSize);

                    if (_mazeGrid[x, y] == (int)MazePart.Wall)
                    {
                        mazePart.transform.localScale = new Vector3(_cellSize, _wallHeight, _cellSize);
                        mazePart.name = $"Wall_{x}_{y}";
                    }
                    else
                    {
                        mazePart.transform.localPosition = new(mazePart.transform.localPosition.x, -_wallHeight / 2, mazePart.transform.localPosition.z);
                        mazePart.transform.localScale = new Vector3(_cellSize, FloorHeight, _cellSize);
                        mazePart.name = $"Floor_{x}_{y}";
                    }
                }
            }
        }

        private void PlaceGoal(Transform goalTransform)
        {
            if (_mazeGrid[_goalPosition.x, _goalPosition.y] == (int)MazePart.Goal)
                _mazeGrid[_goalPosition.x, _goalPosition.y] = (int)MazePart.Floor;

            _goalPosition = _freeCells[Random.Range(0, _freeCells.Count)];
            _mazeGrid[_goalPosition.x, _goalPosition.y] = (int)MazePart.Goal;
            goalTransform.position = new(_goalPosition.x * _cellSize, 0, _goalPosition.y * _cellSize);

            UpdateFreeCellsList();
            _mazePathfinder.PlayerResetPosition();
        }

        private void UpdateFreeCellsList()
        {
            _freeCells.Clear();

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    if (_mazeGrid[x, y] == (int)MazePart.Floor)
                        _freeCells.Add(new Vector2Int(x, y));
        }
    }
}