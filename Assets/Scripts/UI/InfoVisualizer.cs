using UnityEngine;
using TMPro;

public class InfoVisualizer : MonoBehaviour {

    [Header("API")]
    public string Info;

    [Header("References")]
    public Animator infoAnimator;
    public TextMeshProUGUI TextField;

    private void Start()
    {
        if(TextField != null)
        {
            TextField.text = Info;
        }
    }

    public void OnMouseEnter()
    {
        if (infoAnimator != null)
        {
            infoAnimator.ResetTrigger("Hide");
            infoAnimator.SetTrigger("Show");
        }
    }

    public void OnMouseExit()
    {
        if (infoAnimator != null)
        {
            infoAnimator.ResetTrigger("Show");
            infoAnimator.SetTrigger("Hide");
        }
    }
}
