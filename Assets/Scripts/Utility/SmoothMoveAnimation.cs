using System.Collections;
using UnityEngine;
//DA FINIRE DEVELOPEMENT
public class SmoothMoveAnimation : MonoBehaviour
{

    public bool AutoGoBackToStart;

    public KeyCode StartAnimationInput;
    public float AnimationSpeed = 1f;

    [Header("Start Transform")]
    public Vector3 StartPosition;
    public Vector3 StartRotation;

    [Header("End Transform")]
    public Vector3 EndPosition;
    public Vector3 EndRotation;

    IEnumerator animation;
    bool isAtDestination = false;

    private void Update()
    {
        if (Input.GetKey(StartAnimationInput))
        {
            if (!isAtDestination)
            {
                animation = MoveAnimation(StartPosition, StartRotation, EndPosition, EndRotation, AnimationSpeed);
                StartCoroutine(animation);
            }
            else
            {

            } 
        }

    }

    IEnumerator MoveAnimation(Vector3 _startPosition, Vector3 _startRotation, Vector3 _endPosition, Vector3 _endRotation, float speed)
    {
        while (transform.position != _endPosition && transform.rotation.eulerAngles != _endRotation)
        {
            transform.position = Vector3.MoveTowards(transform.position, _endPosition, speed * Time.deltaTime);
            transform.rotation.SetEulerAngles(Vector3.MoveTowards(transform.rotation.eulerAngles, _endRotation, speed * Time.deltaTime));
            yield return null;
        }
        isAtDestination = true;
    }

}
