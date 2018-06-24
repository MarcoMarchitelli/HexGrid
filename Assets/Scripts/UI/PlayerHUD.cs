using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour {

    public PlayerController player;

    public TextMeshProUGUI PVvalue, PEvalue;

    private void Update()
    {
        PVvalue.text = player.VictoryPoints.ToString();
        PEvalue.text = player.EnergyPoints.ToString();
    }

}
