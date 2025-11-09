using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathFindEventWrapper))]
public class AStarPathfinder : MonoBehaviour
{
    private class PriorityQueue<T>
    {
        private List<(T item, float priority)> elements = new();

        public int Count => elements.Count;

        public void Enqueue(T item, float priority)
        {
            elements.Add((item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;
            for (int i = 1; i < elements.Count; i++)
            {
                if (elements[i].priority < elements[bestIndex].priority)
                    bestIndex = i;
            }

            T bestItem = elements[bestIndex].item;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }

        public bool Contains(T item)
        {
            return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.item, item));
        }
    }

    public delegate void PathFindReceived(List<Vector2Int> UnitCoordinate);
    public event PathFindReceived OnPathFound;

    public void FindPath(Vector2Int start, Vector2Int goal, Dictionary<Vector2Int, Tile> grid)
    {
        var openSet = new PriorityQueue<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var gScore = new Dictionary<Vector2Int, float>();
        var fScore = new Dictionary<Vector2Int, float>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();

            if (current == goal)
            {
                OnPathFound.Invoke(ReconstructPath(cameFrom, current));
                
                return;
            }
            

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (!grid.ContainsKey(neighbor) || !grid[neighbor].GetIsValidForPathFinding())
                    continue;

                float tentativeG = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }
        return; // No path found
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance (grid-based)
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        return new List<Vector2Int>
        {
            pos + Vector2Int.up,
            pos + Vector2Int.down,
            pos + Vector2Int.left,
            pos + Vector2Int.right
        };
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var path = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
}


