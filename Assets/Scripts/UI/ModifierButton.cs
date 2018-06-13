using TMPro;
using UnityEngine;

[RequireComponent(typeof(ButtonController))]
public class ModifierButton : MonoBehaviour {

    public ButtonController buttonController;
    public TextMeshProUGUI valueText;
    public int value;

    private void OnEnable()
    {
        if (valueText != null)
            valueText.text = value.ToString();
    }

}
