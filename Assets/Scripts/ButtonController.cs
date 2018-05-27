using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

    public Sprite inactiveSprite;
    public Sprite activeSprite;
    public Sprite specialSprite;
    public Sprite confirmSprite;

    public enum SpriteType
    {
        inactive, active, special, confirm
    }

    [HideInInspector]
    public SpriteType spriteType;

    public Image image;
    public Button button;

    public void SetSprite(SpriteType type)
    {
        switch (type)
        {
            case SpriteType.inactive:
                if (inactiveSprite != null)
                    image.sprite = inactiveSprite;
                break;
            case SpriteType.active:
                if (activeSprite != null)
                    image.sprite = activeSprite;
                break;
            case SpriteType.special:
                if (specialSprite != null)
                    image.sprite = specialSprite;
                break;
            case SpriteType.confirm:
                if (confirmSprite != null)
                    image.sprite = confirmSprite;
                break;
        }
    }

    public void SetUsability(bool flag)
    {
        if (flag)
            button.enabled = true;
        else
            button.enabled = false;
    }
}
