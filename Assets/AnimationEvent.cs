using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour {

    public UnityEvent OnAnimationEvent;

	public void InvokeAnimationEvent()
    {
        OnAnimationEvent.Invoke();
    }
}
