using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SmoothMoveAnimation : MonoBehaviour {

    public Transform TargetPosition;
    public float speed;
    public GameObject PVSphere;
    public GameObject SmokeParticle;
    public UnityEvent OnAnimationFinish;
    public UnityEvent OnAnimationStart;

    GameObject instSmoke;

    public IEnumerator Animation()
    {
        
        OnAnimationStart.Invoke();
        if (SmokeParticle)
        {
            instSmoke = Instantiate(SmokeParticle, transform.position, Quaternion.Euler(Vector3.left * 90f));
        }
        Vector3 target = TargetPosition.position;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        if(instSmoke != null)
        {
            Destroy(instSmoke);
        }
        OnAnimationFinish.Invoke();
    }

    public void DestroySphere()
    {
        if (PVSphere)
        {
            Destroy(PVSphere);
        }
    }

}
