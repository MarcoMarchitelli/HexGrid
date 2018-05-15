using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotDraw : MonoBehaviour {

    public float size = 1.0f;
    public Color color = Color.green;


    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, size);
    }
}
