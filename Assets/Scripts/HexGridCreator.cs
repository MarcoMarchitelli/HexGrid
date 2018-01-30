using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridCreator : MonoBehaviour {

    public Transform hexagonPrefab;
    public Transform waypointPrefab;
    public Vector2 gridSize;

    public GameObject playerReference;

    [Range(0.1f, 1f)]
    public float waysWitdth = 0.5f;

    float hexWidth = 1.723f, hexHeight = 2.000f;

    public List<Vector3> Waypoints = new List<Vector3>();

    private void Awake()
    {
        CreateGrid();
        InstantiatePlayer();
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

        Transform mapContainer = new GameObject("Map").transform;
        mapContainer.parent = transform;

        //adding ways
        AddWays();

        for (int y = 0; y < gridSize.y; y++)
        {
            float offset = 0;
            if (y % 2 != 0)
                offset = hexWidth / 2;
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 hexPosition = new Vector3(-gridSize.x + x * hexWidth + offset, 0, -gridSize.y + y * hexHeight * .75f);
                Transform instantiatedHex = Instantiate(hexagonPrefab, hexPosition, Quaternion.Euler(Vector3.up * 90));
                instantiatedHex.parent = mapContainer;

                Vector3 waypointPosition3 = hexPosition + Vector3.forward * (hexHeight/2);
                Vector3 waypointPosition4 = hexPosition + Vector3.forward * (hexHeight * .25f) + Vector3.right * (hexWidth/2);
                Vector3 waypointPosition5 = hexPosition + Vector3.forward * (-hexHeight * .25f) + Vector3.right * (hexWidth / 2);
                Vector3 waypointPosition6 = hexPosition + Vector3.forward * (-hexHeight / 2);
                Vector3 waypointPosition2 = hexPosition + Vector3.forward * (hexHeight * .25f) + Vector3.right * (-hexWidth / 2);
                Vector3 waypointPosition = hexPosition + Vector3.forward * (-hexHeight * .25f) + Vector3.right * (-hexWidth / 2);

                if (!DoesPointAlredyExist(waypointPosition))
                    Waypoints.Add(waypointPosition);
                if (!DoesPointAlredyExist(waypointPosition2))
                    Waypoints.Add(waypointPosition2);
                if (!DoesPointAlredyExist(waypointPosition3))
                    Waypoints.Add(waypointPosition3);
                if (!DoesPointAlredyExist(waypointPosition4))
                    Waypoints.Add(waypointPosition4);
                if (!DoesPointAlredyExist(waypointPosition5))
                    Waypoints.Add(waypointPosition5);
                if (!DoesPointAlredyExist(waypointPosition6))
                    Waypoints.Add(waypointPosition6);
            }
        }

        foreach(Vector3 wayPoint in Waypoints)
        {
            Transform instantiatedWaypoint = Instantiate(waypointPrefab, wayPoint, Quaternion.identity);
            instantiatedWaypoint.parent = mapContainer;
        }
    }

    void InstantiatePlayer()
    {
        GameObject instantiatedPlayer = Instantiate(playerReference, Waypoints[0], Quaternion.identity);
    }

    bool DoesPointAlredyExist(Vector3 point)
    {
        for (int i = 0; i < Waypoints.Count; i++)
        {
            if (Mathf.Approximately(point.x, Waypoints[i].x) && Mathf.Approximately(point.z, Waypoints[i].z))
                return true;
        }
        return false;
    }
}
