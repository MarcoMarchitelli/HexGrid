using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hexagon {

    public int x, y;
    public Vector3 worldPosition;
    public Type type;

    public enum Type
    {
        empty, energy, ability, win
    }

    public Hexagon(int _x, int _y, Vector3 _worldPosition, Type _type)
    {
        x = _x;
        y = _y;
        worldPosition = _worldPosition;
        type = _type;
    }

}
