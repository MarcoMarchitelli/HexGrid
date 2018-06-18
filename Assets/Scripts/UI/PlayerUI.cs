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
    public Animator ExpandAnimator;
    public Animator PVanimator;
    public Animator PEanimator;
    public Animator BonusMoveAnimator;

    public void SetPlayerReference(PlayerController _player)
    {
        player = _player;

        if (icon != null)
            icon.sprite = player.icon;
        if (PVtext != null)
            PVtext.text = player.VictoryPoints.ToString();
        if (PEtext != null)
            PEtext.text = player.EnergyPoints.ToString();
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
            PVtext.text = player.VictoryPoints.ToString();
        if (PEtext != null)
            PEtext.text = player.EnergyPoints.ToString();
        if (bonusMoveActionText != null)
        {
            if (player.bonusMoveActions > 0)
                bonusMoveActionText.text = player.bonusMoveActions.ToString();
            else
                bonusMoveActionText.text = null;
        }
    }

    public void Expand(bool flag)
    {
        if (ExpandAnimator)
        {
            if (flag)
            {
                ExpandAnimator.ResetTrigger("Shrink");
                ExpandAnimator.SetTrigger("Expand");
                print("MI sto espandendooo");
            }
            else
            {
                ExpandAnimator.ResetTrigger("Expand");
                ExpandAnimator.SetTrigger("Shrink");
            }
        }
            
    }

}
