using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{

    public bool helpEnabled = false;

    #region Public Variables

    [Header("Print Sections")]
    public TextMeshProUGUI bigCentralSection;
    public TextMeshProUGUI MediumCentralSection;
    public Animation bigNewsAnimation;
    public Animation mediumNewsAnimation;

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
    public GameObject pauseMenu;
    public GameObject[] actionIcons;
    public Image WinningPlayerIcon;

    #endregion
    
    bool paused = false;
    string[] CannotUndoRotateMessages = { "Place that card first!", "Too late to go back now!", "You've got to finish your action!" };
    string[] CannotUndoMovingMessages = { "C'mon you're still running...", "Wait, you're still moving!", "You cannot interrupt your move now." };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.instance.currentActivePlayer.currentAction != PlayerController.Action.fight && !GameManager.instance.helpOpened)
        {
            if (paused)
                Pause(false);
            else
                Pause(true);
        }
    }

    public void Pause(bool flag)
    {
        if (flag)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            paused = true;
        }
        else
        {
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
            paused = false;
        }
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

    public void PrintMediumNews(string msg)
    {
        if (mediumNewsAnimation.isPlaying)
        {
            mediumNewsAnimation.Stop();
            MediumCentralSection.text = msg;
            mediumNewsAnimation.Play();
        }
        else
        {
            MediumCentralSection.text = msg;
            mediumNewsAnimation.Play();
        }
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

        if (player.isRunning)
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

        if(player.currentAction == PlayerController.Action.rotateCard && player.selectedCard != null)
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
            if (player.BonusMoveActions > 0)
            {
                moveButton.SetSprite(ButtonController.SpriteType.special);
                moveButton.SetUsability(true);
            }
            else
            {
                moveButton.SetSprite(ButtonController.SpriteType.active);
                moveButton.SetUsability(true);
            }

            //buy
            if (player.hasDiscount)
            {
                buyCardButton.SetSprite(ButtonController.SpriteType.special);
                buyCardButton.SetUsability(true);
            }
            else if (GameManager.instance.turnCount <= 3 && player.EnergyPoints >= 1)
            {
                buyCardButton.SetSprite(ButtonController.SpriteType.active);
                buyCardButton.SetUsability(true);
            }
            else if (GameManager.instance.turnCount > 3 && player.EnergyPoints >= 2)
            {
                buyCardButton.SetSprite(ButtonController.SpriteType.active);
                buyCardButton.SetUsability(true);
            }
            else if (player.EnergyPoints <= 1)
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
            if (player.BonusMoveActions > 0)
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
        else if (GameManager.instance.currentActivePlayer.currentAction != PlayerController.Action.start)
        {
            endTurnButton.SetUsability(false);
            endTurnButton.SetSprite(ButtonController.SpriteType.inactive);
        }
        else
        {
            if (GameManager.instance.currentActivePlayer.actions > 0 || GameManager.instance.currentActivePlayer.BonusMoveActions > 0)
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
                if (GameManager.instance.currentActivePlayer.isRunning == false)
                {
                    GameManager.instance.UndoAction();
                }
                else
                    PrintMediumNews(CannotUndoMovingMessages[Random.Range(0, CannotUndoMovingMessages.Length)]);
                break;
            case PlayerController.Action.buyCard:
            case PlayerController.Action.placeCard:
            case PlayerController.Action.fight:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(0);
                break;
            case PlayerController.Action.rotateCard:
                if (GameManager.instance.currentActivePlayer.selectedCard == null)
                {
                    GameManager.instance.UndoAction();
                    GameManager.instance.ChoseAction(0);
                }
                else
                    PrintMediumNews(CannotUndoRotateMessages[Random.Range(0, CannotUndoRotateMessages.Length)]);
                break;
        }
        Refresh();
    }

    public void OnBuyButton()
    {
        switch (GameManager.instance.currentActivePlayer.currentAction)
        {
            case PlayerController.Action.start:
                GameManager.instance.ChoseAction(1);
                break;
            case PlayerController.Action.moving:
                if (GameManager.instance.currentActivePlayer.isRunning == false)
                {
                    GameManager.instance.UndoAction();
                    GameManager.instance.ChoseAction(1);
                }
                else
                    PrintMediumNews(CannotUndoMovingMessages[Random.Range(0, CannotUndoMovingMessages.Length)]);
                break;
            case PlayerController.Action.placeCard:
            case PlayerController.Action.fight:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(1);
                break;
            case PlayerController.Action.rotateCard:
                if (GameManager.instance.currentActivePlayer.selectedCard == null)
                {
                    GameManager.instance.UndoAction();
                    GameManager.instance.ChoseAction(1);
                }
                else
                    PrintMediumNews(CannotUndoRotateMessages[Random.Range(0, CannotUndoRotateMessages.Length)]);
                break;
        }
        Refresh();
    }

    public void OnPlaceCardButton()
    {
        switch (GameManager.instance.currentActivePlayer.currentAction)
        {
            case PlayerController.Action.start:
                GameManager.instance.ChoseAction(3);
                break;
            case PlayerController.Action.moving:
                if (GameManager.instance.currentActivePlayer.isRunning == false)
                {
                    GameManager.instance.UndoAction();
                    GameManager.instance.ChoseAction(3);
                }
                else
                    PrintMediumNews(CannotUndoMovingMessages[Random.Range(0, CannotUndoMovingMessages.Length)]);
                break;
            case PlayerController.Action.rotateCard:
                if (GameManager.instance.currentActivePlayer.selectedCard == null)
                {
                    GameManager.instance.UndoAction();
                    GameManager.instance.ChoseAction(3);
                }
                else
                    PrintMediumNews(CannotUndoRotateMessages[Random.Range(0, CannotUndoRotateMessages.Length)]);
                break;
            case PlayerController.Action.buyCard:
            case PlayerController.Action.fight:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(3);
                break;
        }
        Refresh();
    }

    public void OnRotateCardButton()
    {
        switch (GameManager.instance.currentActivePlayer.currentAction)
        {
            case PlayerController.Action.start:
                GameManager.instance.ChoseAction(4);
                break;
            case PlayerController.Action.moving:
                if (GameManager.instance.currentActivePlayer.isRunning == false)
                {
                    GameManager.instance.UndoAction();
                    GameManager.instance.ChoseAction(4);
                }
                else
                    PrintMediumNews(CannotUndoMovingMessages[Random.Range(0, CannotUndoMovingMessages.Length)]);
                break;
            case PlayerController.Action.placeCard:
            case PlayerController.Action.buyCard:
            case PlayerController.Action.fight:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(4);
                break;
            case PlayerController.Action.rotateCard:
                if (GameManager.instance.currentActivePlayer.selectedCard == null)
                {
                    GameManager.instance.UndoAction();
                }
                else
                    PrintMediumNews(CannotUndoRotateMessages[Random.Range(0, CannotUndoRotateMessages.Length)]);
                break;
        }
        Refresh();
    }

    public void OnFightButton()
    {
        switch (GameManager.instance.currentActivePlayer.currentAction)
        {
            case PlayerController.Action.start:
                GameManager.instance.ChoseAction(5);
                break;
            case PlayerController.Action.moving:
                if (GameManager.instance.currentActivePlayer.isRunning == false)
                {
                    GameManager.instance.UndoAction();
                    GameManager.instance.ChoseAction(5);
                }
                else
                    PrintMediumNews(CannotUndoMovingMessages[Random.Range(0, CannotUndoMovingMessages.Length)]);
                break;
            case PlayerController.Action.rotateCard:
                if (GameManager.instance.currentActivePlayer.selectedCard == null)
                {
                    GameManager.instance.UndoAction();
                    GameManager.instance.ChoseAction(5);
                }
                else
                    PrintMediumNews(CannotUndoRotateMessages[Random.Range(0, CannotUndoRotateMessages.Length)]);
                break;
            case PlayerController.Action.placeCard:
            case PlayerController.Action.buyCard:
                GameManager.instance.UndoAction();
                GameManager.instance.ChoseAction(5);
                break;
            case PlayerController.Action.fight:
                GameManager.instance.UndoAction();
                break;
        }
        Refresh();
    }

    public void Win(PlayerController player)
    {
        WinningPlayerIcon.sprite = player.winIcon;
        winOverlay.SetActive(true);
        AudioManager.instance.Stop("Background");
        AudioManager.instance.Play("Victory");
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        AudioManager.instance.StopAll();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
