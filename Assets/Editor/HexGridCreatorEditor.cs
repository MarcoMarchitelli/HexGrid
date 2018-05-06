using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGridCreator))]
public class HexGridCreatorEditor : Editor
{

    HexGridCreator hexGrid;

    void OnEnable()
    {
        hexGrid = target as HexGridCreator;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //hexGrid.CreateGrid();
    }
}
