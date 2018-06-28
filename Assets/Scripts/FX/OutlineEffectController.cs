using UnityEngine;
using cakeslice;

public class OutlineEffectController : MonoBehaviour
{
    public static OutlineEffectController instance;

    Color outlineColor00;
    Color outlineColor01;
    Color outlineColor02;

    float alphaCutoff;
    float thickness;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        outlineColor00 = OutlineEffect.Instance.lineColor0;
        outlineColor01 = OutlineEffect.Instance.lineColor1;
        outlineColor02 = OutlineEffect.Instance.lineColor2;
        alphaCutoff = OutlineEffect.Instance.alphaCutoff;
        thickness = OutlineEffect.Instance.lineThickness;
    }

    public void ChangeLineColor(int lineColorIndex)
    {
        switch (lineColorIndex)
        {
            case 0:
                OutlineEffect.Instance.lineColor0 = outlineColor00;
                break;
            case 1:
                OutlineEffect.Instance.lineColor1 = outlineColor01;
                break;
            case 2:
                OutlineEffect.Instance.lineColor2 = outlineColor02;
                break;
            default:
                Debug.LogWarning("Color index not valid.");
                break;
        }
    }

    public void ChangeLineColor(int lineColorIndex, Color newColor)
    {
        switch (lineColorIndex)
        {
            case 0:
                OutlineEffect.Instance.lineColor0 = newColor;
                break;
            case 1:
                OutlineEffect.Instance.lineColor1 = newColor;
                break;
            case 2:
                OutlineEffect.Instance.lineColor2 = newColor;
                break;
            default:
                Debug.LogWarning("Color index not valid.");
                break;
        }
    }

    public void ChangeLineAlphaCutoff(float value)
    {
        if (value < 0 || value > 1)
        {
            Debug.LogWarning("Alpha cutoff value must be between 0 and 1.");
            return;
        }

        OutlineEffect.Instance.alphaCutoff = value;
    }

    public void ChangeLineAlphaCutoff()
    {
        OutlineEffect.Instance.alphaCutoff = alphaCutoff;
    }

    public void ChangeLineThickness(float value)
    {
        if(value<1 || value>6)
        {
            Debug.LogWarning("Thickness alue must be between 0 and 1.");
            return;
        }

        OutlineEffect.Instance.lineThickness = value;
    }

    public void ChangeLineThickness()
    {
        OutlineEffect.Instance.lineThickness = thickness;
    }
}
