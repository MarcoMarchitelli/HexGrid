using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Point{

    public int x, y;
    public Vector3 worldPosition;
    public Movement type;

    public enum Movement
    {
        straight, inverted
    }

    public Point(int _x, int _y, Vector3 _worldPosition, Movement _type)
    {
        x = _x;
        y = _y;
        type = _type;
        worldPosition = _worldPosition;
    }
}
