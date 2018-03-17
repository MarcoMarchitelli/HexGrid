using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFM : MonoBehaviour {

    public EFMPhase currentPhase;

    GameManager gameManager;
    PlayerController[] players;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void ChangePhase(int turnNumber)
    {
        if (turnNumber % 4 == 1)
        {
            players = gameManager.players;
            switch (currentPhase)
            {
                case EFMPhase.first:
                    #region go to second phase
                    currentPhase = EFMPhase.second;
                    foreach (PlayerController player in players)
                    {
                        switch (player.type)
                        {
                            case PlayerController.Type.hypogeum:
                                player.weaknessType = PlayerController.Type.forest;
                                player.strenghtType = PlayerController.Type.underground;
                                break;
                            case PlayerController.Type.underwater:
                                player.weaknessType = PlayerController.Type.underground;
                                player.strenghtType = PlayerController.Type.forest;
                                break;
                            case PlayerController.Type.forest:
                                player.weaknessType = PlayerController.Type.underwater;
                                player.strenghtType = PlayerController.Type.hypogeum;
                                break;
                            case PlayerController.Type.underground:
                                player.weaknessType = PlayerController.Type.hypogeum;
                                player.strenghtType = PlayerController.Type.underwater;
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                    break;
                case EFMPhase.second:
                    #region go to third phase
                    currentPhase = EFMPhase.third;
                    foreach (PlayerController player in players)
                    {
                        switch (player.type)
                        {
                            case PlayerController.Type.hypogeum:
                                player.weaknessType = PlayerController.Type.underground;
                                player.strenghtType = PlayerController.Type.underwater;
                                break;
                            case PlayerController.Type.underwater:
                                player.weaknessType = PlayerController.Type.hypogeum;
                                player.strenghtType = PlayerController.Type.forest;
                                break;
                            case PlayerController.Type.forest:
                                player.weaknessType = PlayerController.Type.underwater;
                                player.strenghtType = PlayerController.Type.underground;
                                break;
                            case PlayerController.Type.underground:
                                player.weaknessType = PlayerController.Type.forest;
                                player.strenghtType = PlayerController.Type.hypogeum;
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                    break;
                case EFMPhase.third:
                    #region go to fourth phase
                    currentPhase = EFMPhase.fourth;
                    foreach (PlayerController player in players)
                    {
                        switch (player.type)
                        {
                            case PlayerController.Type.hypogeum:
                                player.weaknessType = PlayerController.Type.underwater;
                                player.strenghtType = PlayerController.Type.underground;
                                break;
                            case PlayerController.Type.underwater:
                                player.weaknessType = PlayerController.Type.forest;
                                player.strenghtType = PlayerController.Type.hypogeum;
                                break;
                            case PlayerController.Type.forest:
                                player.weaknessType = PlayerController.Type.underground;
                                player.strenghtType = PlayerController.Type.underwater;
                                break;
                            case PlayerController.Type.underground:
                                player.weaknessType = PlayerController.Type.hypogeum;
                                player.strenghtType = PlayerController.Type.forest;
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                    break;
                case EFMPhase.fourth:
                    #region go to first phase
                    currentPhase = EFMPhase.first;
                    foreach (PlayerController player in players)
                    {
                        switch (player.type)
                        {
                            case PlayerController.Type.hypogeum:
                                player.weaknessType = PlayerController.Type.underground;
                                player.strenghtType = PlayerController.Type.forest;
                                break;
                            case PlayerController.Type.underwater:
                                player.weaknessType = PlayerController.Type.forest;
                                player.strenghtType = PlayerController.Type.underground;
                                break;
                            case PlayerController.Type.forest:
                                player.weaknessType = PlayerController.Type.hypogeum;
                                player.strenghtType = PlayerController.Type.underwater;
                                break;
                            case PlayerController.Type.underground:
                                player.weaknessType = PlayerController.Type.underwater;
                                player.strenghtType = PlayerController.Type.hypogeum;
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                    break;
            }
        }
    }
}

public enum EFMPhase
{
    first, second, third, fourth
}
