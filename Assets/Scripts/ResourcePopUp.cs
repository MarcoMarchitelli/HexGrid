using UnityEngine;
using TMPro;

public class ResourcePopUp : MonoBehaviour {

    public TextMeshProUGUI resourcePopUpText;
    public Animator animator;

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

}
