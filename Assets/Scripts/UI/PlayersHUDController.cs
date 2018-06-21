using UnityEngine;

public class PlayersHUDController : MonoBehaviour
{

    public PlayerUI[] playerUIs;
    public PlayerUI ActivePlayerUI;

    private void Start()
    {
        for (int i = 0; i < playerUIs.Length; i++)
        {
            playerUIs[i].SetPlayerReference(GameManager.instance.players[i]);
        }
    }

    public void CyclePlayersHUDs(bool isFirstTurn)
    {
        ActivePlayerUI.SetPlayerReference(GameManager.instance.currentActivePlayer);
        ActivePlayerUI.Refresh();

        if (isFirstTurn)
        {
            playerUIs[0].Expand(true);
            return;
        }
        for (int i = 0; i < GameManager.instance.players.Length; i++)
        {  
            if (playerUIs[i].player == GameManager.instance.currentActivePlayer)
            {
                if (i != 0)
                    playerUIs[i - 1].Expand(false);
                else
                    playerUIs[3].Expand(false);

                playerUIs[i].Expand(true);
                return;
            }
        }
    }

    public void RefreshPlayerUIs()
    {
        foreach (var playerUI in playerUIs)
        {
            playerUI.Refresh();
        }
    }

}