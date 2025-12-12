using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    internal class Maze : MonoBehaviour
    {
        [SerializeField] private int _width = 10;
        [SerializeField] private int _height = 10;
        [SerializeField] private float _wallHeight = 2f;
        [SerializeField] private float _ñellSize = 2;

        private TargetGoal _goal;
        private Player _player;
        private float _floorHeight = 0.05f;
        private int[,] _mazeGrid;
        private Vector2Int _goalPosition;
        private List<Vector2Int> _freeCells = new();
        private MazePathfinder _mazePathfinder;

        private void Start()
        {
            _mazePathfinder = new(_ñellSize, _player.transform, _goal.transform);
            _goal.FoundTarget += PlaceGoal;
            _mazeGrid = Algoritms.PrimMazeGenerator(_width,_height);

            UpdateFreeCellsList();
            SpawnMaze();
            PlaceGoal();
        }

        public void Init(Player player, TargetGoal goal)
        {
            _goal = goal;
            _player = player;
        }

        public Vector3 FindClosestPoint() =>
            _mazePathfinder.FindClosestPoint(_mazeGrid);

        private void SpawnMaze()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            _player.transform.position = Vector3.right * _ñellSize;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    GameObject mazePart = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mazePart.transform.parent = transform;
                    mazePart.transform.position = new(x * _ñellSize, 0, y * _ñellSize);

                    if (_mazeGrid[x, y] == (int)MazeParts.Wall)
                    {
                        mazePart.transform.localScale = new Vector3(_ñellSize, _wallHeight, _ñellSize);
                        mazePart.name = $"Wall_{x}_{y}";
                    }
                    else
                    {
                        mazePart.transform.localPosition = new(mazePart.transform.localPosition.x, -_wallHeight / 2, mazePart.transform.localPosition.z);
                        mazePart.transform.localScale = new Vector3(_ñellSize, _floorHeight, _ñellSize);
                        mazePart.name = $"Floor_{x}_{y}";
                    }
                }
            }
        }

        private void PlaceGoal()
        {
            if (_mazeGrid[_goalPosition.x, _goalPosition.y] == (int)MazeParts.Goal)
                _mazeGrid[_goalPosition.x, _goalPosition.y] = (int)MazeParts.Floor;

            _goalPosition = _freeCells[UnityEngine.Random.Range(0, _freeCells.Count)];
            _mazeGrid[_goalPosition.x, _goalPosition.y] = (int)MazeParts.Goal;
            _goal.transform.position = new(_goalPosition.x * _ñellSize, 0, _goalPosition.y * _ñellSize);

            UpdateFreeCellsList();
            _mazePathfinder.PlayerResetPosition();
        }

        private void UpdateFreeCellsList()
        {
            _freeCells.Clear();

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    if (_mazeGrid[x, y] == (int)MazeParts.Floor)
                        _freeCells.Add(new Vector2Int(x, y));
        }
    }

    internal enum MazeParts
    {
        Floor = 0,
        Wall = 1,
        Goal = 2
    }
}