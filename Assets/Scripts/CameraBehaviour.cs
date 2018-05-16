using System.Collections;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    Vector3 StandardView;
    public Transform HighView;
    public float transitionTime = 1f;
    public bool isMoving = false;
    bool isHighView = false, canChangeView = true;
    IEnumerator animation;

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
                animation = CameraAnimation(StandardView);
                StartCoroutine(animation);
                isHighView = !isHighView;
            }
            else
            {
                animation = CameraAnimation(HighView.position);
                StartCoroutine(animation);
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
            if(animation != null)
                StopCoroutine(animation);
            animation = CameraAnimation(HighView.position);
            StartCoroutine(animation);
            isHighView = !isHighView;
            canChangeView = false;

        }
        else if(!flag && isHighView)
        {
            if (animation != null)
                StopCoroutine(animation);
            animation = CameraAnimation(StandardView);
            StartCoroutine(animation);
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
}
