using System.Collections;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Vector3 offset;
    public float transitionTime = 1f;
    public bool isMoving = false;

    public void SetTransform(PlayerController player)
    {
        StartCoroutine(CameraAnimation(player));
    }

    private void LateUpdate()
    {
        if(isMoving)
            transform.LookAt(GameManager.instance.gridReference.center.position);
    }

    IEnumerator CameraAnimation(PlayerController player)
    {
        isMoving = true;
        if (GameManager.instance.currentActivePlayer.UIrefresh != null)
            GameManager.instance.currentActivePlayer.UIrefresh(GameManager.instance.currentActivePlayer);
        Vector3 target = player.startingWayPoint.worldPosition + offset;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, transitionTime * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
        if (GameManager.instance.currentActivePlayer.UIrefresh != null)
            GameManager.instance.currentActivePlayer.UIrefresh(GameManager.instance.currentActivePlayer);
    }
}
