using UnityEngine;
using UnityEngine.Events;

public class ModifierSelectionProxy : MonoBehaviour {

    public UnityEvent OnModifierSelectionTextDisappear;

	public void InvokeModProxyEvent()
    {
        OnModifierSelectionTextDisappear.Invoke();
    }

}
