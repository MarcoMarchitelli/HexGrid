using System.Collections;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Vector3 offset;
    public float transitionTime = 1f;
<<<<<<< HEAD

    public void SetTransform(PlayerController player)
    {
        transform.DOMove(new Vector3(player.startingWayPoint.worldPosition.x, offset.y, player.startingWayPoint.worldPosition.z), transitionTime);
        switch (player.type)
        {
            case PlayerController.Type.hypogeum:
                transform.DORotate(new Vector3(50f, 30f, 0), transitionTime);
                break;
            case PlayerController.Type.underwater:
                transform.DORotate(new Vector3(50f, -30f, 0), transitionTime);
                break;
            case PlayerController.Type.forest:
                transform.DORotate(new Vector3(50f, 210f, 0), transitionTime);
                break;
            case PlayerController.Type.underground:
                transform.DORotate(new Vector3(50f, -210f, 0), transitionTime);
                break;
=======
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
        Vector3 target = player.startingWayPoint.worldPosition + offset;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, transitionTime * Time.deltaTime);
            yield return null;
>>>>>>> 71e80326ba1a40fe6ea99b56d07341995d0a8fc2
        }
        isMoving = false;
    }
}
