using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour {


    public List<Point> FindPath(Point start, Point target)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        List<Point> openSet = new List<Point>();
        HashSet<Point> closedSet = new HashSet<Point>();

        openSet.Add(start);

        while(openSet.Count > 0)
        {
            Point currentPoint = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentPoint.fCost || openSet[i].fCost == currentPoint.fCost && openSet[i].hCost == currentPoint.hCost)
                {
                    currentPoint = openSet[i];
                }
            }

            openSet.Remove(currentPoint);
            closedSet.Add(currentPoint);

            if(currentPoint == target)
            {
                sw.Stop();
                print("Path found after " + sw.ElapsedMilliseconds + " ms.");
                return RetracePath(start, target);
            }

            foreach (var neighbour in currentPoint.possibleDestinations)
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
