using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point{

    public int x, y;
    public Vector3 worldPosition;
    public List<Point> possibleDestinations;
    public List<Hexagon> nearHexagons;
    public Type type;
    public bool isStartingPoint;
    public bool isFinalWaypoint;

    public int gCost;
    public int hCost;
    public Point parent;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
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
            case 0:
                if (x == 4 || x == 6)
                {
                    type = Type.hypogeum;
                }
                if (x == 8 || x == 10)
                {
                    type = Type.underwater;
                }
                break;
            case 1:
                if (x == 5)
                {
                    type = Type.hypogeum;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9)
                {
                    type = Type.underwater;
                }
                break;
            case 2:
                if (x == 3 || x == 5)
                {
                    type = Type.hypogeum;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9 || x == 11)
                {
                    type = Type.underwater;
                }
                break;
            case 3:
                if (x == 4 || x == 6)
                {
                    type = Type.hypogeum;
                }
                if (x == 8 || x == 10)
                {
                    type = Type.underwater;
                }
                break;
            case 4:
                if (x == 2 || x == 4 || x == 6)
                {
                    type = Type.hypogeum;
                }
                if (x == 8 || x == 10 || x == 12)
                {
                    type = Type.underwater;
                }
                break;
            case 5:
                if (x == 3 || x == 5)
                {
                    type = Type.hypogeum;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9 || x == 11)
                {
                    type = Type.underwater;
                }
                break;
            case 6:
                if (x == 1 || x == 5 || x == 3)
                {
                    type = Type.forest;
                }
                if (x == 9 || x == 11 || x == 13)
                {
                    type = Type.underground;
                }
                if (x == 7)
                {
                    type = Type.purple;
                }
                break;
            case 7:
                if (x == 2 || x == 8 || x == 6 || x == 12)
                {
                    type = Type.purple;
                }
                if (x == 10)
                {
                    type = Type.underground;
                }
                if (x == 4)
                {
                    type = Type.forest;
                }
                break;
            case 8:
                if (x == 2 || x == 8 || x == 6 || x == 12)
                {
                    type = Type.purple;
                }
                if (x == 10)
                {
                    type = Type.hypogeum;
                }
                break;
            case 9:
                if (x == 9 || x == 11 || x == 13)
                {
                    type = Type.hypogeum;
                }
                if (x == 7)
                {
                    type = Type.purple;
                }
                break;
            case 10:
                if (x == 3 || x == 5)
                {
                    type = Type.underground;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9 || x == 11)
                {
                    type = Type.forest;
                }
                break;
            case 11:
                if (x == 2 || x == 4 || x == 6)
                {
                    type = Type.underground;
                }
                if (x == 8 || x == 10 || x == 12)
                {
                    type = Type.forest;
                }
                break;
            case 12:
                if (x == 4 || x == 6)
                {
                    type = Type.underground;
                }
                if (x == 8 || x == 10)
                {
                    type = Type.forest;
                }
                break;
            case 13:
                if (x == 3 || x == 5)
                {
                    type = Type.underground;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9 || x == 11)
                {
                    type = Type.forest;
                }
                break;
            case 14:
                if (x == 5)
                {
                    type = Type.underground;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9)
                {
                    type = Type.forest;
                }
                break;
            case 15:
                if (x == 4 || x == 6)
                {
                    type = Type.underground;
                }
                if (x == 8 || x == 10)
                {
                    type = Type.forest;
                }
                break;
        }
    }
}
