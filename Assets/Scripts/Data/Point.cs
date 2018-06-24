using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point : IHeapItem<Point>{

    public int x, y;
    public Vector3 worldPosition;
    [System.NonSerialized]
    public List<Point> _possibleDestinations;
    [System.NonSerialized]
    public List<Point> _currentPathDestinations = new List<Point>();

    public List<Point> possibleDestinations
    {
        get
        {
            return _possibleDestinations;
        }
        set
        {
            _possibleDestinations = value;
        }
    }

    public List<Point> currentPathDestinations
    {
        get
        {
            return _currentPathDestinations;
        }
        set
        {
            _currentPathDestinations = value;
        }
    }

    public MaterialChange triangle;

    public List<Hexagon> nearHexagons;
    public Type type;
    public bool isStartingPoint;
    public bool isFinalWaypoint;
    public bool isFinalWaypointUsed;

    public int gCost;
    public int hCost;
    int heapIndex;
    public Point parent;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Point pointToCompare)
    {
        int compare = fCost.CompareTo(pointToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(pointToCompare.hCost);
        }
        return -compare;
    }

    public enum Type
    {
        underwater, underground, forest, hypogeum, grey, purple, win
    }

    public Point(int _x, int _y, Vector3 _worldPosition)
    {
        x = _x;
        y = _y;
        worldPosition = _worldPosition;
        isStartingPoint = false;
        isFinalWaypoint = false;
        isFinalWaypointUsed = false;
        possibleDestinations = new List<Point>();
        nearHexagons = new List<Hexagon>();
        SetTypeFromCoords();
    }

    public Point(int _x, int _y, Vector3 _worldPosition, Type _type, bool _isStartingPoint)
    {
        x = _x;
        y = _y;
        worldPosition = _worldPosition;
        type = _type;
        isStartingPoint = _isStartingPoint;
        isFinalWaypoint = false;
        isFinalWaypointUsed = false;
        possibleDestinations = new List<Point>();
        nearHexagons = new List<Hexagon>();
    }

    public Point()
    {

    }

    public void SetTypeFromCoords()
    {
        switch (y)
        {
            case 6:
                if (x == 1)
                {
                    type = Type.underground;
                }
                if (x == 13)
                {
                    type = Type.underwater;
                }
                if (x == 7)
                {
                    type = Type.purple;
                }
                if(x == 3  || x == 5 || x == 9  || x == 11)
                {
                    type = Type.grey;
                }
                break;
            case 7:
                if (x == 2 || x == 8 || x == 6 || x == 12)
                {
                    type = Type.purple;
                }
                if( x == 4 || x == 10)
                {
                    type = Type.grey;
                }
                break;
            case 8:
                if (x == 2 || x == 8 || x == 6 || x == 12)
                {
                    type = Type.purple;
                }
                if (x == 4 || x == 10)
                {
                    type = Type.grey;
                }
                break;
            case 9:
                if (x == 13)
                {
                    type = Type.forest;
                }
                if (x == 1)
                {
                    type = Type.hypogeum;
                }
                if (x == 7)
                {
                    type = Type.purple;
                }
                if (x == 3 || x == 4 || x == 5 || x == 9 || x == 10 || x == 11)
                {
                    type = Type.grey;
                }
                break;
            default:
                type = Type.grey;
                break;
        }
    }

}
