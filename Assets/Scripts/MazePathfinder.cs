using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    internal class MazePathfinder
    {
        private readonly float _cellSize;
        private readonly Transform _player;
        private readonly Transform _goal;

        private List<Vector2Int> _currentPath = new();
        private Vector2Int _playerLastPosition;
        private Vector3 _lastPositon;

        public MazePathfinder(float cellSize, Transform player, Transform goal)
        {
            _cellSize = cellSize;
            _player = player;
            _goal = goal;
        }

        public void PlayerResetPosition() =>
            _playerLastPosition = Vector2Int.down;

        public Vector3 FindClosestPoint(int[,] mazeGrid)
        {
            DebugDrawPath();
            Vector2Int start = ParseWorldToGridPosition(_player.transform.position);

            if (_playerLastPosition == start)
                return _lastPositon;

            Vector2Int goalPos = ParseWorldToGridPosition(_goal.transform.position);
            _playerLastPosition = start;

            if (!IsWalkable(mazeGrid, start) || !IsWalkable(mazeGrid, goalPos))
                return ParseGridToWorldPosition(goalPos);

            List<Vector2Int> newPath = Algorithms.FindPathAStar(start, goalPos, mazeGrid, IsWalkable);

            if (newPath.Count > 0)
                _currentPath = newPath;
            else
                _currentPath.Clear();

            if (newPath.Count != 0)
                _currentPath = newPath;

            NextPathPoint();
            return _lastPositon;
        }

        private Vector2Int ParseWorldToGridPosition(Vector3 position) =>
            new(Mathf.RoundToInt(position.x / _cellSize),
                Mathf.RoundToInt(position.z / _cellSize));

        private Vector3 ParseGridToWorldPosition(Vector2Int gridPos) =>
            new(gridPos.x * _cellSize, 0, gridPos.y * _cellSize);

        private bool IsWalkable(int[,] grid, Vector2Int position)
        {
            if (position.x < 0 || position.x >= grid.GetLength(0) ||
                position.y < 0 || position.y >= grid.GetLength(1))
                return false;

            return grid[position.x, position.y] != (int)MazePart.Wall;
        }

        private void NextPathPoint()
        {
            if (_currentPath == null || _currentPath.Count == 0)
            {
                _lastPositon = ParseGridToWorldPosition(ParseWorldToGridPosition(_goal.transform.position));
                return;
            }

            Vector2Int currentGridPos = ParseWorldToGridPosition(_player.transform.position);
            Vector2Int nextPoint = Vector2Int.zero;

            float closestDistance = float.MaxValue;
            int closestIndex = -1;

            for (int i = 0; i < _currentPath.Count; i++)
            {
                float distance = Vector2Int.Distance(currentGridPos, _currentPath[i]);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            if (closestIndex >= 0 && closestIndex < _currentPath.Count - 1)
                nextPoint = _currentPath[closestIndex + 1];
            else if (closestIndex >= 0)
                nextPoint = _currentPath[closestIndex];
            else
                nextPoint = ParseWorldToGridPosition(_goal.transform.position);

            _lastPositon = ParseGridToWorldPosition(nextPoint);
        }

        private void DebugDrawPath()
        {
            if (_currentPath == null || _currentPath.Count == 0)
                return;

            for (int i = 0; i < _currentPath.Count - 1; i++)
                Debug.DrawLine(ParseGridToWorldPosition(_currentPath[i]), ParseGridToWorldPosition(_currentPath[i + 1]), Color.green, 2);

            if (_lastPositon != Vector3.zero)
                Debug.DrawRay(_lastPositon, Vector3.up * 2, Color.red, 2);
        }
    }
}