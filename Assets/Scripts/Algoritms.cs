using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    internal static class Algoritms
    {
        internal static List<Vector2Int> FindPathAStar(Vector2Int start, Vector2Int goal, int[,] grid, System.Func<int[,], Vector2Int, bool> isWalkable)
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

                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighborPos = currentNode.Position + direction;

                    if (!isWalkable(grid, neighborPos) || closedSet.Contains(neighborPos))
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

        internal static float Heuristic(Vector2Int a, Vector2Int b) =>
                Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

        internal readonly static Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        internal static List<Vector2Int> RetracePath(Node endNode)
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

        internal static int[,] PrimMazeGenerator(int _width, int _height)
        {
            int[,] _mazeGrid = new int[_width, _height];

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
                int randomIndex = UnityEngine.Random.Range(0, walls.Count);
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
            return _mazeGrid;
        }

        internal class Edge
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

        internal class Node
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
    }
}