using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    internal static class Algorithms
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

        internal static int[,] GenerateMazeByPrim(int width, int height)
        {
            int[,] mazeGrid = new int[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    mazeGrid[x, y] = 1;

            int startX = 1;
            int startY = 1;
            mazeGrid[startX, startY] = 0;
            List<Edge> walls = new();

            if (startX > 1)
                walls.Add(new Edge(startX, startY, startX - 1, startY));

            if (startX < width - 2)
                walls.Add(new Edge(startX, startY, startX + 1, startY));

            if (startY > 1)
                walls.Add(new Edge(startX, startY, startX, startY - 1));

            if (startY < height - 2)
                walls.Add(new Edge(startX, startY, startX, startY + 1));

            while (walls.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, walls.Count);
                Edge wall = walls[randomIndex];
                walls.RemoveAt(randomIndex);

                int oppositeX = wall.X2 + (wall.X2 - wall.X1);
                int oppositeY = wall.Y2 + (wall.Y2 - wall.Y1);

                if (oppositeX >= 0 && oppositeX < width && oppositeY >= 0 && oppositeY < height)
                {
                    if (mazeGrid[oppositeX, oppositeY] == 1)
                    {
                        mazeGrid[wall.X2, wall.Y2] = 0;
                        mazeGrid[oppositeX, oppositeY] = 0;

                        if (oppositeX > 1 && mazeGrid[oppositeX - 2, oppositeY] == 1)
                            walls.Add(new Edge(oppositeX, oppositeY, oppositeX - 1, oppositeY));

                        if (oppositeX < width - 2 && mazeGrid[oppositeX + 2, oppositeY] == 1)
                            walls.Add(new Edge(oppositeX, oppositeY, oppositeX + 1, oppositeY));

                        if (oppositeY > 1 && mazeGrid[oppositeX, oppositeY - 2] == 1)
                            walls.Add(new Edge(oppositeX, oppositeY, oppositeX, oppositeY - 1));

                        if (oppositeY < height - 2 && mazeGrid[oppositeX, oppositeY + 2] == 1)
                            walls.Add(new Edge(oppositeX, oppositeY, oppositeX, oppositeY + 1));
                    }
                }
            }

            mazeGrid[1, 0] = 0;
            mazeGrid[width - 2, height - 1] = 0;
            return mazeGrid;
        }

        internal class Edge
        {
            public int X1;
            public int X2;

            public int Y1;
            public int Y2;

            public Edge(int x1, int y1, int x2, int y2)
            {
                X1 = x1;
                X2 = x2;

                Y1 = y1;
                Y2 = y2;
            }
        }

        internal class Node
        {
            public Vector2Int Position { get; }
            public float GCost { get; set; }        //cost from start
            public float HCost { get; }             //cost to target
            public float FCost => GCost + HCost;    //total cost
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