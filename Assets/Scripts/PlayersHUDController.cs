using UnityEngine;

public class PlayersHUDController : MonoBehaviour {

    public PlayerUI[] playerUIs;

    private void Start()
    {
        for (int i = 0; i < playerUIs.Length; i++)
        {
            playerUIs[i].SetPlayerReference(GameManager.instance.players[i]);
        }
    }

    public void CyclePlayersHUDs(int _turnCount)
    {
        for (int i = 0; i < playerUIs.Length; i++)
        {
            playerUIs[i].SetPlayerReference(GameManager.instance.players[(i + _turnCount) % 4]);
        }
    }

}
