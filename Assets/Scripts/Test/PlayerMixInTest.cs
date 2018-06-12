using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMixInTest : MonoBehaviour {

    PlayerController player;

    public int VP, EP;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        if (!player)
            Debug.LogWarning("Player reference not found.");
    }

    private void Start()
    {
        if (player)
        {
            player.victoryPoints = VP;
            player.EnergyPoints = EP;
        }
    }

}
