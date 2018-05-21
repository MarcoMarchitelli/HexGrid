using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnactivePlayerUI : MonoBehaviour {

    public bool isActivePlayer = false;
    public PlayerController player;

    [Header("UI References")]
    public Image icon;
    public TextMeshProUGUI PVtext;
    public TextMeshProUGUI PEtext;

    public void SetPlayerReference(PlayerController _player)
    {
        player = _player;
        
        SetIcon();
        SetPE();
        SetPV();
        SetBonusMoves();
        SetAvailableActions();
    }

    public void SetIcon()
    {
        icon = player.icon;
    }

    public void SetPV()
    {
        PVtext.text = player.victoryPoints.ToString();
    }

    public void SetPE()
    {
        PEtext.text = player.energyPoints.ToString();
    }

    public void SetBonusMoves()
    {
        if (!isActivePlayer)
            return;
        //DO STUFF
    }

    public void SetAvailableActions()
    {
        if (!isActivePlayer)
            return;
        //DO STUFF
    }

}
