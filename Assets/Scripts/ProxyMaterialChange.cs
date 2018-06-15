using UnityEngine;

public class ProxyMaterialChange : MonoBehaviour {

    Point myPoint;

    private void Start()
    {
        myPoint = GameManager.instance.gridReference.GetPointFromWorldPosition(transform.position);
        if (myPoint == null)
            Debug.LogWarning("Point reference not found.");
    }

    private void OnMouseEnter()
    {
        myPoint.triangle.HighlightOn();
    }

    private void OnMouseExit()
    {
        myPoint.triangle.HighlightOff();
    }

}
