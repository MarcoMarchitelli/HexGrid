using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CardController : MonoBehaviour
{
    int eulerAngle = 0;
    [HideInInspector]
    public int placedEulerAngle;
    [HideInInspector]
    public int extractableEnergy = 0, moveHexTouched = 0, abilityHexTouched = 0;
    [HideInInspector]
    public Hexagon hexImOn;

    public enum State
    {
        inHand, selectedFromHand, selectedFromMap, placed
    }

    public enum Type
    {
        card1, card2, card3
    }

    [HideInInspector]
    public State state = State.inHand;
    public Type type;
    [HideInInspector]
    public PlayerController player;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public bool closeAnimFinished = false, openAnimFinished = false, rotateRightFlowFinished = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Open");
    }

    void Update()
    {
        if (state == State.selectedFromHand)
        {
            Vector3 myMousePos = Input.mousePosition;
            myMousePos.z = 28f;
            transform.position = Camera.main.ScreenToWorldPoint(myMousePos);
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.Rotate(Vector3.up * -60);
                eulerAngle -= 60;
                if (eulerAngle == 360 || eulerAngle == -360)
                    eulerAngle = 0;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.Rotate(Vector3.up * 60);
                eulerAngle += 60;
                if (eulerAngle == 360 || eulerAngle == -360)
                    eulerAngle = 0;
            }
        }

        if (state == State.selectedFromMap)
        {
            transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.Rotate(Vector3.up * -60);
                eulerAngle -= 60;
                if (eulerAngle == 360 || eulerAngle == -360)
                    eulerAngle = 0;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.Rotate(Vector3.up * 60);
                eulerAngle += 60;
                if (eulerAngle == 360 || eulerAngle == -360)
                    eulerAngle = 0;
            }
        }
    }

    public void Place(Hexagon hex)
    {
        if (state == State.selectedFromHand || state == State.selectedFromMap)
        {
            BlockPaths(hex);
            state = State.placed;
            placedEulerAngle = eulerAngle;
            hexImOn = hex;
            hexImOn.card = this;
            ResetTouchValues();
            SetTouchvalues(hexImOn);
            player.cardsInHand.Remove(this);
            GameManager.instance.cardsManager.PlacedCards.Add(this);

            //ANIMATION
            StartCoroutine(AnimateToDestination(hexImOn.worldPosition /*+ Vector3.up * .5f*/, 7f));
        }
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

        extractableEnergy = 0;
    }

    public IEnumerator RotateRight()
    {
        ResetAnimationFlags();
        FreePaths(hexImOn);
        animator.SetTrigger("Close");

        while (!closeAnimFinished)
            yield return null;
        
        yield return StartCoroutine(AnimateToRotation((transform.rotation.eulerAngles + Vector3.up * 60), 1f));

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

    void SetTouchvalues(Hexagon myHex)
    {
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (topLeft != null && !topLeft.card)
                            switch (topLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (topRight != null && !topRight.card)
                            switch (topRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (right != null && !right.card)
                            switch (right.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (botRight != null && !botRight.card)
                            switch (botRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (botLeft != null && !botLeft.card)
                            switch (botLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (left != null && !left.card)
                            switch (left.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (topLeft != null && !topLeft.card)
                            switch (topLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (botLeft != null && !botLeft.card)
                            switch (botLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (topRight != null && !topRight.card)
                            switch (topRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (left != null && !left.card)
                            switch (left.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (right != null && !right.card)
                            switch (right.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (topLeft != null && !topLeft.card)
                            switch (topLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (botRight != null && !botRight.card)
                            switch (botRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (topRight != null && !topRight.card)
                            switch (topRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (botLeft != null && !botLeft.card)
                            switch (botLeft.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (right != null && !right.card)
                            switch (right.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (left != null && !left.card)
                            switch (left.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
                                    break;
                            }
                        if (botRight != null && !botRight.card)
                            switch (botRight.type)
                            {
                                case Hexagon.Type.energy:
                                    extractableEnergy++;
                                    break;
                                case Hexagon.Type.ability:
                                    abilityHexTouched++;
                                    break;
                                case Hexagon.Type.move:
                                    moveHexTouched++;
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

    void ResetTouchValues()
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
            Destroy(this.gameObject);
        }  
    }

    #region animations

    IEnumerator AnimateToDestination(Vector3 Destination, float speed)
    {
        while (transform.position != Destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, Destination, speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator AnimateToRotation(Vector3 TargetEulerAngle, float durationInSeconds)
    {
        float counter = 0f;
        Quaternion startingRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(TargetEulerAngle);
        while (counter < durationInSeconds)
        {
            counter += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, counter / durationInSeconds);
           yield return null;
        }
    }

    #endregion

}