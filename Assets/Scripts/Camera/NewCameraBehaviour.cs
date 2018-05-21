using UnityEngine;
using System.Collections;

public class NewCameraBehaviour : MonoBehaviour {

    [Header("Standard Transform")]
    public Vector3 StandardViewPosition;
    public Vector3 StandardViewRotation;
    [Header("High Transform")]
    public Vector3 HighViewPosition;
    public Vector3 HighViewRotation;

    public float speed = 1f;

    bool isHighView = false;
    IEnumerator cameraAnimation;

    private void Start()
    {
        StandardViewPosition = transform.position;
        StandardViewRotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            if (isHighView)
            {
                cameraAnimation = MoveAnimation(StandardViewPosition, HighViewPosition, speed);
                StartCoroutine(cameraAnimation);
                isHighView = !isHighView;
            }
            else
            {
                cameraAnimation = MoveAnimation(HighViewPosition, StandardViewPosition, speed);
                StartCoroutine(cameraAnimation);
                isHighView = !isHighView;
            }       
        }
    }

    IEnumerator MoveAnimation(Vector3 _startPosition, Vector3 _endPosition, float speed)
    {
        while (transform.position != _endPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, _endPosition, speed * Time.deltaTime);
            yield return null;
        }
    }
}
