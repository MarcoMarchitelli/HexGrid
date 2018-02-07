using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Point{

    public int x, y;
    public Vector3 worldPosition;
    public Movement movementType;
    public Type type;

    public enum Movement
    {
        straight, inverted
    }

    public enum Type
    {
        blue, red, green, yellow, grey, purple
    }

    public Point(int _x, int _y, Vector3 _worldPosition, Movement _movementType)
    {
        x = _x;
        y = _y;
        movementType = _movementType;
        worldPosition = _worldPosition;
        SetTypeFromCoords();
    }

    public void SetTypeFromCoords()
    {
        switch (this.y)
        {
            case 0:
                if (this.x == 4 || this.x == 6)
                {
                    this.type = Type.yellow;
                }
                if (this.x == 8 || this.x == 10)
                {
                    this.type = Type.blue;
                }
                break;
            case 1:
                if (this.x == 5)
                {
                    this.type = Type.yellow;
                }
                if (this.x == 7)
                {
                    this.type = Type.grey;
                }
                if (this.x == 9)
                {
                    this.type = Type.blue;
                }
                break;
            case 2:
                if (this.x == 3 || this.x == 5)
                {
                    this.type = Type.yellow;
                }
                if (this.x == 7)
                {
                    this.type = Type.grey;
                }
                if (this.x == 9 || this.x == 11)
                {
                    this.type = Type.blue;
                }
                break;
            case 3:
                if (this.x == 4 || this.x == 6)
                {
                    this.type = Type.yellow;
                }
                if (this.x == 8 || this.x == 10)
                {
                    this.type = Type.blue;
                }
                break;
            case 4:
                if (this.x == 2 || this.x == 4 || this.x == 6)
                {
                    this.type = Type.yellow;
                }
                if (this.x == 8 || this.x == 10 || this.x == 12)
                {
                    this.type = Type.blue;
                }
                break;
            case 5:
                if (this.x == 3 || this.x == 5)
                {
                    this.type = Type.yellow;
                }
                if (this.x == 7)
                {
                    this.type = Type.grey;
                }
                if (this.x == 9 || this.x == 11)
                {
                    this.type = Type.blue;
                }
                break;
            case 6:
                if (this.x == 1 || this.x == 5 || this.x == 3)
                {
                    this.type = Type.green;
                }
                if (this.x == 9 || this.x == 11 || this.x == 13)
                {
                    this.type = Type.red;
                }
                if (this.x == 7)
                {
                    this.type = Type.purple;
                }
                break;
            case 7:
                if (this.x == 2 || this.x == 8 || this.x == 6 || this.x == 12)
                {
                    this.type = Type.purple;
                }
                if (this.x == 10)
                {
                    this.type = Type.red;
                }
                if (this.x == 4)
                {
                    this.type = Type.green;
                }
                break;
            case 8:
                if (this.x == 2 || this.x == 8 || this.x == 6 || this.x == 12)
                {
                    this.type = Type.purple;
                }
                if (this.x == 10)
                {
                    this.type = Type.yellow;
                }
                break;
            case 9:
                if (this.x == 9 || this.x == 11 || this.x == 13)
                {
                    this.type = Type.yellow;
                }
                if (this.x == 7)
                {
                    this.type = Type.purple;
                }
                break;
            case 10:
                if (this.x == 3 || this.x == 5)
                {
                    this.type = Type.red;
                }
                if (this.x == 7)
                {
                    this.type = Type.grey;
                }
                if (this.x == 9 || this.x == 11)
                {
                    this.type = Type.green;
                }
                break;
            case 11:
                if (this.x == 2 || this.x == 4 || this.x == 6)
                {
                    this.type = Type.red;
                }
                if (this.x == 8 || this.x == 10 || this.x == 12)
                {
                    this.type = Type.green;
                }
                break;
            case 12:
                if (this.x == 4 || this.x == 6)
                {
                    this.type = Type.red;
                }
                if (this.x == 8 || this.x == 10)
                {
                    this.type = Type.green;
                }
                break;
            case 13:
                if (this.x == 3 || this.x == 5)
                {
                    this.type = Type.red;
                }
                if (this.x == 7)
                {
                    this.type = Type.grey;
                }
                if (this.x == 9 || this.x == 11)
                {
                    this.type = Type.green;
                }
                break;
            case 14:
                if (this.x == 5)
                {
                    this.type = Type.red;
                }
                if (this.x == 7)
                {
                    this.type = Type.grey;
                }
                if (this.x == 9)
                {
                    this.type = Type.green;
                }
                break;
            case 15:
                if (this.x == 4 || this.x == 6)
                {
                    this.type = Type.red;
                }
                if (this.x == 8 || this.x == 10)
                {
                    this.type = Type.green;
                }
                break;
        }
    }
}
