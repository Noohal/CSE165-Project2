using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCheckpoints : MonoBehaviour
{
    public TextAsset coordinateFile;

    public List<Vector3> coordinates;

    public GameObject checkpointPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (coordinateFile != null)
        {
            coordinates = ParseCoordinates.ParseFile(coordinateFile);
        }

        CreateCheckpoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateCheckpoints()
    {
        foreach (var coord in coordinates)
        {
            Instantiate(checkpointPrefab, coord, Quaternion.identity);
        }
    }
}
