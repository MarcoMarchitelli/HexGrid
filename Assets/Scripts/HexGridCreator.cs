using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridCreator : MonoBehaviour
{

    //Hexagon prefabs
    public Transform emptyHexagonPrefab;
    public Transform energyHexagonPrefab;
    public Transform abilityHexagonPrefab;
    public Transform winHexagonPrefab;

    //Waypoint prefabs
    public Transform blueWaypointPrefab;
    public Transform yellowWaypointPrefab;
    public Transform redWaypointPrefab;
    public Transform greenWaypointPrefab;
    public Transform greyWaypointPrefab;
    public Transform purpleWaypointPrefab;

    Vector2 gridSize = new Vector2(7f, 7f);

    //public GameObject playerReference;

    [Range(0.1f, 3f)]
    public float waysWitdth = 1f;

    float hexWidth = 1.723f * 2, hexHeight = 2.000f * 2;

    public List<Hexagon> HexGrid = new List<Hexagon>();
    public List<Point> WaypointGrid = new List<Point>();

    Transform mapContainer;

    private void Awake()
    {
        CreateGrid();
        CreateSpecialWaypoints();
        SetDestinationsForEachPoint();
        SetNearHexagonsForEachPoint();
        InstantiateWaypoints();
        InstantiateHexagons();
    }

    //adding widht to hexagon
    void AddWays()
    {
        hexWidth = 1.723f;
        hexHeight = 2.000f;
        hexWidth += waysWitdth;
        hexHeight += waysWitdth;
    }


    //FUNCTIONS TO BUILD GRID AND DATA
    public void CreateGrid()
    {

        if (transform.Find("Map"))
            DestroyImmediate(transform.Find("Map").gameObject);

        mapContainer = new GameObject("Map").transform;
        mapContainer.parent = transform;

        //adding ways
        AddWays();

        for (int y = 0; y < (int)gridSize.y; y++)
        {
            float offset = 0;
            int xOffset = 0;

            if (y % 2 != 0)
            {
                offset = hexWidth / 2;
                xOffset = 1;
            }

            for (int x = 0; x < (int)gridSize.x; x++)
            {
                //create hex object at hexPosition with empty type as default
                Vector3 hexPosition = new Vector3(-gridSize.x + x * hexWidth + offset, 0, -gridSize.y + y * hexHeight * .75f);
                Hexagon hex = new Hexagon(x, y, hexPosition, Hexagon.Type.empty);

                //da cambiare
                if (y == 0 && x <= 1 || y == 0 && x == (int)gridSize.x - 1 || y == 1 && x == 0 || y == 1 && x == (int)gridSize.x - 1 || y == 2 && x == 0 ||
                    y == (int)gridSize.y - 1 && x <= 1 || y == (int)gridSize.y - 1 && x == (int)gridSize.x - 1 || y == (int)gridSize.y - 2 && x == 0 ||
                    y == (int)gridSize.y - 2 && x == (int)gridSize.x - 1 || y == (int)gridSize.y - 3 && x == 0)
                {
                    continue;
                }

                HexGrid.Add(hex);

                //creating 6 vertices around hex object, in world positions
                Vector3 lowVertexPosition = hexPosition + Vector3.forward * (-hexHeight / 2);
                Vector3 lowLeftVertexPosition = hexPosition + Vector3.forward * (-hexHeight * .25f) + Vector3.right * (-hexWidth / 2);
                Vector3 lowRightVertexPosition = hexPosition + Vector3.forward * (-hexHeight * .25f) + Vector3.right * (hexWidth / 2);
                Vector3 highRightVertexPosition = hexPosition + Vector3.forward * (hexHeight * .25f) + Vector3.right * (hexWidth / 2);
                Vector3 highLeftVertexPosition = hexPosition + Vector3.forward * (hexHeight * .25f) + Vector3.right * (-hexWidth / 2);
                Vector3 highVertexPosition = hexPosition + Vector3.forward * (hexHeight / 2);

                int myX = x * 2;
                int myY = y * 2;

                //creating 6 Point objects based on the 6 previous positions
                Point lowVertex = new Point(myX + xOffset, myY, lowVertexPosition);
                Point lowLeftVertex = new Point(myX - 1 + xOffset, myY + 1, lowLeftVertexPosition);
                Point lowRightVertex = new Point(myX + 1 + xOffset, myY + 1, lowRightVertexPosition);
                Point highRightVertex = new Point(myX + 1 + xOffset, myY + 2, highRightVertexPosition);
                Point highLeftVertex = new Point(myX - 1 + xOffset, myY + 2, highLeftVertexPosition);
                Point highVertex = new Point(myX + xOffset, myY + 3, highVertexPosition);

                //checking to be sure not to add duplicates
                if (!DoesPointAlredyExist(lowVertex))
                    WaypointGrid.Add(lowVertex);
                if (!DoesPointAlredyExist(lowLeftVertex))
                    WaypointGrid.Add(lowLeftVertex);
                if (!DoesPointAlredyExist(lowRightVertex))
                    WaypointGrid.Add(lowRightVertex);
                if (!DoesPointAlredyExist(highRightVertex))
                    WaypointGrid.Add(highRightVertex);
                if (!DoesPointAlredyExist(highLeftVertex))
                    WaypointGrid.Add(highLeftVertex);
                if (!DoesPointAlredyExist(highVertex))
                    WaypointGrid.Add(highVertex);

                //stupid way to remove waypoints
                if (y == 0 && x == 2 || y == 1 && x == 1 || y == 2 && x == 1 || y == 3 && x == 0)
                {
                    WaypointGrid.Remove(lowLeftVertex);
                }
                if (y == 0 && x == (int)gridSize.x - 2 || y == 1 && x == (int)gridSize.x - 2 || y == 2 && x == (int)gridSize.x - 1 || y == 3 && x == (int)gridSize.x - 1)
                {
                    WaypointGrid.Remove(lowRightVertex);
                }
                if (y == (int)gridSize.y - 1 && x == 2 || y == (int)gridSize.y - 2 && x == 1 || y == (int)gridSize.y - 3 && x == 1 || y == 3 && x == 0)
                {
                    WaypointGrid.Remove(highLeftVertex);
                }
                if (y == (int)gridSize.y - 1 && x == (int)gridSize.x - 2 || y == (int)gridSize.y - 2 && x == (int)gridSize.x - 2 || y == (int)gridSize.y - 3 && x == (int)gridSize.x - 1 || y == 3 && x == (int)gridSize.x - 1)
                {
                    WaypointGrid.Remove(highRightVertex);
                }
            }
        }
    }

    void InstantiateHexagons()
    {
        foreach (Hexagon hex in HexGrid)
        {
            hex.SetTypeFromCoords(gridSize);
        }

        foreach (Hexagon hex in HexGrid)
        {
            switch (hex.type)
            {
                case Hexagon.Type.empty:
                    Transform instantiatedEmptyHex = Instantiate(emptyHexagonPrefab, hex.worldPosition, Quaternion.Euler(Vector3.up * 90));
                    instantiatedEmptyHex.parent = mapContainer;
                    break;
                case Hexagon.Type.energy:
                    Transform instantiatedEnergyHex = Instantiate(energyHexagonPrefab, hex.worldPosition, Quaternion.Euler(Vector3.up * 90));
                    instantiatedEnergyHex.parent = mapContainer;
                    break;
                case Hexagon.Type.ability:
                    Transform instantiatedAbilityHex = Instantiate(abilityHexagonPrefab, hex.worldPosition, Quaternion.Euler(Vector3.up * 90));
                    instantiatedAbilityHex.parent = mapContainer;
                    break;
                case Hexagon.Type.win:
                    Transform instantiatedWinHex = Instantiate(winHexagonPrefab, hex.worldPosition, Quaternion.Euler(Vector3.up * 90));
                    instantiatedWinHex.parent = mapContainer;
                    break;
            }
        }
    }

    void InstantiateWaypoints()
    {
        foreach (Point point in WaypointGrid)
        {
            switch (point.type)
            {
                case Point.Type.blue:
                    Transform instantiatedBlueWaypoint = Instantiate(blueWaypointPrefab, point.worldPosition, Quaternion.identity);
                    instantiatedBlueWaypoint.parent = mapContainer;
                    break;
                case Point.Type.red:
                    Transform instantiatedRedWaypoint = Instantiate(redWaypointPrefab, point.worldPosition, Quaternion.identity);
                    instantiatedRedWaypoint.parent = mapContainer;
                    break;
                case Point.Type.yellow:
                    Transform instantiatedYellowWaypoint = Instantiate(yellowWaypointPrefab, point.worldPosition, Quaternion.identity);
                    instantiatedYellowWaypoint.parent = mapContainer;
                    break;
                case Point.Type.green:
                    Transform instantiatedGreenWaypoint = Instantiate(greenWaypointPrefab, point.worldPosition, Quaternion.identity);
                    instantiatedGreenWaypoint.parent = mapContainer;
                    break;
                case Point.Type.win:
                    Transform instantiatedWinWaypoint = Instantiate(purpleWaypointPrefab, point.worldPosition + Vector3.up * .5f, Quaternion.identity);
                    instantiatedWinWaypoint.parent = mapContainer;
                    break;
                case Point.Type.purple:
                    Transform instantiatedPurpleWaypoint = Instantiate(purpleWaypointPrefab, point.worldPosition, Quaternion.identity);
                    instantiatedPurpleWaypoint.parent = mapContainer;
                    break;
                case Point.Type.grey:
                    Transform instantiatedGreyWaypoint = Instantiate(greyWaypointPrefab, point.worldPosition, Quaternion.identity);
                    instantiatedGreyWaypoint.parent = mapContainer;
                    break;
            }
        }
    }

    void CreateSpecialWaypoints()
    {
        float blueStartingPointX = 0f, blueStartingPointY = 0f;
        float redStartingPointX = 0f, redStartingPointY = 0f;
        float greenStartingPointX = 0f, greenStartingPointY = 0f;
        float yellowStartingPointX = 0f, yellowStartingPointY = 0f;
        float startingPointX = 0f, startingPointY = 0f;

        float blueSpecialPointX = 0f, blueSpecialPointY = 0f;
        float redSpecialPointX = 0f, redSpecialPointY = 0f;
        float greenSpecialPointX = 0f, greenSpecialPointY = 0f;
        float yellowSpecialPointX = 0f, yellowSpecialPointY = 0f;

        foreach (Point point in WaypointGrid)
        {
            if (point.y == (int)MyData.startingYellowPoint.y)
            {
                yellowStartingPointY = point.worldPosition.z;
            }
            if (point.x == (int)MyData.startingYellowPoint.x)
            {
                yellowStartingPointX = point.worldPosition.x;
            }

            if (point.y == (int)MyData.startingBluePoint.y)
            {
                blueStartingPointY = point.worldPosition.z;
            }
            if (point.x == (int)MyData.startingBluePoint.x)
            {
                blueStartingPointX = point.worldPosition.x;
            }

            if (point.y == (int)MyData.startingRedPoint.y)
            {
                redStartingPointY = point.worldPosition.z;
            }
            if (point.x == (int)MyData.startingRedPoint.x)
            {
                redStartingPointX = point.worldPosition.x;
            }

            if (point.y == (int)MyData.startingGreenPoint.y)
            {
                greenStartingPointY = point.worldPosition.z;
            }
            if (point.x == (int)MyData.startingGreenPoint.x)
            {
                greenStartingPointX = point.worldPosition.x;
            }

            if (point.y == (int)MyData.specialYellowPoint.y)
            {
                yellowSpecialPointY = point.worldPosition.z;
            }
            if (point.x == (int)MyData.specialYellowPoint.x)
            {
                yellowSpecialPointX = point.worldPosition.x;
            }

            if (point.y == (int)MyData.specialBluePoint.y)
            {
                blueSpecialPointX = point.worldPosition.z;
            }
            if (point.x == (int)MyData.specialBluePoint.x)
            {
                blueSpecialPointX = point.worldPosition.x;
            }

            if (point.y == (int)MyData.specialRedPoint.y)
            {
                blueSpecialPointY = point.worldPosition.z;
            }
            if (point.x == (int)MyData.specialRedPoint.x)
            {
                blueSpecialPointX = point.worldPosition.x;
            }

            if (point.y == (int)MyData.specialGreenPoint.y)
            {
                greenSpecialPointX = point.worldPosition.z;
            }
            if (point.x == (int)MyData.specialGreenPoint.x)
            {
                greenSpecialPointY = point.worldPosition.x;
            }

        }

        Point yellowStartingPoint = new Point((int)MyData.startingYellowPoint.x, (int)MyData.startingYellowPoint.y, new Vector3(yellowStartingPointX, 0f, yellowStartingPointY), Point.Type.yellow, true);
        WaypointGrid.Add(yellowStartingPoint);

        Point blueStartingPoint = new Point((int)MyData.startingBluePoint.x, (int)MyData.startingBluePoint.y, new Vector3(blueStartingPointX, 0f, blueStartingPointY), Point.Type.blue, true);
        WaypointGrid.Add(blueStartingPoint);

        Point redStartingPoint = new Point((int)MyData.startingRedPoint.x, (int)MyData.startingRedPoint.y, new Vector3(redStartingPointX, 0f, redStartingPointY), Point.Type.red, true);
        WaypointGrid.Add(redStartingPoint);

        Point greenStartingPoint = new Point((int)MyData.startingGreenPoint.x, (int)MyData.startingGreenPoint.y, new Vector3(greenStartingPointX, 0f, greenStartingPointY), Point.Type.green, true);
        WaypointGrid.Add(greenStartingPoint);

        //win waypoint
        foreach (Hexagon hex in HexGrid)
        {
            if (hex.x == 3 && hex.y == 3)
            {
                startingPointX = hex.worldPosition.x;
                startingPointY = hex.worldPosition.z;
            }
        }
        Point winPoint = new Point(7, 7, new Vector3(startingPointX, 0f, startingPointY), Point.Type.win, false);
        WaypointGrid.Add(winPoint);
    }

    void SetDestinationsForEachPoint()
    {
        Point blue = new Point();
        Point yellow = new Point();
        Point red = new Point();
        Point green = new Point();
        Point win = new Point();

        foreach (Point point in WaypointGrid)
        {
            point.possibleDestinations = GetPossibleDestinationsFromPoint(point);
        }

        foreach (Point point in WaypointGrid)
        {
            if (point.type == Point.Type.yellow && point.isStartingPoint)
                yellow = point;
            if (point.type == Point.Type.red && point.isStartingPoint)
                red = point;
            if (point.type == Point.Type.blue && point.isStartingPoint)
                blue = point;
            if (point.type == Point.Type.green && point.isStartingPoint)
                green = point;
            if (point.type == Point.Type.win)
                win = point;
        }

        foreach (Point point in WaypointGrid)
        {
            if (yellow.possibleDestinations.Contains(point.worldPosition))
                point.possibleDestinations.Add(yellow.worldPosition);
            if (blue.possibleDestinations.Contains(point.worldPosition))
                point.possibleDestinations.Add(blue.worldPosition);
            if (green.possibleDestinations.Contains(point.worldPosition))
                point.possibleDestinations.Add(green.worldPosition);
            if (red.possibleDestinations.Contains(point.worldPosition))
                point.possibleDestinations.Add(red.worldPosition);
            if (win.possibleDestinations.Contains(point.worldPosition))
                point.possibleDestinations.Add(win.worldPosition);
        }
    }

    void SetNearHexagonsForEachPoint()
    {
        foreach (Point point in WaypointGrid)
        {
            if (point.type != Point.Type.win || !point.isStartingPoint)
            {
                point.nearHexagons = GetNearHexagonsFromPoint(point);
            }
        }
    }

    public List<Hexagon> GetNearHexagonsFromPoint(Point point)
    {
        List<Hexagon> nearHexagons = new List<Hexagon>();

        if (point.y % 2 != 0)
        {
            foreach (Hexagon hex in HexGrid)
            {
                //down
                if (hex.y == (point.y - 3) / 2 && hex.x == point.x / 2)
                    nearHexagons.Add(hex);
                //top right
                if (hex.y == (point.y - 1) / 2 && hex.x == (point.x + 1) / 2)
                    nearHexagons.Add(hex);
                //top left
                if (hex.y == (point.y - 1) / 2 && hex.x == (point.x - 1) / 2)
                    nearHexagons.Add(hex);
            }
        }

        if (point.y % 2 == 0)
        {
            foreach (Hexagon hex in HexGrid)
            {
                //top
                if (hex.y == point.y / 2 && hex.x == point.x / 2)
                    nearHexagons.Add(hex);
                //down right
                if (hex.y == (point.y - 1) / 2 && hex.x == (point.x + 1) / 2)
                    nearHexagons.Add(hex);
                //down left
                if (hex.y == (point.y - 1) / 2 && hex.x == (point.x - 1) / 2)
                    nearHexagons.Add(hex);
            }
        }

        return nearHexagons;
    }

    public List<Vector3> GetPossibleDestinationsFromPoint(Point point)
    {
        List<Vector3> destinations = new List<Vector3>();

        for (int i = 0; i < WaypointGrid.Count; i++)
        {
            //starting point yellow
            if (point.type == Point.Type.yellow && point.isStartingPoint)
            {
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y + 4)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x + 2 && WaypointGrid[i].y == point.y)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x + 1 && WaypointGrid[i].y == point.y + 2)
                    destinations.Add(WaypointGrid[i].worldPosition);
            }
            //starting point blue
            if (point.type == Point.Type.blue && point.isStartingPoint)
            {
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y + 4)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x - 2 && WaypointGrid[i].y == point.y)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x - 1 && WaypointGrid[i].y == point.y + 2)
                    destinations.Add(WaypointGrid[i].worldPosition);
            }
            //starting point red
            if (point.type == Point.Type.red && point.isStartingPoint)
            {
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y - 4)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x + 2 && WaypointGrid[i].y == point.y)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x + 1 && WaypointGrid[i].y == point.y - 2)
                    destinations.Add(WaypointGrid[i].worldPosition);
            }
            //starting point green
            if (point.type == Point.Type.green && point.isStartingPoint)
            {
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y - 4)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x - 2 && WaypointGrid[i].y == point.y)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x - 1 && WaypointGrid[i].y == point.y - 2)
                    destinations.Add(WaypointGrid[i].worldPosition);
            }

            //win point
            if (point.type == Point.Type.win)
            {
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y + 2)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y - 1)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x + 1 && WaypointGrid[i].y == point.y)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x - 1 && WaypointGrid[i].y == point.y)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x + 1 && WaypointGrid[i].y == point.y + 1)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x - 1 && WaypointGrid[i].y == point.y + 1)
                    destinations.Add(WaypointGrid[i].worldPosition);
            }

            //general straight movement point
            if (point.y % 2 != 0)
            {
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y + 1)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x - 1 && WaypointGrid[i].y == point.y - 1)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x + 1 && WaypointGrid[i].y == point.y - 1)
                    destinations.Add(WaypointGrid[i].worldPosition);
            }

            //general inverted movement point
            if (point.y % 2 == 0)
            {
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y - 1)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x - 1 && WaypointGrid[i].y == point.y + 1)
                    destinations.Add(WaypointGrid[i].worldPosition);
                if (WaypointGrid[i].x == point.x + 1 && WaypointGrid[i].y == point.y + 1)
                    destinations.Add(WaypointGrid[i].worldPosition);
            }
        }

        return destinations;
    }

    bool DoesPointAlredyExist(Point point)
    {
        for (int i = 0; i < WaypointGrid.Count; i++)
        {
            if (point.x == WaypointGrid[i].x && point.y == WaypointGrid[i].y)
                return true;
        }
        return false;
    }


    //FUNCTIONS TO RETREIVE DATA

    public Point GetPointFromWorldPosition(Vector3 worldPosition)
    {
        for (int i = 0; i < WaypointGrid.Count; i++)
        {
            if (Mathf.Approximately(worldPosition.x, WaypointGrid[i].worldPosition.x) && Mathf.Approximately(worldPosition.z, WaypointGrid[i].worldPosition.z))
            {
                return WaypointGrid[i];
            }
        }
        return null;
    }

    public Hexagon GetHexagonFromWorldPosition(Vector3 worldPosition)
    {
        for (int i = 0; i < HexGrid.Count; i++)
        {
            if (Mathf.Approximately(worldPosition.x, HexGrid[i].worldPosition.x) && Mathf.Approximately(worldPosition.z, HexGrid[i].worldPosition.z))
            {
                return HexGrid[i];
            }
        }
        return null;
    }

    public Point GetPointFromCoords(int x, int y)
    {
        foreach (Point point in WaypointGrid)
        {
            if (point.x == x && point.y == y)
            {
                return point;
            }
        }
        return null;
    }

    public Hexagon GetHexagonFromCoords(int x, int y)
    {
        foreach (Hexagon hex in HexGrid)
        {
            if (hex.x == x && hex.y == y)
            {
                return hex;
            }
        }
        return null;
    }

    public List<Point> GetSixPointsAroundHexagon(Hexagon hex)
    {
        List<Point> pointsAroundHex = new List<Point>();

        if (hex.y % 2 == 0)
        {
            foreach (Point point in WaypointGrid)
            {
                if (point.x == hex.x * 2 && point.y == hex.y * 2)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 && point.y == hex.y * 2 + 3)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 + 1 && point.y == hex.y * 2 + 1)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 - 1 && point.y == hex.y * 2 + 1)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 + 1 && point.y == hex.y * 2 + 2)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 - 1 && point.y == hex.y * 2 + 2)
                {
                    pointsAroundHex.Add(point);
                }
            }
        }
        else
        if (hex.y % 2 != 0)
        {
            foreach (Point point in WaypointGrid)
            {
                if (point.x == hex.x * 2 + 1 && point.y == hex.y * 2)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 + 1 && point.y == hex.y * 2 + 3)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 + 1 + 1 && point.y == hex.y * 2 + 1)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 - 1 + 1 && point.y == hex.y * 2 + 1)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 + 1 + 1 && point.y == hex.y * 2 + 2)
                {
                    pointsAroundHex.Add(point);
                }
                if (point.x == hex.x * 2 - 1 + 1 && point.y == hex.y * 2 + 2)
                {
                    pointsAroundHex.Add(point);
                }
            }
        }

        return pointsAroundHex;
    }

    public List<Hexagon> GetHexagonsAroundHexagon(Hexagon myHex)
    {
        List<Hexagon> aroundHexagons = new List<Hexagon>();

        if (myHex.y % 2 != 0)
        {
            foreach (Hexagon hex in HexGrid)
            {
                if (hex.y == myHex.y - 1 && hex.x == myHex.x)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y - 1 && hex.x == myHex.x + 1)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y && hex.x == myHex.x + 1)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y && hex.x == myHex.x - 1)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y + 1 && hex.x == myHex.x)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y + 1 && hex.x == myHex.x + 1)
                {
                    aroundHexagons.Add(hex);
                }
            }
        }
        else
            if (myHex.y % 2 == 0)
        {
            foreach (Hexagon hex in HexGrid)
            {
                if (hex.y == myHex.y - 1 && hex.x == myHex.x)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y - 1 && hex.x == myHex.x - 1)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y && hex.x == myHex.x + 1)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y && hex.x == myHex.x - 1)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y + 1 && hex.x == myHex.x)
                {
                    aroundHexagons.Add(hex);
                }
                else
                if (hex.y == myHex.y + 1 && hex.x == myHex.x - 1)
                {
                    aroundHexagons.Add(hex);
                }
            }
        }

        return aroundHexagons;
    }

}
