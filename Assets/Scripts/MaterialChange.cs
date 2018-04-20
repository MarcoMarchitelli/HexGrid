using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class MaterialChange : MonoBehaviour {

    public Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnMouseEnter()
    {
        if(GameManager.instance.currentActivePlayer.currentWayPoint.worldPosition == transform.position)
        {
            return;
        }
        else if (GameManager.instance.currentActivePlayer.currentWayPoint.possibleDestinations.Contains(transform.position) && GameManager.instance.currentActivePlayer.possibleMoves != 0 && GameManager.instance.currentActivePlayer.currentAction == PlayerController.Action.moving)
        {
            outline.enabled = true;
            outline.color = 1;
        }
        else
        {
            outline.enabled = true;
            outline.color = 0;
        }
    }
        
    private void OnMouseExit()
    {
        outline.enabled = false;
    }
}
