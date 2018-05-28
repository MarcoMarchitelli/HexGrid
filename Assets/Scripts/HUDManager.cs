using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{

    #region Public Variables

    [Header("Print Sections")]
    public TextMeshProUGUI bigCentralSection;

    [Header("Action Buttons")]
    public ButtonController moveButton;
    public ButtonController buyCardButton;
    public ButtonController placeCardButton;
    public ButtonController rotateCardButton;
    public ButtonController fightButton;

    [Header("Other Buttons")]
    public ButtonController endTurnButton;

    [Header("Other UI Elements")]
    public GameObject winOverlay;
    public GameObject[] actionIcons;

    #endregion

    [HideInInspector]
    public Animation bigNewsAnimation;

    void Start()
    {
        bigNewsAnimation = bigCentralSection.GetComponentInParent<Animation>();
    }

    public void Refresh()
    {
        ToggleActionButtons();
        ToggleEndTurnButton();
        ToggleActionsNumberDisplay();
    }

    public void PrintBigNews(string msg)
    {
        bigCentralSection.text = msg;
        bigNewsAnimation.Play();
    }

    public void ToggleActionButtons()
    {
        PlayerController player = GameManager.instance.currentActivePlayer;

        if (GameManager.instance.currentPhase != GameManager.Phase.main)
        {
            moveButton.SetSprite(ButtonController.SpriteType.inactive);
            moveButton.SetUsability(false);

            buyCardButton.SetSprite(ButtonController.SpriteType.inactive);
            buyCardButton.SetUsability(false);

            placeCardButton.SetSprite(ButtonController.SpriteType.inactive);
            placeCardButton.SetUsability(false);

            rotateCardButton.SetSprite(ButtonController.SpriteType.inactive);
            rotateCardButton.SetUsability(false);

            fightButton.SetSprite(ButtonController.SpriteType.inactive);
            fightButton.SetUsability(false);

            return;
        }

        if (player.actions > 0)
        {
            //move
            if(player.bonusMoveActions > 0)
            {
                moveButton.SetSprite(ButtonController.SpriteType.special);
                moveButton.SetUsability(true);
            }
            else
            {
                moveButton.SetSprite(ButtonController.SpriteType.active);
                moveButton.SetUsability(true);
            }    
            
            //confirm move
            if(player.currentAction == PlayerController.Action.moving && player.possibleMoves < 3)
            {
                moveButton.SetSprite(ButtonController.SpriteType.confirm);
                moveButton.SetUsability(true);
            }

            //buy
            if (player.hasDiscount)
            {
                buyCardButton.SetSprite(ButtonController.SpriteType.special);
                buyCardButton.SetUsability(true);
            }
            else if (GameManager.instance.turnCount <= 3 && player.energyPoints >= 1)
            {
                buyCardButton.SetSprite(ButtonController.SpriteType.active);
                buyCardButton.SetUsability(true);
            }
            else if (GameManager.instance.turnCount > 3 && player.energyPoints >= 2)
            {
                buyCardButton.SetSprite(ButtonController.SpriteType.active);
                buyCardButton.SetUsability(true);
            }
            else if (player.energyPoints <= 1)
            {
                buyCardButton.SetSprite(ButtonController.SpriteType.inactive);
                buyCardButton.SetUsability(false);
            }

            //place
            if (player.cardsInHand.Count >= 1)
            {
                placeCardButton.SetSprite(ButtonController.SpriteType.active);
                placeCardButton.SetUsability(true);
            }
            else if (player.cardsInHand.Count <= 0)
            {
                placeCardButton.SetSprite(ButtonController.SpriteType.inactive);
                placeCardButton.SetUsability(false);
            }

            //rotate
            if (player.HasCardInNearHexagons())
            {
                rotateCardButton.SetSprite(ButtonController.SpriteType.active);
                rotateCardButton.SetUsability(true);
            }
            else
            {
                rotateCardButton.SetSprite(ButtonController.SpriteType.inactive);
                rotateCardButton.SetUsability(false);
            }

            //fight
            if (player.playersToRob.Count > 0)
            {
                fightButton.SetSprite(ButtonController.SpriteType.active);
                fightButton.SetUsability(true);
            }
            else
            {
                fightButton.SetSprite(ButtonController.SpriteType.inactive);
                fightButton.SetUsability(false);
            }
        }
        else
        {
            if(player.bonusMoveActions > 0)
            {
                moveButton.SetSprite(ButtonController.SpriteType.special);
                moveButton.SetUsability(true);
            }
            else
            {
                moveButton.SetSprite(ButtonController.SpriteType.inactive);
                moveButton.SetUsability(false);
            } 

            buyCardButton.SetSprite(ButtonController.SpriteType.inactive);
            buyCardButton.SetUsability(false);

            placeCardButton.SetSprite(ButtonController.SpriteType.inactive);
            placeCardButton.SetUsability(false);

            rotateCardButton.SetSprite(ButtonController.SpriteType.inactive);
            rotateCardButton.SetUsability(false);

            fightButton.SetSprite(ButtonController.SpriteType.inactive);
            fightButton.SetUsability(false);
        }
    }

    public void ToggleEndTurnButton()
    {
        if (GameManager.instance.currentPhase != GameManager.Phase.main)
        {
            endTurnButton.SetUsability(false);
            endTurnButton.SetSprite(ButtonController.SpriteType.inactive);
        }
        else if (GameManager.instance.currentActivePlayer.currentAction == PlayerController.Action.start)
        {
            if (GameManager.instance.currentActivePlayer.actions > 0 || GameManager.instance.currentActivePlayer.bonusMoveActions > 0)
            {
                endTurnButton.SetUsability(true);
                endTurnButton.SetSprite(ButtonController.SpriteType.active);
            }
            else
            {
                endTurnButton.SetUsability(true);
                endTurnButton.SetSprite(ButtonController.SpriteType.special);
            }
        }
    }

    public void ToggleActionsNumberDisplay()
    {
        int actionsNumber = GameManager.instance.currentActivePlayer.actions;

        for (int i = 1; i <= actionIcons.Length; i++)
        {
            if (i <= actionsNumber)
                actionIcons[i - 1].SetActive(true);
            else
                actionIcons[i - 1].SetActive(false);
        }
    }

    public void OnMoveButton()
    {
        switch (GameManager.instance.currentActivePlayer.currentAction)
        {
            case PlayerController.Action.start:
                GameManager.instance.ChoseAction(0);
                break;
            case PlayerController.Action.moving:
                if(GameManager.instance.currentActivePlayer.possibleMoves == 3)
                {
                    GameManager.instance.UndoAction();
                }
                else
                {
                    GameManager.instance.ConfirmAction();
                }
                break;
            case PlayerController.Action.buyCard:
            case PlayerController.Action.placeCard:
            case PlayerController.Action.rotateCard:
            case PlayerController.Action.fight:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(0);
                break;
            default:
                break;
        }
    }

    public void OnBuyButton()
    {
        switch (GameManager.instance.currentActivePlayer.currentAction)
        {
            case PlayerController.Action.start:
                GameManager.instance.ChoseAction(1);
                break;         
            case PlayerController.Action.moving:
            case PlayerController.Action.placeCard:
            case PlayerController.Action.rotateCard:
            case PlayerController.Action.fight:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(1);
                break;
            default:
                break;
        }
    }

    public void OnPlaceCardButton()
    {
        switch (GameManager.instance.currentActivePlayer.currentAction)
        {
            case PlayerController.Action.start:
                GameManager.instance.ChoseAction(3);
                break;
            case PlayerController.Action.moving:
            case PlayerController.Action.buyCard:
            case PlayerController.Action.rotateCard:
            case PlayerController.Action.fight:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(3);
                break;
            default:
                break;
        }
    }

    public void OnRotateCardButton()
    {
        switch (GameManager.instance.currentActivePlayer.currentAction)
        {
            case PlayerController.Action.start:
                GameManager.instance.ChoseAction(4);
                break;
            case PlayerController.Action.moving:
            case PlayerController.Action.placeCard:
            case PlayerController.Action.buyCard:
            case PlayerController.Action.fight:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(4);
                break;
            default:
                break;
        }
    }

    public void OnFightButton()
    {
        switch (GameManager.instance.currentActivePlayer.currentAction)
        {
            case PlayerController.Action.start:
                GameManager.instance.ChoseAction(5);
                break;
            case PlayerController.Action.moving:
            case PlayerController.Action.placeCard:
            case PlayerController.Action.rotateCard:
            case PlayerController.Action.buyCard:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(5);
                break;
            default:
                break;
        }
    }

    public void Win(PlayerController player)
    {
        winOverlay.SetActive(true);
        winOverlay.GetComponentInChildren<TextMeshProUGUI>().text = player.type.ToString() + " Wins !";
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
