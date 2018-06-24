using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

    [Header("Sprites")]
    public Sprite inactiveSprite;
    public Sprite activeSprite;
    public Sprite specialSprite;
    public Sprite confirmSprite;

    public UnityEvent OnSpriteInactive;
    public UnityEvent OnSpriteActive;
    public UnityEvent OnSpriteSpecial;

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
                if (OnSpriteInactive != null)
                    OnSpriteInactive.Invoke();
                break;
            case SpriteType.active:
                if (activeSprite != null)
                    image.sprite = activeSprite;
                if (OnSpriteActive != null)
                    OnSpriteActive.Invoke();
                break;
            case SpriteType.special:
                if (specialSprite != null)
                    image.sprite = specialSprite;
                if (OnSpriteSpecial != null)
                    OnSpriteSpecial.Invoke();
                break;
            case SpriteType.confirm:
                if (confirmSprite != null)
                    image.sprite = confirmSprite;
                break;
        }
    }

    public void SetSpriteSpecial()
    {
        if (specialSprite != null)
            image.sprite = specialSprite;
    }

    public void SetUsability(bool flag)
    {
        if (flag)
            button.enabled = true;
        else
            button.enabled = false;
    }
}
