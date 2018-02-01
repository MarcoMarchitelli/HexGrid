using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour {

    MeshRenderer myRenderer;

    public Material newMaterial;
    Material oldMaterial;

	void Awake () {
        myRenderer = GetComponent<MeshRenderer>();
        oldMaterial = myRenderer.material;
    }

    private void OnMouseEnter()
    {
        myRenderer.material = newMaterial;
    }

    private void OnMouseExit()
    {
        myRenderer.material = oldMaterial;
    }
}
