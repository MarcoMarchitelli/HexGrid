using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridCreator : MonoBehaviour {

    public Transform emptyHexagonPrefab;
    public Transform energyHexagonPrefab;
    public Transform abilityHexagonPrefab;
    public Transform winHexagonPrefab;

    public Transform waypointPrefab;
    Vector2 gridSize = new Vector2(7f, 7f);

    public GameObject playerReference;

    [Range(0.1f, 1f)]
    public float waysWitdth = 0.5f;

    float hexWidth = 1.723f, hexHeight = 2.000f;

    public List<Hexagon> HexGrid = new List<Hexagon>();
    public List<Point> WaypointGrid = new List<Point>();

    Transform mapContainer;

    private void Awake()
    {
        CreateGrid();
        InstantiateWaypoints();
        InstantiateHexagons();
    }

    void AddWays()
    {
        hexWidth = 1.723f;
        hexHeight = 2.000f;
        hexWidth += waysWitdth;
        hexHeight += waysWitdth;
    }

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

                Vector3 hexPosition = new Vector3(-gridSize.x + x * hexWidth + offset, 0, -gridSize.y + y * hexHeight * .75f);
                Hexagon hex = new Hexagon(x, y, hexPosition, Hexagon.Type.empty);

                //da cambiare
                if(y == 0 && x <= 1 || y==0 && x == (int)gridSize.x -1 || y == 1 && x == 0 || y==1 && x == (int)gridSize.x - 1 || y == 2 && x == 0 ||
                    y == (int)gridSize.y - 1 && x <= 1 || y == (int)gridSize.y - 1 && x == (int)gridSize.x - 1 || y == (int)gridSize.y - 2 && x == 0 || 
                    y == (int)gridSize.y - 2 && x == (int)gridSize.x - 1 || y == (int)gridSize.y - 3 && x == 0)
                {
                    continue;
                }

                HexGrid.Add(hex);

                Vector3 lowVertexPosition = hexPosition + Vector3.forward * (-hexHeight / 2);
                Vector3 lowLeftVertexPosition = hexPosition + Vector3.forward * (-hexHeight * .25f) + Vector3.right * (-hexWidth / 2);
                Vector3 lowRightVertexPosition = hexPosition + Vector3.forward * (-hexHeight * .25f) + Vector3.right * (hexWidth / 2);
                Vector3 highRightVertexPosition = hexPosition + Vector3.forward * (hexHeight * .25f) + Vector3.right * (hexWidth / 2);
                Vector3 highLeftVertexPosition = hexPosition + Vector3.forward * (hexHeight * .25f) + Vector3.right * (-hexWidth / 2);
                Vector3 highVertexPosition = hexPosition + Vector3.forward * (hexHeight/2);
                
                int myX = x * 2;
                int myY = y * 2;

                Point lowVertex = new Point(myX + xOffset, myY, lowVertexPosition, Point.Movement.inverted);
                Point lowLeftVertex = new Point(myX - 1 + xOffset, myY + 1, lowLeftVertexPosition, Point.Movement.straight);
                Point lowRightVertex = new Point(myX + 1 + xOffset, myY + 1, lowRightVertexPosition, Point.Movement.straight);
                Point highRightVertex = new Point(myX + 1 + xOffset, myY + 2, highRightVertexPosition, Point.Movement.inverted);
                Point highLeftVertex = new Point(myX - 1 + xOffset, myY + 2, highLeftVertexPosition, Point.Movement.inverted);
                Point highVertex = new Point(myX + xOffset, myY + 3, highVertexPosition, Point.Movement.straight);

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
                    Transform instantiatedEmptyHex = Instantiate(emptyHexagonPrefab, hex.worldPosition, Quaternion.identity);
                    instantiatedEmptyHex.parent = mapContainer;
                    break;
                case Hexagon.Type.energy:
                    Transform instantiatedEnergyHex = Instantiate(energyHexagonPrefab, hex.worldPosition, Quaternion.identity);
                    instantiatedEnergyHex.parent = mapContainer;
                    break;
                case Hexagon.Type.ability:
                    Transform instantiatedAbilityHex = Instantiate(abilityHexagonPrefab, hex.worldPosition, Quaternion.identity);
                    instantiatedAbilityHex.parent = mapContainer;
                    break;
                case Hexagon.Type.win:
                    Transform instantiatedWinHex = Instantiate(winHexagonPrefab, hex.worldPosition, Quaternion.identity);
                    instantiatedWinHex.parent = mapContainer;
                    break;
            }
        }
    }

    void InstantiateWaypoints()
    {
        foreach (Point point in WaypointGrid)
        {
            Transform instantiatedWaypoint = Instantiate(waypointPrefab, point.worldPosition, Quaternion.identity);
            instantiatedWaypoint.parent = mapContainer;
        }
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

    public List<Point> GetPossibleDestinationsFromPoint(Point point)
    {
        List<Point> destinations = new List<Point>();

        for(int i = 0; i < WaypointGrid.Count; i++)
        {
            if(point.type == Point.Movement.straight)
            {
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y + 1)
                    destinations.Add(WaypointGrid[i]);
                if (WaypointGrid[i].x == point.x - 1 && WaypointGrid[i].y == point.y - 1)
                    destinations.Add(WaypointGrid[i]);
                if (WaypointGrid[i].x == point.x + 1 && WaypointGrid[i].y == point.y - 1)
                    destinations.Add(WaypointGrid[i]);
            }

            if (point.type == Point.Movement.inverted)
            {
                if (WaypointGrid[i].x == point.x && WaypointGrid[i].y == point.y - 1)
                    destinations.Add(WaypointGrid[i]);
                if (WaypointGrid[i].x == point.x - 1 && WaypointGrid[i].y == point.y + 1)
                    destinations.Add(WaypointGrid[i]);
                if (WaypointGrid[i].x == point.x + 1 && WaypointGrid[i].y == point.y + 1)
                    destinations.Add(WaypointGrid[i]);
            }
        }

        return destinations;
    }

}
