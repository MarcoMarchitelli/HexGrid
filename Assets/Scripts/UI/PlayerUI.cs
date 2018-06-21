using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {

    public PlayerController player;

    [Header("UI References")]
    public Image icon;
    public TextMeshProUGUI PVtext;
    public TextMeshProUGUI PVDifferenceText;
    public TextMeshProUGUI PEtext;
    public TextMeshProUGUI PEDifferenceText;
    public TextMeshProUGUI bonusMoveActionText;
    public TextMeshProUGUI bmDifferenceText;
    public Animator ExpandAnimator;

    public UnityEvent OnPVGain;
    public UnityEvent OnPEGain;
    public UnityEvent OnPVLoss;
    public UnityEvent OnPELoss;

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
            if (player.BonusMoveActions > 0)
                bonusMoveActionText.text = player.BonusMoveActions.ToString();
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
        if (PVDifferenceText)
            PVDifferenceText.text = Mathf.Abs(player.PVdiffenece).ToString();

        if (player.PVdiffenece > 0)
            OnPVGain.Invoke();
        else if(player.PVdiffenece < 0)
            OnPVLoss.Invoke();
        
        if (PEtext != null)
            PEtext.text = player.EnergyPoints.ToString();
        if (PEDifferenceText)
            PEDifferenceText.text = Mathf.Abs(player.PEdifference).ToString();

        if (player.PEdifference > 0)
            OnPEGain.Invoke();
        else if(player.PEdifference < 0)
            OnPELoss.Invoke();

        if (bonusMoveActionText != null)
        {
            if (player.BonusMoveActions > 0)
            {
                bonusMoveActionText.text = player.BonusMoveActions.ToString();
                bmDifferenceText.text = player.BMdifference.ToString();
            }
            else
                bonusMoveActionText.text = null;
        }

        player.PVdiffenece = 0;
        player.PEdifference = 0;
        player.BMdifference = 0;
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
