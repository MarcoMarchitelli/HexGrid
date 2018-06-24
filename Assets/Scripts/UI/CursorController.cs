using UnityEngine;

public class CursorController : MonoBehaviour {

    public Texture2D CursorIcon;

    private void Start()
    {
        Cursor.SetCursor(CursorIcon, Vector2.zero, CursorMode.Auto);
    }

}
