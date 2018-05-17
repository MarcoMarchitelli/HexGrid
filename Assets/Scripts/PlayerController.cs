using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using DG.Tweening;
using cakeslice;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public delegate void UIevent(PlayerController player);
    public UIevent UIrefresh;

    public enum Type
    {
        hypogeum, underwater, forest, underground
    }

    public enum Action
    {
        idle, start, moving, buyCard, sellCard, placeCard, rotateCard, bet
    }

    #region Public Variables

    public Type type, weaknessType, strenghtType;
    [HideInInspector]
    public Point startingWayPoint;
    [HideInInspector]
    public Point currentWayPoint, moveStartPoint;
    [HideInInspector]
    public Action currentAction = Action.idle;
    [HideInInspector]
    public int energyPoints = 2, victoryPoints = 3, actions = 2;
    [HideInInspector]
    public int possibleMoves = 3;
    [HideInInspector]
    public int numberOfCards1InHand = 0, numberOfCards2InHand = 0, numberOfCards3InHand = 0;
    [HideInInspector]
    public int bonusMoveActions = 0, bonusAbilityActions = 0;
    [HideInInspector]
    public bool hasPlacedCard, hasBet, canBet, hasBought, isBonusMove, hasSold, hasDiscount;
    public List<CardController> cardsInHand;
    [HideInInspector]
    public CardController selectedCard, lastPlacedCard, cardToSell;
    public List<PlayerController> playersToRob = new List<PlayerController>();
    public LayerMask pointLayer, hexLayer, cardLayer, playerLayer;
    [HideInInspector]
    public int beforeActionEnergyPoints, beforeRotateActionCardEulerAngle, beforeMoveActionMoves, beforeActionBonusMoveActions;

    #endregion

    Hexagon lastSelectedHex;
    bool uiRefreshFlag, isFirstTime = true;
    [HideInInspector]
    public bool isRunning = false;
    string bottomLeftMsg;
    int maxPE = 25;

    Animator animator;

    private void Start()
    {
        currentWayPoint = startingWayPoint;
        cardsInHand = new List<CardController>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isRunning", false);
    }

    void Update()
    {
        if (energyPoints > maxPE)
            energyPoints = maxPE;

        switch (currentAction)
        {
            case Action.idle:
                selectedCard = null;
                isFirstTime = true;
                uiRefreshFlag = false;
                hasDiscount = false;
                break;

            case Action.start:

                #region Start

                if (isFirstTime)
                {
                    GainPhase();
                    DiscountCheck();
                    isFirstTime = false;
                }

                #endregion

                break;

            case Action.moving:

                #region Moving

                RaycastHit hitInfo;

                //moving ray
                if (Input.GetMouseButtonDown(0) && !isRunning)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, pointLayer))
                    {
                        Point pointHit = GameManager.instance.gridReference.GetPointFromWorldPosition(hitInfo.collider.transform.position);

                        if(pointHit != null && GameManager.instance.gridReference.GetPossibleDestinationsFromPoint(currentWayPoint).Contains(pointHit.worldPosition) && CheckIfPointIsWalkable(pointHit) && possibleMoves > 0)
                        {
                            StartCoroutine(RunAnimation(pointHit));
                        }
                    }
                }

                #endregion

                break;

            case Action.placeCard:

                #region Place Card

                RaycastHit placingHitInfo;

                //Se ho carta selezionata. E se l'ho selezionata da mano -->
                if (selectedCard && selectedCard.state == CardController.State.selectedFromHand && !hasPlacedCard)
                {
                    //(PLACE)
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out placingHitInfo, 100, hexLayer))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Hexagon hexHit = GameManager.instance.gridReference.GetHexagonFromWorldPosition(placingHitInfo.collider.transform.position);

                            if (hexHit != null)
                            {
                                if (hexHit.card == null)
                                {
                                    selectedCard.Place(hexHit);
                                    lastPlacedCard = selectedCard;
                                    selectedCard = null;
                                    hasPlacedCard = true;
                                    energyPoints += lastPlacedCard.extractableEnergy;
                                    bonusMoveActions += lastPlacedCard.moveHexTouched;
                                    if (UIrefresh != null)
                                    {
                                        UIrefresh(this);
                                    }
                                }
                            }
                        }
                    }

                    //(UNDO)
                    if (Input.GetMouseButtonDown(1) && !hasPlacedCard && selectedCard.state == CardController.State.selectedFromHand)
                    {
                        UnselectCard();
                    }
                }

                #endregion

                break;

            case Action.rotateCard:

                #region Rotate Card

                RaycastHit selectingHitInfo;

                //Se non ho carta selezionata. Quindi la seleziono dalla mappa -->
                if (!selectedCard)
                {
                    //(SELECT)
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out selectingHitInfo, 100, cardLayer))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            selectedCard = selectingHitInfo.collider.GetComponentInParent<CardController>();

                            if (selectedCard && selectedCard.state == CardController.State.placed && currentWayPoint.nearHexagons.Contains(selectedCard.hexImOn))
                            {
                                selectedCard.state = CardController.State.selectedFromMap;
                                selectedCard.FreePaths(selectedCard.hexImOn);
                                GameManager.instance.cardsManager.PlacedCards.Remove(selectedCard);
                                beforeRotateActionCardEulerAngle = selectedCard.placedEulerAngle;
                                if (UIrefresh != null)
                                {
                                    UIrefresh(this);
                                }
                                return;
                            }
                        }
                    }
                }

                //Se ho carta selezionata. E se l'ho selezionata dalla mappa -->
                if (selectedCard && selectedCard.state == CardController.State.selectedFromMap)
                {

                    //(PLACE)
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedCard.Place(selectedCard.hexImOn);
                        hasPlacedCard = true;
                        lastPlacedCard = selectedCard;
                        selectedCard = null;
                        energyPoints += lastPlacedCard.extractableEnergy;
                        if (UIrefresh != null)
                        {
                            UIrefresh(this);
                        }
                    }

                    //(UNDO)
                    if (Input.GetMouseButtonDown(1))
                    {
                        selectedCard.SetRotationBackToPlaced();
                        selectedCard.Place(selectedCard.hexImOn);
                        lastPlacedCard = selectedCard;
                        selectedCard = null;
                        if (UIrefresh != null)
                        {
                            UIrefresh(this);
                        }
                    }

                    //(RETURN TO OWNER'S HAND)
                    //if (Input.GetKeyDown(KeyCode.Space))
                    //{
                    //    UnselectCard();
                    //    hasPlacedCard = true;
                    //    if (UIrefresh != null)
                    //    {
                    //        UIrefresh(this);
                    //    }
                    //}

                }

                #endregion

                break;

            case Action.bet:

                #region Bet
                if (!hasBet)
                {
                    RaycastHit betHitInfo;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out betHitInfo, 100, playerLayer))
                    {

                        PlayerController playerHit = betHitInfo.collider.GetComponent<PlayerController>();

                        //Seleziono un player nemico da attaccare
                        if (Input.GetMouseButtonDown(0) && playerHit && playersToRob.Contains(playerHit))
                        {
                            hasBet = true;
                            if (UIrefresh != null)
                            {
                                UIrefresh(this);
                            }
                            StartCoroutine(GameManager.instance.Bet(this, playerHit));
                        }
                    }
                }

                #endregion

                break;
        }
    }

    #region Card Selection

    public void SelectCard(CardController card)
    {
        selectedCard = card;
        if (selectedCard.state == CardController.State.inHand)
        {
            selectedCard.state = CardController.State.selectedFromHand;
        }
        if (UIrefresh != null)
        {
            UIrefresh(this);
        }
    }

    public void UnselectCard()
    {
        selectedCard.state = CardController.State.inHand;
        selectedCard.transform.position = MyData.prefabsPosition;
        selectedCard = null;

        if (UIrefresh != null)
        {
            UIrefresh(this);
        }
    }

    public void SendCardInHand(CardController card)
    {
        cardsInHand.Add(card);
        card.FreePaths(card.hexImOn);
        card.hexImOn.card = null;
        card.state = CardController.State.inHand;
        card.transform.position = MyData.prefabsPosition;
        switch (card.type)
        {
            case CardController.Type.card1:
                numberOfCards1InHand++;
                break;
            case CardController.Type.card2:
                numberOfCards2InHand++;
                break;
            case CardController.Type.card3:
                numberOfCards3InHand++;
                break;
        }
        lastPlacedCard = null;

        if (UIrefresh != null)
        {
            UIrefresh(this);
        }
    }

    #endregion

    void DataStuffAfterMove(Point _pointHit)
    {
        currentWayPoint = _pointHit;

        if (currentWayPoint.isFinalWaypoint && IsMyColor(currentWayPoint))
            energyPoints++;

        if (currentWayPoint.type == Point.Type.purple)
        {
            possibleMoves = 0;
        }
        else
        if (currentWayPoint.type == Point.Type.win)
        {
            possibleMoves = 0;
            if (victoryPoints >= 5)
            {
                GameManager.instance.Win(this);
            }
        }
        else
            possibleMoves--;

        if (UIrefresh != null)
        {
            UIrefresh(this);
        }

        CustomLogger.Log("Mi trovo sul punto {0} , {1} di tipo {2}", currentWayPoint.x, currentWayPoint.y, currentWayPoint.type);
    }

    IEnumerator RunAnimation(Point targetPoint)
    {
        Vector3 target = targetPoint.worldPosition + Vector3.up * .7f;
        transform.DOLookAt(target, .2f);
        animator.SetBool("isRunning", true);
        isRunning = true;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        animator.SetBool("isRunning", false);
        isRunning = false;
        DataStuffAfterMove(targetPoint); 
    }

    public void SetNumberOfCardTypesInHand()
    {
        numberOfCards1InHand = numberOfCards2InHand = numberOfCards3InHand = 0;

        foreach (CardController card in cardsInHand)
        {
            switch (card.type)
            {
                case CardController.Type.card1:
                    numberOfCards1InHand++;
                    break;
                case CardController.Type.card2:
                    numberOfCards2InHand++;
                    break;
                case CardController.Type.card3:
                    numberOfCards3InHand++;
                    break;
            }
        }
    }

    bool CheckIfPointIsWalkable(Point point)
    {
        PlayerController[] players = GameManager.instance.players;

        foreach (PlayerController player in players)
        {
            if (player.currentWayPoint == point)
                return false;
        }

        if (point.type == Point.Type.win && victoryPoints < 5)
            return false;

        return true;
    }

    public bool IsMyColor(Point point)
    {

        if (type == Type.hypogeum && point.type == Point.Type.hypogeum)
            return true;
        if (type == Type.forest && point.type == Point.Type.forest)
            return true;
        if (type == Type.underwater && point.type == Point.Type.underwater)
            return true;
        if (type == Type.underground && point.type == Point.Type.underground)
            return true;

        return false;
    }

    public bool HasCardInNearHexagons()
    {
        List<Hexagon> nearHexagons = currentWayPoint.nearHexagons;

        foreach (Hexagon hex in nearHexagons)
        {
            if (hex.card)
                return true;
        }

        return false;
    }

    public void MoveToPoint(Point point)
    {
        transform.position = point.worldPosition + Vector3.up * .7f;
    }

    public void ChoseAction(int actionIndex)
    {
        switch (actionIndex)
        {
            case 0:
                currentAction = Action.moving;
                possibleMoves = beforeMoveActionMoves = 3;
                moveStartPoint = currentWayPoint;
                break;
            case 1:
                currentAction = Action.buyCard;
                beforeActionEnergyPoints = energyPoints;
                break;
            case 2:
                currentAction = Action.sellCard;
                beforeActionEnergyPoints = energyPoints;
                break;
            case 3:
                currentAction = Action.placeCard;
                beforeActionEnergyPoints = energyPoints;
                beforeActionBonusMoveActions = bonusMoveActions;
                GameManager.instance.mainCamera.SetHighView(true);
                break;
            case 4:
                currentAction = Action.rotateCard;
                beforeActionEnergyPoints = energyPoints;
                beforeActionBonusMoveActions = bonusMoveActions;
                GameManager.instance.mainCamera.SetHighView(true);
                break;
            case 5:
                currentAction = Action.bet;
                break;
        }

        if(UIrefresh != null)
        {
            UIrefresh(this);
        }

    }

    public void GainPhase()
    {
        hasPlacedCard = false;
        selectedCard = null;
        lastPlacedCard = null;
        hasBet = false;
        actions = 2;
        bonusMoveActions = 0;
        GameManager.instance.cardsManager.GainPhase(this);
        if (UIrefresh != null)
        {
            UIrefresh(this);
        }
    }

    void DiscountCheck()
    {
        bool haveCardsPlaced = false;

        foreach (CardController card in GameManager.instance.cardsManager.PlacedCards)
        {
            if (card.player == this)
                haveCardsPlaced = true;
        }

        if (energyPoints <= 1 && !haveCardsPlaced)
            hasDiscount = true;
    }

    public void AddCard(CardController card)
    {
        cardsInHand.Add(card);
        card.state = CardController.State.inHand;
    }
}
