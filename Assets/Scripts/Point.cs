using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Point{

    public int x, y;
    public Vector3 worldPosition;
    public List<Point> possibleDestinations;
    public Type type;
    public bool isStartingPoint;

    public enum Type
    {
        blue, red, green, yellow, grey, purple, win
    }

    public Point(int _x, int _y, Vector3 _worldPosition)
    {
        x = _x;
        y = _y;
        worldPosition = _worldPosition;
        isStartingPoint = false;
        possibleDestinations = new List<Point>();
        SetTypeFromCoords();
    }

    public Point(int _x, int _y, Vector3 _worldPosition, Type _type, bool _isStartingPoint)
    {
        x = _x;
        y = _y;
        worldPosition = _worldPosition;
        type = _type;
        isStartingPoint = _isStartingPoint;
        possibleDestinations = new List<Point>();
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
                    type = Type.yellow;
                }
                if (x == 8 || x == 10)
                {
                    type = Type.blue;
                }
                break;
            case 1:
                if (x == 5)
                {
                    type = Type.yellow;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9)
                {
                    type = Type.blue;
                }
                break;
            case 2:
                if (x == 3 || x == 5)
                {
                    type = Type.yellow;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9 || x == 11)
                {
                    type = Type.blue;
                }
                break;
            case 3:
                if (x == 4 || x == 6)
                {
                    type = Type.yellow;
                }
                if (x == 8 || x == 10)
                {
                    type = Type.blue;
                }
                break;
            case 4:
                if (x == 2 || x == 4 || x == 6)
                {
                    type = Type.yellow;
                }
                if (x == 8 || x == 10 || x == 12)
                {
                    type = Type.blue;
                }
                break;
            case 5:
                if (x == 3 || x == 5)
                {
                    type = Type.yellow;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9 || x == 11)
                {
                    type = Type.blue;
                }
                break;
            case 6:
                if (x == 1 || x == 5 || x == 3)
                {
                    type = Type.green;
                }
                if (x == 9 || x == 11 || x == 13)
                {
                    type = Type.red;
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
                    type = Type.red;
                }
                if (x == 4)
                {
                    type = Type.green;
                }
                break;
            case 8:
                if (x == 2 || x == 8 || x == 6 || x == 12)
                {
                    type = Type.purple;
                }
                if (x == 10)
                {
                    type = Type.yellow;
                }
                break;
            case 9:
                if (x == 9 || x == 11 || x == 13)
                {
                    type = Type.yellow;
                }
                if (x == 7)
                {
                    type = Type.purple;
                }
                break;
            case 10:
                if (x == 3 || x == 5)
                {
                    type = Type.red;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9 || x == 11)
                {
                    type = Type.green;
                }
                break;
            case 11:
                if (x == 2 || x == 4 || x == 6)
                {
                    type = Type.red;
                }
                if (x == 8 || x == 10 || x == 12)
                {
                    type = Type.green;
                }
                break;
            case 12:
                if (x == 4 || x == 6)
                {
                    type = Type.red;
                }
                if (x == 8 || x == 10)
                {
                    type = Type.green;
                }
                break;
            case 13:
                if (x == 3 || x == 5)
                {
                    type = Type.red;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9 || x == 11)
                {
                    type = Type.green;
                }
                break;
            case 14:
                if (x == 5)
                {
                    type = Type.red;
                }
                if (x == 7)
                {
                    type = Type.grey;
                }
                if (x == 9)
                {
                    type = Type.green;
                }
                break;
            case 15:
                if (x == 4 || x == 6)
                {
                    type = Type.red;
                }
                if (x == 8 || x == 10)
                {
                    type = Type.green;
                }
                break;
        }
    }
}
