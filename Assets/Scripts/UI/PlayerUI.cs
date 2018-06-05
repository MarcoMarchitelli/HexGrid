using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {

    public PlayerController player;

    [Header("UI References")]
    public Image icon;
    public TextMeshProUGUI PVtext;
    public TextMeshProUGUI PEtext;
    public TextMeshProUGUI bonusMoveActionText;

    public void SetPlayerReference(PlayerController _player)
    {
        player = _player;

        if (icon != null)
            icon.sprite = player.icon;
        if (PVtext != null)
            PVtext.text = player.victoryPoints.ToString();
        if (PEtext != null)
            PEtext.text = player.energyPoints.ToString();
        if (bonusMoveActionText != null)
        {
            if (player.bonusMoveActions > 0)
                bonusMoveActionText.text = player.bonusMoveActions.ToString();
            else
                bonusMoveActionText.text = null;
        }

    }

    public void Refresh()
    {
        if (icon != null)
            icon.sprite = player.icon;
        if (PVtext != null)
            PVtext.text = player.victoryPoints.ToString();
        if (PEtext != null)
            PEtext.text = player.energyPoints.ToString();
        if (bonusMoveActionText != null)
        {
            if (player.bonusMoveActions > 0)
                bonusMoveActionText.text = player.bonusMoveActions.ToString();
            else
                bonusMoveActionText.text = null;
        }
    }

}
