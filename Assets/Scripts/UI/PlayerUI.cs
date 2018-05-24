using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour {

    public PlayerController player;

    [Header("UI References")]
    public Image icon;
    public TextMeshProUGUI PVtext;
    public TextMeshProUGUI PEtext;

    public void SetPlayerReference(PlayerController _player)
    {
        player = _player;

        if (icon != null)
            icon.sprite = player.icon;
        if (PVtext != null)
            PVtext.text = player.victoryPoints.ToString();
        if (PEtext != null)
            PEtext.text = player.energyPoints.ToString();

    }

}
