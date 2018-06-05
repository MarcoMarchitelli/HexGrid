using UnityEngine.Events;
using UnityEngine;
using cakeslice;

public class OutlineController : MonoBehaviour
{

    public bool MultipleOutlines;
    public bool OutlineInChildren;
    public Transform OutlinesParent;

    public UnityEvent OnMouseEnterEvent;
    public UnityEvent OnMouseExitEvent;

    Outline outline;
    Outline[] outlines;
    bool isOutlined = false;

    private void Start()
    {
        if (MultipleOutlines)
        {
            if (OutlineInChildren)
                outlines = OutlinesParent.GetComponentsInChildren<Outline>();
            else
                outlines = GetComponents<Outline>();
        }
        else
        {
            if (OutlineInChildren)
                outline = OutlinesParent.GetComponentInChildren<Outline>();
            else
                outline = GetComponent<Outline>();
        }

        if (outline != null)
            outline.enabled = false;
        if (outlines != null)
            foreach (var _outline in outlines)
            {
                _outline.enabled = false;
            }

        if (!outline && outlines == null)
            print("Outline reference not found");
    }

    public void EnableOutline(bool flag)
    {
        if (MultipleOutlines)
        {
            if (flag)
                foreach (var _outline in outlines)
                    _outline.enabled = true;
            else
                foreach (var _outline in outlines)
                    _outline.enabled = false;
        }
        else
        {
            if (flag)
                outline.enabled = true;
            else
                outline.enabled = false;
        }

    }

    public void SetColor(int colorId)
    {
        if(colorId <0 || colorId > 2)
        {
            print("Invalid color ID.");
            return;
        }

        if (MultipleOutlines)
            foreach (var _outline in outlines)
            {
                _outline.color = colorId;
            }
        else
            outline.color = colorId;

    }

    private void OnMouseEnter()
    {
        OnMouseEnterEvent.Invoke();
    }

    private void OnMouseExit()
    {
        OnMouseExitEvent.Invoke();
    }

}
