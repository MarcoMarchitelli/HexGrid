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

    #endregion

    [HideInInspector]
    public Animation bigNewsAnimation;

    void Start()
    {
        bigNewsAnimation = bigCentralSection.GetComponentInParent<Animation>();
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

        if (player.currentAction == PlayerController.Action.start && player.actions > 0)
        {
            if (moveButton.gameObject.activeSelf)
            {
                //move
                moveButton.SetSprite(ButtonController.SpriteType.active);
                moveButton.SetUsability(true);
            }

            if (buyCardButton.gameObject.activeSelf)
            {
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
                else if (player.energyPoints >= 2)
                {
                    buyCardButton.SetSprite(ButtonController.SpriteType.active);
                    buyCardButton.SetUsability(true);
                }
                else if (player.energyPoints <= 1)
                {
                    buyCardButton.SetSprite(ButtonController.SpriteType.inactive);
                    buyCardButton.SetUsability(false);
                }
            }

            if (placeCardButton.gameObject.activeSelf)
            {
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
            }

            if (rotateCardButton.gameObject.activeSelf)
            {
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
            }

            if (fightButton.gameObject.activeSelf)
            {
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
        }
        else
        {
            if (moveButton.gameObject.activeSelf)
            {
                moveButton.SetSprite(ButtonController.SpriteType.inactive);
                moveButton.SetUsability(false);
            }
            if (buyCardButton.gameObject.activeSelf)
            {
                buyCardButton.SetSprite(ButtonController.SpriteType.inactive);
                buyCardButton.SetUsability(false);
            }
            if (placeCardButton.gameObject.activeSelf)
            {
                placeCardButton.SetSprite(ButtonController.SpriteType.inactive);
                placeCardButton.SetUsability(false);
            }
            if (rotateCardButton.gameObject.activeSelf)
            {
                rotateCardButton.SetSprite(ButtonController.SpriteType.inactive);
                rotateCardButton.SetUsability(false);
            }
            if (fightButton.gameObject.activeSelf)
            {
                fightButton.SetSprite(ButtonController.SpriteType.inactive);
                fightButton.SetUsability(false);
            }
        }
    }

    public void ToggleEndTurnButton(PlayerController player)
    {
        if (GameManager.instance.currentPhase != GameManager.Phase.main)
        {
            endTurnButton.SetUsability(false);
            endTurnButton.SetSprite(ButtonController.SpriteType.inactive);
        }
        else
        {
            endTurnButton.SetUsability(true);
            endTurnButton.SetSprite(ButtonController.SpriteType.active);
        }
    }

    public void OnMoveButton()
    {
        GameManager.instance.ChoseAction(0);
    }

    public void OnBuyButton()
    {
        GameManager.instance.ChoseAction(1);
    }

    public void OnPlaceCardButton()
    {
        GameManager.instance.ChoseAction(3);
    }

    public void OnRotateCardButton()
    {
        GameManager.instance.ChoseAction(4);
    }

    public void OnFightButton()
    {
        GameManager.instance.ChoseAction(5);
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
