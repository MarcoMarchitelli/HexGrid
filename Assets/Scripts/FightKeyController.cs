using UnityEngine;
using UnityEngine.Events;

public class FightKeyController : MonoBehaviour {

    public KeyCode Keycode1;
    public KeyCode Keycode2;
    public UnityEvent OnButtonPress;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(Keycode1) || Input.GetKeyDown(Keycode2))
            OnButtonPress.Invoke();
	}
}
