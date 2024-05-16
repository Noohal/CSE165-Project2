using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AirRace : MonoBehaviour
{
    public GenerateCheckpoints checkpoints;
    public DroneMovement drone;
    public DirectionalArrow arrow;

    public TextMeshProUGUI checkpointText;

    [SerializeField] int checkpointsCompleted = 0;

    bool finishedRace = false;

    private void Awake()
    {
        drone.OnCompleteCheckpoint += CompleteCheckpoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        checkpointsCompleted = 0;
        finishedRace = false;

        checkpointText.text = "Checkpoints: " + (checkpointsCompleted + 1) + "/" + checkpoints.coordinates.Count;
        drone.SetPosition(checkpoints.coordinates[checkpointsCompleted]);
        drone.SetLastValidPosition(checkpoints.coordinates[checkpointsCompleted]);
        drone.SetTarget(checkpoints.checkpoints[checkpointsCompleted + 1]);
    }

    // Update is called once per frame
    void Update()
    {
        if (checkpointsCompleted < checkpoints.coordinates.Count - 1) 
            arrow.LookAtCheckpoint(checkpoints.coordinates[checkpointsCompleted + 1]);
    }

    void CompleteCheckpoint()
    {
        if (finishedRace)
            return;
        checkpointsCompleted++;
        checkpointText.text = "Checkpoints: " + (checkpointsCompleted + 1) + "/" + checkpoints.coordinates.Count;
        if (checkpointsCompleted == checkpoints.checkpoints.Count - 1)
        {
            finishedRace = true;
            return;
        }
        drone.SetLastValidPosition(checkpoints.coordinates[checkpointsCompleted]);
        drone.SetTarget(checkpoints.checkpoints[checkpointsCompleted + 1]);

    }
}
