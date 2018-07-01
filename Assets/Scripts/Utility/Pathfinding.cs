using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour {


    public List<Point> FindPath(Point start, Point target, int mapMaxSize)
    {

        Heap<Point> openSet = new Heap<Point>(mapMaxSize);
        HashSet<Point> closedSet = new HashSet<Point>();

        openSet.Add(start);

        while(openSet.Count > 0)
        {
            Point currentPoint = openSet.RemoveFirst();
            closedSet.Add(currentPoint);

            if(currentPoint == target)
            {
                return RetracePath(start, target);
            }

            foreach (var neighbour in currentPoint.currentPathDestinations)
            {
                if (closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentPoint.gCost + GetDistance(currentPoint, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, target);
                    neighbour.parent = currentPoint;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);  
                    }
                }
            }
        }

        return null;
    }

    List<Point> RetracePath(Point start, Point end)
    {
        List<Point> path = new List<Point>();
        Point currentPoint = end;

        while(currentPoint != start)
        {
            path.Add(currentPoint);
            currentPoint = currentPoint.parent;
        }

        path.Reverse();
        return path;
    }

    public int GetDistance (Point pointA, Point pointB)
    {
        int xDst = Mathf.Abs(pointA.x - pointB.x);
        int yDst = Mathf.Abs(pointA.y - pointB.y);

        if (xDst > yDst)
            return 14 * yDst + 10 * (xDst - yDst);
        return 14 * xDst + 10 * (yDst - xDst);
    }

}
