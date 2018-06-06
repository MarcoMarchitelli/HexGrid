using UnityEngine;
using UnityEngine.UI;

public class ModifierButtonsController : MonoBehaviour {

    public ModifierButton[] buttons;
    public Image playerIcon;

    public void ToggleModifierButtons(PlayerController player)
    {
        if (playerIcon != null)
            playerIcon.sprite = player.icon;
        foreach (ModifierButton button in buttons)
        {
            if(player.energyPoints >= button.value)
            {
                button.buttonController.SetSprite(ButtonController.SpriteType.active);
                button.buttonController.SetUsability(true);
            }
            else
            {
                button.buttonController.SetSprite(ButtonController.SpriteType.inactive);
                button.buttonController.SetUsability(false);
            }


        }
    }

    public void EnableModifierButtons(bool flag)
    {
        foreach (ModifierButton modifierButton in buttons)
        {
            if (!flag)
                modifierButton.buttonController.SetUsability(false);
            else
                modifierButton.buttonController.SetUsability(true);
        }
    }

}
