using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using DG.Tweening;
using cakeslice;

public class PlayerController : MonoBehaviour
{
    public Sprite icon;
    public Sprite winIcon;
    public float moveSpeed;

    public enum Type
    {
        hypogeum, underwater, forest, underground
    }

    public enum Action
    {
        idle, start, moving, buyCard, placeCard, rotateCard, fight
    }

    #region Public Variables

    public Type type;
    public GameObject VFXobject;
    public OutlineController outlineController;
    public int stepPerMove;
    [HideInInspector]
    public Point startingWayPoint;
    [HideInInspector]
    public CapsuleCollider myCollider;
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
    public bool hasPlacedCard, hasFought, canBet, hasBought, isBonusMove, hasSold, hasDiscount;
    [HideInInspector]
    public List<CardController> cardsInHand;
    [HideInInspector]
    public CardController selectedCard, lastPlacedCard, cardToSell;
    [HideInInspector]
    public List<PlayerController> playersToRob = new List<PlayerController>();
    public LayerMask pointLayer, hexLayer, cardLayer, playerLayer, UIlayer;
    [HideInInspector]
    public int beforeActionEnergyPoints, beforeRotateActionCardEulerAngle, beforeMoveActionMoves, beforeActionBonusMoveActions;

    #endregion

    public int VictoryPoints
    {
        get
        {
            return victoryPoints;
        }
        set
        {
            victoryPoints = value;
            GameManager.instance.CheckPlayersPV();
        }
    }

    public int EnergyPoints
    {
        get
        {
            return energyPoints;
        }
        set
        {
            energyPoints = value;
            if (energyPoints >= 25)
                energyPoints = 25;
        }
    }

    [HideInInspector]
    public bool isRunning = false, hasMoved = false;


    Animator animator;
    List<Point> walkablePointMap = new List<Point>();

    private void Awake()
    {
        myCollider = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        currentAction = Action.idle;
        currentWayPoint = startingWayPoint;
        cardsInHand = new List<CardController>();
        if (animator)
            animator.SetBool("isRunning", false);
    }

    void Update()
    {
        switch (currentAction)
        {
            case Action.moving:

                #region Moving

                RaycastHit hitInfo;

                //moving ray
                if (Input.GetMouseButtonDown(0) && !hasMoved)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, pointLayer))
                    {
                        Point pointHit = GameManager.instance.gridReference.GetPointFromWorldPosition(hitInfo.collider.transform.position);
                        SmoothMoveAnimation finalPointHit = hitInfo.collider.GetComponent<SmoothMoveAnimation>();

                        if (pointHit != null)
                        {
                            foreach (var point in walkablePointMap)
                            {
                                if (point == pointHit)
                                {
                                    if (point == currentWayPoint || (point.isFinalWaypoint && point.isFinalWaypointUsed))
                                    {
                                        return;
                                    }

                                    if (finalPointHit != null)
                                    {
                                        if (Mathf.Approximately(pointHit.worldPosition.x, finalPointHit.transform.position.x) && Mathf.Approximately(pointHit.worldPosition.y, finalPointHit.transform.position.y))
                                            StartCoroutine(RunAnimation(pointHit, finalPointHit));
                                    }
                                    else
                                    {
                                        StartCoroutine(FollowPath(GameManager.instance.pathfinding.FindPath(currentWayPoint, pointHit, walkablePointMap.Count)));
                                    }
                                    if (GameManager.instance.OnMoveSelected != null)
                                        GameManager.instance.OnMoveSelected();
                                    GameManager.instance.hudManager.Refresh();
                                    hasMoved = true;
                                    break;
                                }
                            }
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
                                    EnergyPoints += lastPlacedCard.extractableEnergy;
                                    bonusMoveActions += lastPlacedCard.moveHexTouched;
                                    GameManager.instance.ConfirmAction();
                                }
                            }
                        }
                    }

                    //(UNDO)
                    if (Input.GetMouseButtonDown(1) && !hasPlacedCard && selectedCard.state == CardController.State.selectedFromHand)
                    {
                        UnselectCard();
                        GameManager.instance.UndoAction();
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
                                selectedCard.SelectFromMap();
                                beforeRotateActionCardEulerAngle = selectedCard.placedEulerAngle;
                                GameManager.instance.hudManager.Refresh();
                                return;
                            }
                            selectedCard = null;
                        }
                    }
                }

                //Se ho carta selezionata. E se l'ho selezionata dalla mappa -->
                if (selectedCard && selectedCard.state == CardController.State.selectedFromMap)
                {

                    //(PLACE)
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out selectingHitInfo, 100, UIlayer))
                        {
                            return;
                        }
                        else
                        if (selectedCard.Place(selectedCard.hexImOn))
                        {
                            hasPlacedCard = true;
                            lastPlacedCard = selectedCard;
                            selectedCard = null;
                            EnergyPoints += lastPlacedCard.extractableEnergy;
                            bonusMoveActions += lastPlacedCard.moveHexTouched;
                            GameManager.instance.ConfirmAction();
                        }
                    }
                }

                #endregion

                break;

            case Action.fight:

                #region Fight

                if (!hasFought)
                {
                    RaycastHit betHitInfo;

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out betHitInfo, 100, playerLayer))
                    {

                        PlayerController playerHit = betHitInfo.collider.GetComponent<PlayerController>();

                        //Seleziono un player nemico da attaccare
                        if (Input.GetMouseButtonDown(0) && playerHit && playersToRob.Contains(playerHit))
                        {
                            hasFought = true;
                            GameManager.instance.StartFight(this, playerHit);
                            GameManager.instance.hudManager.Refresh();
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

        GameManager.instance.hudManager.Refresh();
    }

    public void UnselectCard()
    {
        selectedCard.state = CardController.State.inHand;
        selectedCard.transform.position = MyData.prefabsPosition + Vector3.one * GameManager.instance.cardSpawnCounter * 2;
        selectedCard = null;

        GameManager.instance.hudManager.Refresh();
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

        GameManager.instance.hudManager.Refresh();
    }

    #endregion

    void DataStuffAfterMove(Point _pointHit)
    {
        currentWayPoint = _pointHit;

        playersToRob = GameManager.instance.FindPlayersInRange(2, this);

        if (currentWayPoint.isFinalWaypoint && IsMyColor(currentWayPoint))
        {
            VictoryPoints++;
            currentWayPoint.isFinalWaypointUsed = true;
            GameManager.instance.ConfirmAction();
        }

        if (currentWayPoint.type == Point.Type.purple)
        {
            possibleMoves = 0;
        }
        else
        if (currentWayPoint.type == Point.Type.win)
        {
            possibleMoves = 0;
            if (VictoryPoints >= 5)
            {
                GameManager.instance.Win(this);
            }
        }
        else
            possibleMoves--;

        GameManager.instance.hudManager.Refresh();

        CustomLogger.Log("Mi trovo sul punto {0} , {1} di tipo {2}", currentWayPoint.x, currentWayPoint.y, currentWayPoint.type);
    }

    IEnumerator FollowPath(List<Point> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            yield return StartCoroutine(RunAnimation(path[i]));
        }

        GameManager.instance.ConfirmAction();
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

    IEnumerator RunAnimation(Point targetPoint, SmoothMoveAnimation final)
    {
        Vector3 target = targetPoint.worldPosition + Vector3.up * .7f;
        transform.DOLookAt(target, .2f);

        yield return StartCoroutine(final.Animation());

        yield return new WaitForSeconds(.7f);

        animator.SetBool("isRunning", true);
        isRunning = true;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        animator.SetBool("isRunning", false);
        isRunning = false;
        final.DestroySphere();
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
                hasMoved = false;
                possibleMoves = beforeMoveActionMoves = stepPerMove;
                moveStartPoint = currentWayPoint;
                walkablePointMap = GameManager.instance.FindWalkablePointsInRange(possibleMoves, this);
                if (GameManager.instance.OnMoveEnter != null)
                    GameManager.instance.OnMoveEnter(walkablePointMap);
                if (bonusMoveActions > 0)
                    isBonusMove = true;
                break;
            case 1:
                currentAction = Action.buyCard;
                beforeActionEnergyPoints = EnergyPoints;
                break;
            case 3:
                currentAction = Action.placeCard;
                beforeActionEnergyPoints = EnergyPoints;
                beforeActionBonusMoveActions = bonusMoveActions;
                GameManager.instance.mainCamera.SetHighView(true);
                GameManager.instance.mainCamera.canChangeView = false;
                break;
            case 4:
                currentAction = Action.rotateCard;
                beforeActionEnergyPoints = EnergyPoints;
                beforeActionBonusMoveActions = bonusMoveActions;
                GameManager.instance.cardsManager.HighlightPlacedCards(currentWayPoint.nearHexagons, true);
                GameManager.instance.mainCamera.SetHighView(true);
                GameManager.instance.mainCamera.canChangeView = false;
                break;
            case 5:
                currentAction = Action.fight;
                GameManager.instance.mainCamera.canChangeView = false;
                OutlineEffectController.instance.ChangeLineAlphaCutoff(.7f);
                OutlineEffectController.instance.ChangeLineThickness(2.5f);
                OutlineEffectController.instance.ChangeLineColor(2, Color.white);
                OutlineEffectController.instance.ChangeLineColor(1, new Color(.95f,.95f,.95f,1));
                foreach (var player in playersToRob)
                {
                    player.myCollider.enabled = true;
                    player.outlineController.SetColor(1);
                    player.outlineController.EnableOutline(true);
                }
                break;
        }

        GameManager.instance.hudManager.ToggleActionButtons();

    }

    public void ResetValues()
    {
        actions = 2;
        bonusMoveActions = 0;
        hasDiscount = DiscountCheck();
    }

    public void TurnStart()
    {
        currentAction = Action.start;
        hasPlacedCard = false;
        selectedCard = null;
        lastPlacedCard = null;
        hasFought = false;
        playersToRob = GameManager.instance.FindPlayersInRange(2, this);
        GameManager.instance.hudManager.ToggleActionButtons();
    }

    public bool DiscountCheck()
    {
        bool hasCards = false;

        foreach (CardController card in GameManager.instance.cardsManager.PlacedCards)
        {
            if (card.player == this)
                hasCards = true;
        }

        if (cardsInHand.Count > 0)
            hasCards = true;

        if (EnergyPoints <= 0 && !hasCards && cardsInHand.Count <= 0)
            return true;
        else
            return false;
    }

    public void AddCard(CardController card)
    {
        cardsInHand.Add(card);
        card.state = CardController.State.inHand;
    }
}
