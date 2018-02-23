using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChange : MonoBehaviour {

    MeshRenderer myRenderer;
    GameManager gameManager;

    public Material newMaterial;
    Color originalColor;

	void Awake () {
        gameManager = FindObjectOfType<GameManager>();
        myRenderer = GetComponent<MeshRenderer>();
        originalColor = myRenderer.material.color;
    }

    private void OnMouseEnter()
    {
        if(gameManager.currentActivePlayer.currentWayPoint.worldPosition == transform.position)
        {
            return;
        }else
        if (gameManager.currentActivePlayer.currentWayPoint.possibleDestinations.Contains(transform.position))
        {
            myRenderer.material.color = Color.green;
        }else
        {
            myRenderer.material.color = Color.red;
        }
    }
        

    private void OnMouseExit()
    {
        myRenderer.material.color = originalColor;
    }
}
