using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using EZCameraShake;

public class SmoothMoveAnimation : MonoBehaviour {

    public Transform TargetPosition;
    public float speed;
    public GameObject PVSphere;
    public ParticleSystem SmokeParticle;
    public UnityEvent OnAnimationFinish;
    public UnityEvent OnAnimationStart;

    ParticleSystem instSmoke;

    public IEnumerator Animation()
    {
        
        OnAnimationStart.Invoke();
        CameraShaker.Instance.StartShake(.3f, 5, 0, Vector3.up * .3f, Vector3.zero);
        if (SmokeParticle)
        {
            instSmoke = Instantiate(SmokeParticle.gameObject, transform.position + Vector3.up * .5f, Quaternion.Euler(Vector3.left * 90f)).GetComponent<ParticleSystem>();
        }
        Vector3 target = TargetPosition.position;
        AudioManager.instance.Play("PVPointAnim");
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        AudioManager.instance.Stop("PVPointAnim");
        if (instSmoke)
        {
            instSmoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
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
