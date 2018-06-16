using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using EZCameraShake;

public class SmoothMoveAnimation : MonoBehaviour {

    public Transform TargetPosition;
    public float speed;
    public GameObject PVSphere;
    public GameObject SmokeParticle;
    public UnityEvent OnAnimationFinish;
    public UnityEvent OnAnimationStart;

    GameObject instSmoke;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(Animation());
    }

    public IEnumerator Animation()
    {
        
        OnAnimationStart.Invoke();
        CameraShaker.Instance.StartShake(.3f, 5, 0, Vector3.up * .3f, Vector3.zero);
        if (SmokeParticle)
        {
            instSmoke = Instantiate(SmokeParticle, transform.position + Vector3.up * .5f, Quaternion.Euler(Vector3.left * 90f));
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
        foreach (var shake in CameraShaker.Instance.ShakeInstances)
        {
            shake.StartFadeOut(0);
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
