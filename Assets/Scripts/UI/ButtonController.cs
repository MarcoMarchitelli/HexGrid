using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{

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

    [Header("UI Particles")]
    public GameObject MouseEnterParticle;
    public GameObject MouseDownParticle;
    public GameObject SpecialMouseDownParticle;

    ParticleSystem mouseDownSystem;
    ParticleSystem specialMouseDownSystem;

    private void Awake()
    {
        if(MouseDownParticle)
            mouseDownSystem = MouseDownParticle.GetComponent<ParticleSystem>();
        if(SpecialMouseDownParticle)
            specialMouseDownSystem = SpecialMouseDownParticle.GetComponent<ParticleSystem>();
    }

    public void SetSprite(SpriteType type)
    {
        switch (type)
        {
            case SpriteType.inactive:
                if (inactiveSprite != null)
                {
                    image.sprite = inactiveSprite;
                    spriteType = SpriteType.inactive;
                }
                if (OnSpriteInactive != null)
                    OnSpriteInactive.Invoke();
                break;
            case SpriteType.active:
                if (activeSprite != null)
                {
                    image.sprite = activeSprite;
                    spriteType = SpriteType.active;
                }
                if (OnSpriteActive != null)
                    OnSpriteActive.Invoke();
                break;
            case SpriteType.special:
                if (specialSprite != null)
                {
                    image.sprite = specialSprite;
                    spriteType = SpriteType.special;
                }
                if (OnSpriteSpecial != null)
                    OnSpriteSpecial.Invoke();
                break;
            case SpriteType.confirm:
                if (confirmSprite != null)
                {
                    image.sprite = confirmSprite;
                    spriteType = SpriteType.confirm;
                }
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

    public void OnMouseEnter()
    {
        if (spriteType == SpriteType.active)
        {
            if (MouseEnterParticle != null)
                MouseEnterParticle.SetActive(true);
        }
    }

    public void OnMouseExit()
    {
        if (MouseEnterParticle != null)
            MouseEnterParticle.SetActive(false);
    }

    public void OnMouseDown()
    {
        if (spriteType == SpriteType.active)
        {
            if (mouseDownSystem != null)
            {
                mouseDownSystem.Stop();
                mouseDownSystem.Play();
            }
        }
        else if (spriteType == SpriteType.special)
        {
            if (specialMouseDownSystem != null)
            {
                specialMouseDownSystem.Stop();
                specialMouseDownSystem.Play();
            }
        }

        if (MouseEnterParticle != null)
            MouseEnterParticle.SetActive(false);
    }
}
