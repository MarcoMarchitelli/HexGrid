using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SmoothMoveAnimation : MonoBehaviour {

    public Transform TargetPosition;
    public float speed;
    public UnityEvent OnAnimationFinish;
    public UnityEvent OnAnimationStart;

    public IEnumerator Animation()
    {
        OnAnimationStart.Invoke();
        Vector3 target = TargetPosition.position;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        OnAnimationFinish.Invoke();
    }

}
