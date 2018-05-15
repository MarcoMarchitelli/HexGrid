using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class MaterialChange : MonoBehaviour {

    Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private void OnMouseEnter()
    {
        if (GameManager.instance.currentActivePlayer.currentAction == PlayerController.Action.moving && GameManager.instance.currentActivePlayer.possibleMoves > 0 && GameManager.instance.currentActivePlayer.currentWayPoint.possibleDestinations.Contains(transform.position))
        {
            outline.enabled = true;
            outline.color = 1;
        }
        else if (GameManager.instance.currentActivePlayer.currentAction == PlayerController.Action.moving && (!GameManager.instance.currentActivePlayer.currentWayPoint.possibleDestinations.Contains(transform.position) || GameManager.instance.currentActivePlayer.possibleMoves == 0))
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
