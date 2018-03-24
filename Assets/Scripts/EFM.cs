using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFM : MonoBehaviour
{

    public EFMPhase currentPhase = EFMPhase.first;

    GameManager gameManager;
    PlayerController[] players;

    public int[] atkNumbers = { 1, 3, 4, 5 };
    public int[] defNumbers = { 0, 1, 2, 4, 6 };

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void AutoChangePhase(int turnNumber)
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

    public void SetPhase(EFMPhase phase, PlayerController[] players)
    {
        currentPhase = phase;

        switch (currentPhase)
        {
            case EFMPhase.first:
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
            case EFMPhase.second:
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
            case EFMPhase.third:
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
            case EFMPhase.fourth:
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
        }
    }

    public PlayerController FightResult(PlayerController attacker, PlayerController defender, int atkBet, int defBet, out bool doesAttackerDoubleSteal)
    {
        PlayerController winner = null;
        doesAttackerDoubleSteal = false;

        //if attacker is weak to defender
        if (attacker.weaknessType == defender.type)
        {
            switch (defBet)
            {
                case 0:
                    winner = attacker;
                    break;
                case 1:
                    switch (atkBet)
                    {
                        case 1:
                            winner = defender;
                            break;
                        case 3:
                        case 4:
                        case 5:
                            winner = attacker;
                            break;
                    }
                    break;
                case 2:
                    switch (atkBet)
                    {
                        case 1:
                        case 3:
                            winner = defender;
                            break;
                        case 4:
                        case 5:
                            winner = attacker;
                            break;
                    }
                    break;
                case 4:
                    switch (atkBet)
                    {
                        case 1:
                        case 3:
                        case 4:
                            winner = defender;
                            break;
                        case 5:
                            winner = attacker;
                            break;
                    }
                    break;
                case 6:
                    winner = defender;
                    break;
            }
        }

        //if defender is weak to attacker
        else if (defender.weaknessType == defender.type)
        {
            //check double points steal
            if (atkBet == atkNumbers[atkNumbers.Length - 1] && defBet == defNumbers[0] || defBet == defNumbers[1])
                doesAttackerDoubleSteal = true;

            //ignores 0 defense and max defense
            switch (defBet)
            {
                case 0:
                    winner = attacker;
                    break;
                case 1:
                    winner = attacker;
                    break;
                case 2:
                    switch (atkBet)
                    {
                        case 1:
                            winner = null; ;
                            break;
                        case 3:
                        case 4:
                        case 5:
                            winner = attacker;
                            break;
                    }
                    break;
                case 4:
                    switch (atkBet)
                    {
                        case 1:
                            winner = defender;
                            break;
                        case 3:
                            winner = null;
                            break;
                        case 4:
                        case 5:
                            winner = attacker;
                            break;
                    }
                    break;
                case 6:
                    switch (atkBet)
                    {
                        case 1:
                        case 3:
                        case 4:
                            winner = defender;
                            break;
                        case 5:
                            winner = null;
                            break;
                    }
                    break;
            }
        }

        //no fight modifiers
        else
        {
            //check double points steal
            if (atkBet == 5 && defBet == 0 || defBet == 1)
                doesAttackerDoubleSteal = true;

            //ignores 0 defense and max defense
            switch (defBet)
            {
                case 0:
                    winner = attacker;
                    break;
                case 1:
                    switch (atkBet)
                    {
                        case 1:
                            winner = null;
                            break;
                        case 3:
                        case 4:
                        case 5:
                            winner = attacker;
                            break;
                    }
                    break;
                case 2:
                    switch (atkBet)
                    {
                        case 1:
                            winner = defender;
                            break;
                        case 3:
                            winner = null;
                            break;
                        case 4:
                        case 5:
                            winner = attacker;
                            break;
                    }
                    break;
                case 4:
                    switch (atkBet)
                    {
                        case 1:
                            winner = defender;
                            break;
                        case 3:
                            winner = defender;
                            break;
                        case 4:
                            winner = null;
                            break;
                        case 5:
                            winner = attacker;
                            break;
                    }
                    break;
                case 6:
                    winner = defender;
                    break;
            }
        }

        return winner;
    }

}

public enum EFMPhase
{
    first, second, third, fourth
}
