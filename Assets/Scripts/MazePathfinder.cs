using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    internal class MazePathfinder
    {
        readonly private float _cellSize;
        readonly private Transform _player;
        readonly private Transform _goal;

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

            if (_playerLastPosition == WorldToGridPosition(_player.transform.position))
                return _lastPositon;

            Vector2Int start = WorldToGridPosition(_player.transform.position);
            Vector2Int goalPos = WorldToGridPosition(_goal.transform.position);
            _playerLastPosition = start;

            if (!IsWalkable(mazeGrid, start)|| !IsWalkable(mazeGrid, goalPos))
                return GridToWorldPosition(goalPos);

            List<Vector2Int> newPath = FindPathAStar(start, goalPos, mazeGrid);

            if (newPath.Count > 0)
                _currentPath = newPath;
            else
                _currentPath.Clear();

            if (newPath.Count != 0)
                _currentPath = newPath;

            NextPathPoint();
            return _lastPositon;
        }

        private Vector2Int WorldToGridPosition(Vector3 position) =>
            new(Mathf.RoundToInt(position.x / _cellSize),
                Mathf.RoundToInt(position.z / _cellSize));

        private Vector3 GridToWorldPosition(Vector2Int gridPos) =>
            new(gridPos.x * _cellSize, 0, gridPos.y * _cellSize);

        private List<Vector2Int> FindPathAStar(Vector2Int start, Vector2Int goal, int[,] grid)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            List<Node> openSet = new();
            HashSet<Vector2Int> closedSet = new();

            Node startNode = new(start, 0, Heuristic(start, goal), null);
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                for (int i = 1; i < openSet.Count; i++)
                    if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                        currentNode = openSet[i];

                if (currentNode.Position == goal)
                    return RetracePath(currentNode);

                openSet.Remove(currentNode);
                closedSet.Add(currentNode.Position);

                foreach (Vector2Int direction in _directions)
                {
                    Vector2Int neighborPos = currentNode.Position + direction;

                    if (!IsWalkable(grid, neighborPos) || closedSet.Contains(neighborPos))
                        continue;

                    float newMovementCost = currentNode.GCost + Vector2Int.Distance(currentNode.Position, neighborPos);
                    Node neighborNode = openSet.Find(n => n.Position == neighborPos);

                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighborPos, newMovementCost, Heuristic(neighborPos, goal), currentNode);
                        openSet.Add(neighborNode);
                    }
                    else if (newMovementCost < neighborNode.GCost)
                    {
                        neighborNode.GCost = newMovementCost;
                        neighborNode.Parent = currentNode;
                    }
                }
            }

            Debug.LogError("Empty");
            return new List<Vector2Int>();
        }

        private bool IsWalkable(int[,] grid, Vector2Int position)
        {
            if (position.x < 0 || position.x >= grid.GetLength(0) ||
                position.y < 0 || position.y >= grid.GetLength(1))
                return false;

            return grid[position.x, position.y] == (int)MazeParts.Floor || grid[position.x, position.y] == (int)MazeParts.Goal;
        }

        private float Heuristic(Vector2Int a, Vector2Int b) =>
            Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

        private List<Vector2Int> RetracePath(Node endNode)
        {
            List<Vector2Int> path = new();
            Node currentNode = endNode;

            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private void NextPathPoint()
        {
            if (_currentPath == null || _currentPath.Count == 0)
            {
                _lastPositon = GridToWorldPosition(WorldToGridPosition(_goal.transform.position));
                return;
            }

            Vector2Int currentGridPos = WorldToGridPosition(_player.transform.position);
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
                nextPoint = WorldToGridPosition(_goal.transform.position);

            _lastPositon = GridToWorldPosition(nextPoint);
        }

        private class Node
        {
            public Vector2Int Position { get; }
            public float GCost { get; set; }        //cтоимость от старта
            public float HCost { get; }             //стоимость до цели
            public float FCost => GCost + HCost;    //общая стоимость
            public Node Parent { get; set; }

            public Node(Vector2Int position, float gCost, float hCost, Node parent)
            {
                Position = position;
                GCost = gCost;
                HCost = hCost;
                Parent = parent;
            }
        }

        private static readonly Vector2Int[] _directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        public void DebugDrawPath()
        {
            if (_currentPath == null || _currentPath.Count == 0) 
                return;

            for (int i = 0; i < _currentPath.Count - 1; i++)
                Debug.DrawLine(GridToWorldPosition(_currentPath[i]), GridToWorldPosition(_currentPath[i + 1]), Color.green, 2);

            if (_lastPositon != Vector3.zero)
                Debug.DrawRay(_lastPositon, Vector3.up * 2, Color.red, 2);
        }
    }
}