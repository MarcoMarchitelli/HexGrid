
using System.Collections;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    Vector3 StandardView;
    public Transform HighView;
    public float transitionTime = 1f;
    public bool isMoving = false, canChangeView = true;
    bool isHighView = false;
    IEnumerator cameraAnimation;

    private void Start()
    {
        transform.LookAt(GameManager.instance.gridReference.center.position);
        StandardView = transform.position;
    }

    private void Update()
    {
        if (!isMoving && Input.GetKey(KeyCode.Tab) && canChangeView)
        {
            if (isHighView)
            {
                cameraAnimation = CameraAnimation(StandardView);
                StartCoroutine(cameraAnimation);
                isHighView = !isHighView;
            }
            else
            {
                cameraAnimation = CameraAnimation(HighView.position);
                StartCoroutine(cameraAnimation);
                isHighView = !isHighView;
            }
        }
    }

    private void LateUpdate()
    {
        if(isMoving)
            transform.LookAt(GameManager.instance.gridReference.center.position);
    }

    public void SetHighView(bool flag)
    {
        if(flag && !isHighView)
        {
            if(cameraAnimation != null)
                StopCoroutine(cameraAnimation);
            cameraAnimation = CameraAnimation(HighView.position);
            StartCoroutine(cameraAnimation);
            isHighView = !isHighView;
            canChangeView = false;

        }
        else if(!flag && isHighView)
        {
            if (cameraAnimation != null)
                StopCoroutine(cameraAnimation);
            cameraAnimation = CameraAnimation(StandardView);
            StartCoroutine(cameraAnimation);
            isHighView = !isHighView;
            canChangeView = true;
        }
            
    }

    IEnumerator CameraAnimation(Vector3 target)
    {
        isMoving = true;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, transitionTime * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
    }

    public void CanChangeView(bool flag)
    {
        if (flag)
            canChangeView = true;
        else
            canChangeView = false;
    }

}
