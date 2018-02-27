using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraBehaviour : MonoBehaviour
{

    public Vector3 offset;

    public void SetTransform(PlayerController player)
    {
        transform.DOMove(new Vector3(player.startingWayPoint.worldPosition.x, offset.y, player.startingWayPoint.worldPosition.z), 1);
        switch (player.name)
        {
            case "yellow":
                transform.DORotate(new Vector3(50f, 30f, 0), 1);
                break;
            case "blue":
                transform.DORotate(new Vector3(50f, -30f, 0), 1);
                break;
            case "green":
                transform.DORotate(new Vector3(50f, 210f, 0), 1);
                break;
            case "red":
                transform.DORotate(new Vector3(50f, -210f, 0), 1);
                break;
        }
    }
}
