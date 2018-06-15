using UnityEngine;
using TMPro;

public class ResourcePopUp : MonoBehaviour {

    public TextMeshProUGUI resourcePopUpText;
    public Animator animator;

    [HideInInspector]
    public bool animFinished = false;

    private void LateUpdate()
    {
        transform.LookAt(GameManager.instance.mainCamera.transform);
        transform.Rotate(Vector3.up * 180f);
    }

    public void SetString(string msg, Color color)
    {
        resourcePopUpText.color = color;
        resourcePopUpText.text = msg;
    }

    public void ResetString()
    {
        resourcePopUpText.text = null;
    }

    public void SetAnimationFinished()
    {
        animFinished = true;
    }

}
