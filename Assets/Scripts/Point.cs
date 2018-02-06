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

    public Point(int _x, int _y, Vector3 _worldPosition, Movement _movementType, Type _type)
    {
        x = _x;
        y = _y;
        movementType = _movementType;
        type = _type;
        worldPosition = _worldPosition;
    }

    public void SetTypeFromCoords(Vector2 _gridSize)
    {

    }
}
