using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using EZCameraShake;

public class CardController : MonoBehaviour
{
    public ResourcePopUp popUp1;
    public ResourcePopUp popUp2;
    public ResourcePopUp popUp3;
    public Image cardImage;
    public GameObject DestroyParticle;
    

    public OutlineController outlineController;

    [Header("Sounds")]
    public AudioSource PlaceSound;
    public AudioSource RotationPhaseSound;
    public AudioSource Rotate;

    public UnityEvent OnCardDestroy;

    int eulerAngle = 0;
    public int placedEulerAngle;
    [HideInInspector]
    public int extractableEnergy = 0, moveHexTouched = 0, abilityHexTouched = 0;
    [HideInInspector]
    public Hexagon hexImOn;

    BoxCollider[] armsColliders;

    public enum State
    {
        inHand, selectedFromHand, selectedFromMap, placed
    }

    public enum Type
    {
        card1, card2, card3
    }

    [HideInInspector]
    public State _state = State.inHand;
    public Type type;
    [HideInInspector]
    public PlayerController _player;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public bool closeAnimFinished = false, openAnimFinished = false, rotateRightFlowFinished = false;

    public State state
    {
        set
        {
            _state = value;
            if(_state == State.inHand)
            {
                foreach (var col in armsColliders)
                {
                    col.enabled = false;
                }
            }
            else
            {
                foreach (var col in armsColliders)
                {
                    col.enabled = true;
                }
            }
        }
        get
        {
            return _state;
        }
    }

    public PlayerController player
    {
        set
        {
            _player = value;
            cardImage.sprite = _player.icon;
        }
        get
        {
            return _player;
        } 
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        armsColliders = GetComponentsInChildren<BoxCollider>();
        animator.SetTrigger("Open");
    }

    void Update()
    {
        if (state == State.selectedFromHand)
        {
            Vector3 myMousePos = Input.mousePosition;
            myMousePos.z = 28f;
            transform.position = Camera.main.ScreenToWorldPoint(myMousePos);
            if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.A))
            {
                transform.Rotate(Vector3.up * -60);
                eulerAngle -= 60;
                if (eulerAngle == 360 || eulerAngle == -360)
                    eulerAngle = 0;
                Rotate.Play();
                return;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKeyDown(KeyCode.D))
            {
                transform.Rotate(Vector3.up * 60);
                eulerAngle += 60;
                if (eulerAngle == 360 || eulerAngle == -360)
                    eulerAngle = 0;
                Rotate.Play();
                return;
            }
        }

        if (state == State.selectedFromMap)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.A))
            {
                transform.Rotate(Vector3.up * -60);
                eulerAngle -= 60;
                if (eulerAngle == 360 || eulerAngle == -360)
                    eulerAngle = 0;
                Rotate.Play();
                print("card rotated!");
                return;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKeyDown(KeyCode.D))
            {
                transform.Rotate(Vector3.up * 60);
                eulerAngle += 60;
                if (eulerAngle == 360 || eulerAngle == -360)
                    eulerAngle = 0;
                Rotate.Play();
                return;
            }
        }
    }

    public bool Place(Hexagon hex)
    {
        if (state == State.selectedFromHand)
        {
            BlockPaths(hex);
            state = State.placed;
            placedEulerAngle = eulerAngle;
            hexImOn = hex;
            hexImOn.card = this;
            ResetTouchValues();
            SetTouchvalues(hexImOn);
            if(player.cardsInHand.Contains(this))
                player.cardsInHand.Remove(this);
            GameManager.instance.cardsManager.PlacedCards.Add(this);

            //ANIMATION
            StartCoroutine(AnimateToDestination(hexImOn.worldPosition /*+ Vector3.up * .5f*/, 12f, true, false));
            return true;
        }
        else if(state == State.selectedFromMap)
        {
            if(eulerAngle != placedEulerAngle)
            {
                BlockPaths(hex);
                state = State.placed;
                placedEulerAngle = eulerAngle;
                hexImOn = hex;
                hexImOn.card = this;
                ResetTouchValues();
                SetTouchvalues(hexImOn);
                if (player.cardsInHand.Contains(this))
                    player.cardsInHand.Remove(this);
                GameManager.instance.cardsManager.PlacedCards.Add(this);

                //ANIMATION
                StartCoroutine(AnimateToDestination(hexImOn.worldPosition /*+ Vector3.up * .5f*/, 12f, true, true));
                return true;
            }
            else
            {
                GameManager.instance.hudManager.PrintMediumNews("Change rotation before placing!");
                return false;
            }
        }
        return false;
    }

    public void BlockPaths(Hexagon hex)
    {
        List<Point> pointsAroundCard = GameManager.instance.gridReference.GetSixPointsAroundHexagon(hex);

        Point bottom = null;
        Point bottomRight = null;
        Point bottomLeft = null;
        Point topRight = null;
        Point topLeft = null;
        Point top = null;

        if (hex.y % 2 == 0)
        {
            foreach (Point point in pointsAroundCard)
            {
                if (point.x == hex.x * 2 && point.y == hex.y * 2)
                {
                    bottom = point;
                }
                if (point.x == hex.x * 2 && point.y == hex.y * 2 + 3)
                {
                    top = point;
                }
                if (point.x == hex.x * 2 + 1 && point.y == hex.y * 2 + 1)
                {
                    bottomRight = point;
                }
                if (point.x == hex.x * 2 - 1 && point.y == hex.y * 2 + 1)
                {
                    bottomLeft = point;
                }
                if (point.x == hex.x * 2 + 1 && point.y == hex.y * 2 + 2)
                {
                    topRight = point;
                }
                if (point.x == hex.x * 2 - 1 && point.y == hex.y * 2 + 2)
                {
                    topLeft = point;
                }
            }
        }
        else
        if (hex.y % 2 != 0)
        {
            foreach (Point point in pointsAroundCard)
            {
                if (point.x == hex.x * 2 + 1 && point.y == hex.y * 2)
                {
                    bottom = point;
                }
                if (point.x == hex.x * 2 + 1 && point.y == hex.y * 2 + 3)
                {
                    top = point;
                }
                if (point.x == hex.x * 2 + 1 + 1 && point.y == hex.y * 2 + 1)
                {
                    bottomRight = point;
                }
                if (point.x == hex.x * 2 - 1 + 1 && point.y == hex.y * 2 + 1)
                {
                    bottomLeft = point;
                }
                if (point.x == hex.x * 2 + 1 + 1 && point.y == hex.y * 2 + 2)
                {
                    topRight = point;
                }
                if (point.x == hex.x * 2 - 1 + 1 && point.y == hex.y * 2 + 2)
                {
                    topLeft = point;
                }
            }
        }

        if (type == Type.card1)
        {
            switch (eulerAngle)
            {
                case 0:
                    if (bottomRight != null && topRight != null)
                    {
                        bottomRight.possibleDestinations.Remove(topRight);
                        topRight.possibleDestinations.Remove(bottomRight);
                    }
                    break;
                case 60:
                case -300:
                    if (bottomRight != null && bottom != null)
                    {
                        bottomRight.possibleDestinations.Remove(bottom);
                        bottom.possibleDestinations.Remove(bottomRight);
                    }
                    break;
                case 120:
                case -240:
                    if (bottomLeft != null && bottom != null)
                    {
                        bottomLeft.possibleDestinations.Remove(bottom);
                        bottom.possibleDestinations.Remove(bottomLeft);
                    }
                    break;
                case 180:
                case -180:
                    if (bottomLeft != null && topLeft != null)
                    {
                        bottomLeft.possibleDestinations.Remove(topLeft);
                        topLeft.possibleDestinations.Remove(bottomLeft);
                    }
                    break;
                case 240:
                case -120:
                    if (top != null && topLeft != null)
                    {
                        topLeft.possibleDestinations.Remove(top);
                        top.possibleDestinations.Remove(topLeft);
                    }
                    break;
                case 300:
                case -60:
                    if (top != null && topRight != null)
                    {
                        top.possibleDestinations.Remove(topRight);
                        topRight.possibleDestinations.Remove(top);
                    }
                    break;
            }
        }
        else
        if (type == Type.card2)
        {
            switch (eulerAngle)
            {
                case 0:
                    if (bottomRight != null && topRight != null)
                    {
                        bottomRight.possibleDestinations.Remove(topRight);
                        topRight.possibleDestinations.Remove(bottomRight);
                    }
                    if (topLeft != null && top != null)
                    {
                        topLeft.possibleDestinations.Remove(top);
                        top.possibleDestinations.Remove(topLeft);
                    }
                    break;
                case 60:
                case -300:
                    if (bottomRight != null && bottom != null)
                    {
                        bottomRight.possibleDestinations.Remove(bottom);
                        bottom.possibleDestinations.Remove(bottomRight);
                    }
                    if (top != null && topRight != null)
                    {
                        top.possibleDestinations.Remove(topRight);
                        topRight.possibleDestinations.Remove(top);
                    }
                    break;
                case 120:
                case -240:
                    if (bottomLeft != null && bottom != null)
                    {
                        bottomLeft.possibleDestinations.Remove(bottom);
                        bottom.possibleDestinations.Remove(bottomLeft);
                    }
                    if (bottomRight != null && topRight != null)
                    {
                        bottomRight.possibleDestinations.Remove(topRight);
                        topRight.possibleDestinations.Remove(bottomRight);
                    }
                    break;
                case 180:
                case -180:
                    if (bottomLeft != null && topLeft != null)
                    {
                        bottomLeft.possibleDestinations.Remove(topLeft);
                        topLeft.possibleDestinations.Remove(bottomLeft);
                    }
                    if (bottomRight != null && bottom != null)
                    {
                        bottomRight.possibleDestinations.Remove(bottom);
                        bottom.possibleDestinations.Remove(bottomRight);
                    }
                    break;
                case 240:
                case -120:
                    if (topLeft != null && top != null)
                    {
                        topLeft.possibleDestinations.Remove(top);
                        top.possibleDestinations.Remove(topLeft);
                    }
                    if (bottomLeft != null && bottom != null)
                    {
                        bottomLeft.possibleDestinations.Remove(bottom);
                        bottom.possibleDestinations.Remove(bottomLeft);
                    }
                    break;
                case 300:
                case -60:
                    if (top != null && topRight != null)
                    {
                        top.possibleDestinations.Remove(topRight);
                        topRight.possibleDestinations.Remove(top);
                    }
                    if (bottomLeft != null && topLeft != null)
                    {
                        bottomLeft.possibleDestinations.Remove(topLeft);
                        topLeft.possibleDestinations.Remove(bottomLeft);
                    }
                    break;
            }
        }
        else
        if (type == Type.card3)
        {
            switch (eulerAngle)
            {
                case 0:
                case 120:
                case -240:
                case 240:
                case -120:
                    if (bottomRight != null && topRight != null)
                    {
                        bottomRight.possibleDestinations.Remove(topRight);
                        topRight.possibleDestinations.Remove(bottomRight);
                    }
                    if (topLeft != null && top != null)
                    {
                        topLeft.possibleDestinations.Remove(top);
                        top.possibleDestinations.Remove(topLeft);
                    }
                    if (bottomLeft != null && bottom != null)
                    {
                        bottomLeft.possibleDestinations.Remove(bottom);
                        bottom.possibleDestinations.Remove(bottomLeft);
                    }
                    break;
                case 60:
                case -300:
                case 180:
                case -180:
                case 300:
                case -60:
                    if (bottomRight != null && bottom != null)
                    {
                        bottomRight.possibleDestinations.Remove(bottom);
                        bottom.possibleDestinations.Remove(bottomRight);
                    }
                    if (top != null && topRight != null)
                    {
                        top.possibleDestinations.Remove(topRight);
                        topRight.possibleDestinations.Remove(top);
                    }
                    if (bottomLeft != null && topLeft != null)
                    {
                        bottomLeft.possibleDestinations.Remove(topLeft);
                        topLeft.possibleDestinations.Remove(bottomLeft);
                    }
                    break;
            }
        }
    }

    public void FreePaths(Hexagon hex)
    {
        List<Point> pointsAroundCard = GameManager.instance.gridReference.GetSixPointsAroundHexagon(hex);

        foreach (Point point in pointsAroundCard)
        {
            point.possibleDestinations = GameManager.instance.gridReference.GetPossibleDestinationsFromPoint(point);
        }

        ResetTouchValues();
    }

    public IEnumerator RotateRight()
    {
        ResetAnimationFlags();
        FreePaths(hexImOn);
        animator.SetTrigger("Close");

        while (!closeAnimFinished)
            yield return null;
        
        yield return StartCoroutine(AnimateToRotation((transform.rotation.eulerAngles + Vector3.up * 60), 1.3f));

        eulerAngle += 60;
        if (eulerAngle == 360 || eulerAngle == -360)
            eulerAngle = 0;

        animator.SetTrigger("Open");

        while (!openAnimFinished)
            yield return null;

        BlockPaths(hexImOn);
        placedEulerAngle = eulerAngle;
        ResetTouchValues();
        SetTouchvalues(hexImOn);
        rotateRightFlowFinished = true;
    }

    public IEnumerator WaitForResourcePopUp()
    {
        int popUpCounter = 0;

        bool popUp1Counted = false;
        bool popUp2Counted = false;
        bool popUp3Counted = false;

        switch (type)
        {
            case Type.card1:
                while (true)
                {
                    if (popUp1 && popUp1.animFinished)
                        break;
                    yield return null;
                }
                break;
            case Type.card2:
                popUp1Counted = false;
                popUp2Counted = false;
                while (popUpCounter < 2)
                {
                    if (popUp1 && popUp1.animFinished && !popUp1Counted)
                    {
                        popUpCounter++;
                        popUp1Counted = true;
                    }
                    if (popUp2 && popUp2.animFinished && !popUp2Counted)
                    {
                        popUpCounter++;
                        popUp2Counted = true;
                    }
                    yield return null;
                }
                break;
            case Type.card3:
                popUp1Counted = false;
                popUp2Counted = false;
                popUp3Counted = false;
                while (popUpCounter < 3)
                {
                    if (popUp1 && popUp1.animFinished && !popUp1Counted)
                    {
                        popUpCounter++;
                        popUp1Counted = true;
                    }
                    if (popUp2 && popUp2.animFinished && !popUp2Counted)
                    {
                        popUpCounter++;
                        popUp2Counted = true;
                    }
                    if (popUp3 && popUp3.animFinished && !popUp3Counted)
                    {
                        popUpCounter++;
                        popUp3Counted = true;
                    }
                    yield return null;
                }
                break;
        }
    }

    public void CloseAnimationEnd()
    {
        closeAnimFinished = true;
    }

    public void OpenAnimationEnd()
    {
        openAnimFinished = true;
    }

    public void ResetAnimationFlags()
    {
        closeAnimFinished = false;
        openAnimFinished = false;
        rotateRightFlowFinished = false;
    }

    public void SetTouchvalues(Hexagon myHex)
    {
        ResetResourcePopUpStrings();
        List<Hexagon> aroundHexes = GameManager.instance.gridReference.GetHexagonsAroundHexagon(myHex);

        Hexagon topLeft = new Hexagon();
        Hexagon topRight = new Hexagon();
        Hexagon left = new Hexagon();
        Hexagon right = new Hexagon();
        Hexagon botLeft = new Hexagon();
        Hexagon botRight = new Hexagon();

        //find all hex around me
        if (myHex.y % 2 != 0)
        {
            foreach (Hexagon hex in aroundHexes)
            {
                if (hex.y == myHex.y - 1 && hex.x == myHex.x)
                {
                    botLeft = hex;
                }
                else
                if (hex.y == myHex.y - 1 && hex.x == myHex.x + 1)
                {
                    botRight = hex;
                }
                else
                if (hex.y == myHex.y && hex.x == myHex.x + 1)
                {
                    right = hex;
                }
                else
                if (hex.y == myHex.y && hex.x == myHex.x - 1)
                {
                    left = hex;
                }
                else
                if (hex.y == myHex.y + 1 && hex.x == myHex.x)
                {
                    topLeft = hex;
                }
                else
                if (hex.y == myHex.y + 1 && hex.x == myHex.x + 1)
                {
                    topRight = hex;
                }
            }
        }
        else
            if (myHex.y % 2 == 0)
        {
            foreach (Hexagon hex in aroundHexes)
            {
                if (hex.y == myHex.y - 1 && hex.x == myHex.x)
                {
                    botRight = hex;
                }
                else
                if (hex.y == myHex.y - 1 && hex.x == myHex.x - 1)
                {
                    botLeft = hex;
                }
                else
                if (hex.y == myHex.y && hex.x == myHex.x + 1)
                {
                    right = hex;
                }
                else
                if (hex.y == myHex.y && hex.x == myHex.x - 1)
                {
                    left = hex;
                }
                else
                if (hex.y == myHex.y + 1 && hex.x == myHex.x)
                {
                    topRight = hex;
                }
                else
                if (hex.y == myHex.y + 1 && hex.x == myHex.x - 1)
                {
                    topLeft = hex;
                }
            }
        }

        //set values
        switch (type)
        {
            case Type.card1:
                switch (placedEulerAngle)
                {
                    case 0:
                        if (right != null && !right.card)
                            switch (right.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        break;
                    case 60:
                    case -300:
                        if (botRight != null && !botRight.card)
                            switch (botRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE",Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        break;
                    case 120:
                    case -240:
                        if (botLeft != null && !botLeft.card)
                            switch (botLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        break;
                    case 180:
                    case -180:
                        if (left != null && !left.card)
                            switch (left.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        break;
                    case 240:
                    case -120:
                        if (topLeft != null && !topLeft.card)
                            switch (topLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        break;
                    case 300:
                    case -60:
                        if (topRight != null && !topRight.card)
                            switch (topRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        break;
                }
                break;
            case Type.card2:
                switch (placedEulerAngle)
                {
                    case 0:
                        if (right != null && !right.card)
                            switch (right.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (topLeft != null && !topLeft.card)
                            switch (topLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        break;
                    case 60:
                    case -300:
                        if (botRight != null && !botRight.card)
                            switch (botRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (topRight != null && !topRight.card)
                            switch (topRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        break;
                    case 120:
                    case -240:
                        if (botLeft != null && !botLeft.card)
                            switch (botLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (right != null && !right.card)
                            switch (right.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        break;
                    case 180:
                    case -180:
                        if (left != null && !left.card)
                            switch (left.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (botRight != null && !botRight.card)
                            switch (botRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        break;
                    case 240:
                    case -120:
                        if (topLeft != null && !topLeft.card)
                            switch (topLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (botLeft != null && !botLeft.card)
                            switch (botLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        break;
                    case 300:
                    case -60:
                        if (topRight != null && !topRight.card)
                            switch (topRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (left != null && !left.card)
                            switch (left.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        break;
                }
                break;
            case Type.card3:
                switch (placedEulerAngle)
                {
                    case 0:
                        if (right != null && !right.card)
                            switch (right.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (topLeft != null && !topLeft.card)
                            switch (topLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        if (botLeft != null && !botLeft.card)
                            switch (botLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp3 != null)
                                    {
                                        popUp3.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp3 != null)
                                        popUp3.ResetString();
                                    break;
                            }
                        break;
                    case 60:
                    case -300:
                        if (botRight != null && !botRight.card)
                            switch (botRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (topRight != null && !topRight.card)
                            switch (topRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        if (left != null && !left.card)
                            switch (left.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp3 != null)
                                    {
                                        popUp3.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp3 != null)
                                        popUp3.ResetString();
                                    break;
                            }
                        break;
                    case 120:
                    case -240:
                        if (botLeft != null && !botLeft.card)
                            switch (botLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (right != null && !right.card)
                            switch (right.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        if (topLeft != null && !topLeft.card)
                            switch (topLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp3 != null)
                                    {
                                        popUp3.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp3 != null)
                                        popUp3.ResetString();
                                    break;
                            }
                        break;
                    case 180:
                    case -180:
                        if (left != null && !left.card)
                            switch (left.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (botRight != null && !botRight.card)
                            switch (botRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        if (topRight != null && !topRight.card)
                            switch (topRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp3 != null)
                                    {
                                        popUp3.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp3 != null)
                                        popUp3.ResetString();
                                    break;
                            }
                        break;
                    case 240:
                    case -120:
                        if (topLeft != null && !topLeft.card)
                            switch (topLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (botLeft != null && !botLeft.card)
                            switch (botLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        if (right != null && !right.card)
                            switch (right.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp3 != null)
                                    {
                                        popUp3.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp3 != null)
                                        popUp3.ResetString();
                                    break;
                            }
                        break;
                    case 300:
                    case -60:
                        if (topRight != null && !topRight.card)
                            switch (topRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp1 != null)
                                    {
                                        popUp1.ResetString();
                                        return;
                                    }
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp1 != null)
                                        popUp1.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp1 != null)
                                        popUp1.ResetString();
                                    break;
                            }
                        if (left != null && !left.card)
                            switch (left.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp2 != null)
                                    {
                                        popUp2.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp2 != null)
                                        popUp2.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp2 != null)
                                        popUp2.ResetString();
                                    break;
                            }
                        if (botRight != null && !botRight.card)
                            switch (botRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 PE", Color.green);
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    if (GameManager.instance.currentPhase != GameManager.Phase.rotation && popUp3 != null)
                                    {
                                        popUp3.ResetString();
                                        return;
                                    }
                                    abilityHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Ability", Color.blue);
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    if (popUp3 != null)
                                        popUp3.SetString("+1 Move", Color.white);
                                    break;
                                case Hexagon.Type.empty:
                                    if (popUp3 != null)
                                        popUp3.ResetString();
                                    break;
                            }
                        break;
                }
                break;
        }
    }

    public void SetRotationBackToPlaced()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, (float)placedEulerAngle, 0));
        eulerAngle = placedEulerAngle;
    }

    public void ResetTouchValues()
    {
        extractableEnergy = 0;
        abilityHexTouched = 0;
        moveHexTouched = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        CardController cardHit = other.GetComponentInParent<CardController>();
        if (cardHit != null)
        {
            GameManager.instance.cardsManager.PlacedCards.Remove(this);
            Instantiate(DestroyParticle, transform.position, Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)));
            CameraShaker.Instance.ShakeOnce(2f, 10f, .5f, .5f);
            FreePaths(hexImOn);
            Destroy(this.gameObject);
        }  
    }

    public void SelectFromMap()
    {
        state = State.selectedFromMap;
        this.FreePaths(this.hexImOn);
        GameManager.instance.cardsManager.PlacedCards.Remove(this);
        StartCoroutine(AnimateToDestination(new Vector3(transform.position.x, 3f, transform.position.z), 8f, false));
    }

    public void ResetResourcePopUpStrings()
    {
        if (popUp1 != null)
            popUp1.ResetString();
        if (popUp2 != null)
            popUp2.ResetString();
        if (popUp3 != null)
            popUp3.ResetString();
    }

    #region animations

    IEnumerator AnimateToDestination(Vector3 Destination, float speed, bool playPopUpAnim, bool stealFromPlayer)
    {
        while (transform.position != Destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, Destination, speed * Time.deltaTime);
            yield return null;
        }
        
        if (playPopUpAnim)
        {
            PlaceSound.Play();
            ResourcesPopUpAnimation();
            CameraShaker.Instance.ShakeOnce(2, 3, 0, 1f, Vector3.up * .5f, Vector3.one * .5f);
        }
        if (!stealFromPlayer)
        {
            player.EnergyPoints += extractableEnergy;
            player.BonusMoveActions += moveHexTouched;
        }
        else
        {
            GameManager.instance.currentActivePlayer.EnergyPoints += extractableEnergy;
            GameManager.instance.currentActivePlayer.BonusMoveActions += moveHexTouched;
        }
        GameManager.instance.hudManager.Refresh();
        GameManager.instance.playersHUDcontroller.RefreshPlayerUIs();
        GameManager.instance.playersHUDcontroller.ActivePlayerUI.Refresh();
    }

    IEnumerator AnimateToDestination(Vector3 Destination, float speed, bool playPopUpAnim)
    {
        while (transform.position != Destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, Destination, speed * Time.deltaTime);
            yield return null;
        }
        
        if (playPopUpAnim)
        {
            ResourcesPopUpAnimation();
            PlaceSound.Play();
            CameraShaker.Instance.ShakeOnce(2, 3, 0, 1f, Vector3.up * .5f, Vector3.one * .5f);
        }

        GameManager.instance.hudManager.Refresh();
        GameManager.instance.playersHUDcontroller.RefreshPlayerUIs();
        GameManager.instance.playersHUDcontroller.ActivePlayerUI.Refresh();
    }

    IEnumerator AnimateToRotation(Vector3 TargetEulerAngle, float durationInSeconds)
    {
        float counter = 0f;
        Quaternion startingRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(TargetEulerAngle);
        RotationPhaseSound.Play();
        while (counter < durationInSeconds)
        {
            counter += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, counter / durationInSeconds);
           yield return null;
        }
        RotationPhaseSound.Stop();
    }

    public void ResourcesPopUpAnimation()
    {
        if(popUp1 != null)
        {
            if (popUp1.resourcePopUpText.text == "" || popUp1.resourcePopUpText.text == null)
                popUp1.animFinished = true;
            else
            {
                popUp1.animFinished = false;
                popUp1.animator.SetTrigger("PopUp");
                print("parte anim popup1");
            }
        }
        if (popUp2 != null)
        {
            if (popUp2.resourcePopUpText.text == "" || popUp2.resourcePopUpText.text == null)
                popUp2.animFinished = true;
            else
            {
                popUp2.animFinished = false;
                popUp2.animator.SetTrigger("PopUp");
                print("parte anim popup2");
            }
        }
        if (popUp3 != null)
        {
            if (popUp3.resourcePopUpText.text == "" || popUp3.resourcePopUpText.text == null)
                popUp3.animFinished = true;
            else
            {
                popUp3.animFinished = false;
                popUp3.animator.SetTrigger("PopUp");
                print("parte anim popup3");
            }
        }
    }

    #endregion

}