using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    GameManager gameManager;
    int eulerAngle = 0;

    public enum State
    {
        inHand, selectedFromHand, selectedFromMap, placed
    }

    public enum Type
    {
        card1, card2, card3
    }

    public State state = State.inHand;
    public Type type;

    public bool isShield = true;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (state == State.selectedFromHand)
        {
            Vector3 myMousePos = Input.mousePosition;
            myMousePos.z = 19f;
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
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
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
                if (eulerAngle == 360 || eulerAngle == -360)
                    eulerAngle = 0;
            }
        }
    }

    public void Place(Hexagon hex)
    {
        if(state == State.selectedFromHand || state == State.selectedFromMap)
        {
            BlockPaths(hex);
            transform.position = hex.worldPosition + Vector3.up * .5f;
            state = State.placed;
        }
    }

    public void BlockPaths(Hexagon hex)
    {
        List<Point> pointsAroundCard = gameManager.gridReference.GetSixPointsAroundHexagon(hex);

        Point bottom = new Point();
        Point bottomRight = new Point();
        Point bottomLeft = new Point();
        Point topRight = new Point();
        Point topLeft = new Point();
        Point top = new Point();

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
                    bottomRight.possibleDestinations.Remove(topRight.worldPosition);
                    topRight.possibleDestinations.Remove(bottomRight.worldPosition);
                    break;
                case 60:
                case -300:
                    bottomRight.possibleDestinations.Remove(bottom.worldPosition);
                    bottom.possibleDestinations.Remove(bottomRight.worldPosition);
                    break;
                case 120:
                case -240:
                    bottomLeft.possibleDestinations.Remove(bottom.worldPosition);
                    bottom.possibleDestinations.Remove(bottomLeft.worldPosition);
                    break;
                case 180:
                case -180:
                    bottomLeft.possibleDestinations.Remove(topLeft.worldPosition);
                    topLeft.possibleDestinations.Remove(bottomLeft.worldPosition);
                    break;
                case 240:
                case -120:
                    topLeft.possibleDestinations.Remove(top.worldPosition);
                    top.possibleDestinations.Remove(topLeft.worldPosition);
                    break;
                case 300:
                case -60:
                    top.possibleDestinations.Remove(topRight.worldPosition);
                    topRight.possibleDestinations.Remove(top.worldPosition);
                    break;
            }
        }
        else 
        if (type == Type.card2)
        {
            switch (eulerAngle)
            {
                case 0:
                    bottomRight.possibleDestinations.Remove(topRight.worldPosition);
                    topRight.possibleDestinations.Remove(bottomRight.worldPosition);
                    topLeft.possibleDestinations.Remove(top.worldPosition);
                    top.possibleDestinations.Remove(topLeft.worldPosition);
                    break;
                case 60:
                case -300:
                    bottomRight.possibleDestinations.Remove(bottom.worldPosition);
                    bottom.possibleDestinations.Remove(bottomRight.worldPosition);
                    top.possibleDestinations.Remove(topRight.worldPosition);
                    topRight.possibleDestinations.Remove(top.worldPosition);
                    break;
                case 120:
                case -240:
                    bottomLeft.possibleDestinations.Remove(bottom.worldPosition);
                    bottom.possibleDestinations.Remove(bottomLeft.worldPosition);
                    bottomRight.possibleDestinations.Remove(topRight.worldPosition);
                    topRight.possibleDestinations.Remove(bottomRight.worldPosition);
                    break;
                case 180:
                case -180:
                    bottomLeft.possibleDestinations.Remove(topLeft.worldPosition);
                    topLeft.possibleDestinations.Remove(bottomLeft.worldPosition);
                    bottomRight.possibleDestinations.Remove(bottom.worldPosition);
                    bottom.possibleDestinations.Remove(bottomRight.worldPosition);
                    break;
                case 240:
                case -120:
                    topLeft.possibleDestinations.Remove(top.worldPosition);
                    top.possibleDestinations.Remove(topLeft.worldPosition);
                    bottomLeft.possibleDestinations.Remove(bottom.worldPosition);
                    bottom.possibleDestinations.Remove(bottomLeft.worldPosition);
                    break;
                case 300:
                case -60:
                    top.possibleDestinations.Remove(topRight.worldPosition);
                    topRight.possibleDestinations.Remove(top.worldPosition);
                    bottomLeft.possibleDestinations.Remove(topLeft.worldPosition);
                    topLeft.possibleDestinations.Remove(bottomLeft.worldPosition);
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
                    bottomRight.possibleDestinations.Remove(topRight.worldPosition);
                    topRight.possibleDestinations.Remove(bottomRight.worldPosition);
                    topLeft.possibleDestinations.Remove(top.worldPosition);
                    top.possibleDestinations.Remove(topLeft.worldPosition);
                    bottomLeft.possibleDestinations.Remove(bottom.worldPosition);
                    bottom.possibleDestinations.Remove(bottomLeft.worldPosition);
                    break;
                case 60:
                case -300:
                case 180:
                case -180:
                case 300:
                case -60:
                    bottomRight.possibleDestinations.Remove(bottom.worldPosition);
                    bottom.possibleDestinations.Remove(bottomRight.worldPosition);
                    top.possibleDestinations.Remove(topRight.worldPosition);
                    topRight.possibleDestinations.Remove(top.worldPosition);
                    bottomLeft.possibleDestinations.Remove(topLeft.worldPosition);
                    topLeft.possibleDestinations.Remove(bottomLeft.worldPosition);
                    break;
            }
        }
    }

    public void FreePaths(Hexagon hex)
    {
        List<Point> pointsAroundCard = gameManager.gridReference.GetSixPointsAroundHexagon(hex);

        foreach (Point point in pointsAroundCard)
        {
            point.possibleDestinations = gameManager.gridReference.GetPossibleDestinationsFromPoint(point);
        }
    }
}