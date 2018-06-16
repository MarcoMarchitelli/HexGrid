using UnityEngine;
using System.Collections.Generic;
using cakeslice;

public class MaterialChange : MonoBehaviour
{

    public bool MultipleOutlines;
    public bool OutlineInChildren;

    Outline outline;
    Outline[] outlines;
    bool isOutlined = false;

    private void Start()
    {
        if (MultipleOutlines)
        {
            if (OutlineInChildren)
                outlines = GetComponentsInChildren<Outline>();
            else
                outlines = GetComponents<Outline>();
        }
        else
        {
            if (OutlineInChildren)
                outline = GetComponentInChildren<Outline>();
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

        GameManager.instance.OnMoveEnter += TurnOutlineOn;
        GameManager.instance.OnMoveSelected += TurnOutlineOff;
    }

    void TurnOutlineOn(List<Point> points)
    {
        foreach (var point in points)
        {
            if (point == GameManager.instance.currentActivePlayer.currentWayPoint || (point.isFinalWaypoint && point.isFinalWaypointUsed))
            {
                if (!MultipleOutlines)
                {
                    outline.enabled = false;
                    isOutlined = false;
                }
                else
                {
                    foreach (var _outline in outlines)
                    {
                        _outline.enabled = false;
                    }
                    isOutlined = false;
                }
            }
            else if (Mathf.Approximately(point.worldPosition.x, transform.position.x) && Mathf.Approximately(point.worldPosition.z, transform.position.z))
            {
                if (!MultipleOutlines)
                {
                    outline.enabled = true;
                    outline.color = 1;
                    isOutlined = true;
                    return;
                }
                else
                {
                    foreach (var _outline in outlines)
                    {
                        _outline.enabled = true;
                        _outline.color = 1;
                    }
                    isOutlined = true;
                    return;
                }
            }
        }
    }

    void TurnOutlineOff()
    {
        if (isOutlined)
        {
            if (outline != null)
            {
                outline.enabled = false;
                isOutlined = false;
                return;
            }
            else if (outlines != null)
            {
                foreach (var _outline in outlines)
                {
                    _outline.enabled = false;
                }
                isOutlined = false;
                return;
            }
        }
    }

    public void HighlightOn()
    {
        if (isOutlined)
        {
            if (outline != null)
                outline.color = 2;
            else if (outlines != null)
                foreach (var _outline in outlines)
                {
                    _outline.color = 2;
                }
        }

    }

    public void HighlightOff()
    {
        if (isOutlined)
        {
            if (outline != null)
                outline.color = 1;
            else if (outlines != null)
                foreach (var _outline in outlines)
                {
                    _outline.color = 1;
                }
        }
    }

    private void OnMouseEnter()
    {
        if (isOutlined)
        {
            if (outline != null)
                outline.color = 2;
            else if (outlines != null)
                foreach (var _outline in outlines)
                {
                    _outline.color = 2;
                }
        }
    }

    private void OnMouseExit()
    {
        if (isOutlined)
        {
            if (outline != null)
                outline.color = 1;
            else if (outlines != null)
                foreach (var _outline in outlines)
                {
                    _outline.color = 1;
                }
        }

    }

    private void OnDisable()
    {
        GameManager.instance.OnMoveEnter -= TurnOutlineOn;
        GameManager.instance.OnMoveSelected -= TurnOutlineOff;
    }

}
