using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirRace : MonoBehaviour
{
    public GenerateCheckpoints checkpoints;
    public DroneMovement drone;
    public DirectionalArrow arrow;

    int currentCheckpoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentCheckpoint = 1;

        drone.SetPosition(checkpoints.coordinates[0]);
        drone.SetLastValidPosition(checkpoints.coordinates[0]);
    }

    // Update is called once per frame
    void Update()
    {
        arrow.LookAtCheckpoint(checkpoints.coordinates[currentCheckpoint]);
    }
}
