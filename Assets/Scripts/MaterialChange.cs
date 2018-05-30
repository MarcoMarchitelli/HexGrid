using UnityEngine;
using System.Collections.Generic;
using cakeslice;

public class MaterialChange : MonoBehaviour {

    Outline outline;
    bool isOutlined = false;

    private void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        GameManager.instance.OnMoveEnter += TurnOutlineOn;
        GameManager.instance.OnMoveExit += TurnOutlineOff;
    }

    void TurnOutlineOn(List<AgentPosition> points)
    {
        foreach (var point in points)
        {
            if (Mathf.Approximately(point.point.worldPosition.x, transform.position.x) && Mathf.Approximately(point.point.worldPosition.z, transform.position.z))
            {
                outline.enabled = true;
                outline.color = 1;
                isOutlined = true;
                return;
            }
            else
            {
                outline.enabled = false;
                isOutlined = false;
            }
        }
    }

    void TurnOutlineOff()
    {
        if (isOutlined)
        {
            outline.enabled = false;
            isOutlined = false;
        }
    }

    private void OnMouseEnter()
    {
        if (isOutlined)
            outline.color = 2;
    }
        
    private void OnMouseExit()
    {
        if (isOutlined)
            outline.color = 1;
    }

    private void OnDisable()
    {
        GameManager.instance.OnMoveEnter -= TurnOutlineOn;
        GameManager.instance.OnMoveExit -= TurnOutlineOff;
    }
}
