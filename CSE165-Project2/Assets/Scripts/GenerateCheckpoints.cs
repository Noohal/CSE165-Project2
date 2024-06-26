using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCheckpoints : MonoBehaviour
{
    public TextAsset coordinateFile;

    public List<Vector3> coordinates;
    public List<GameObject> checkpoints;

    public GameObject checkpointPrefab;

    public DroneMovement drone;

    // Start is called before the first frame update
    void Awake()
    {
        if (coordinateFile != null)
        {
            coordinates = ParseCoordinates.ParseFile(coordinateFile);
        }

        CreateCheckpoints();
    }

    void CreateCheckpoints()
    {
        foreach (var coord in coordinates)
        {
            GameObject go = Instantiate(checkpointPrefab, coord, Quaternion.identity);
            checkpoints.Add(go);
        }
    }
}
