using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hexagon {

    public int x, y;
    public Vector3 worldPosition;
    public Transform card;
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

    public void setType(Type _type)
    {
        type = _type;
    }

    public void SetTypeFromCoords(Vector2 _gridSize)
    {
        if (this.y == 0 || this.y == (int)_gridSize.y -1)
        {
            if (this.x == 3 || this.x == 4 )
            {
                this.type = Type.energy;
            }
        }
        if (this.y == 1 || this.y == (int)_gridSize.y - 2)
        {
            if (this.x == 1 || this.x == 2 || this.x == 4 || this.x == 5)
            {
                this.type = Type.energy;
            }
        }
        if (this.y == 2 || this.y == (int)_gridSize.y - 3)
        {
            if (this.x == 1 || this.x == 3 || this.x == 4 || this.x == 6)
            {
                this.type = Type.energy;
            }
        }
        if (this.y == ((int)_gridSize.y - 1) / 2)
        {
            if (this.x == 1 || this.x == 5)
            {
                this.type = Type.energy;
            }
            if (this.x == 2 || this.x == 4)
            {
                this.type = Type.ability;
            }
            if (this.x == ((int)_gridSize.x - 1) / 2)
            {
                this.type = Type.win;
            }
        }
    }
}