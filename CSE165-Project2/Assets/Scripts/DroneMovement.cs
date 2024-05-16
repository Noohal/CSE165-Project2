using Oculus.Interaction.Body.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public Rigidbody rb;
    public bool isColliding = false;

    private GameObject currentTarget;
    private Vector3 lastValidPosition = Vector3.zero;

    public delegate void CompleteCheckpoint();
    public event CompleteCheckpoint OnCompleteCheckpoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"ENTERED: {other.gameObject.tag}");
        if (other.gameObject.CompareTag("Checkpoint") && other.gameObject.Equals(currentTarget))
            OnCompleteCheckpoint?.Invoke();
        
        if (other.gameObject.CompareTag("Environment"))
        {
            Debug.Log("SENT TO THE GULAG");
            rb.position = lastValidPosition;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public void SetLastValidPosition(Vector3 position)
    {
        lastValidPosition = position;
    }

    public void SetTarget(GameObject target)
    {
        currentTarget = target;
    }

    public void Move(Vector3 direction, float speed)
    {
        Debug.Log($"Current Direction: {direction}, Current Speed: ${speed}");
        speed = Mathf.Clamp(speed, 0.1f, 35.0f);
        Vector3 movement = direction;
        movement = movement.normalized * speed * Time.deltaTime;
        //rb.velocity = movement;
        Vector3 newPosition = rb.position + movement;
        rb.MovePosition(newPosition);
        Debug.Log("Move!");
    }

    public void StopDrone()
    {
        //rb.velocity = Vector3.zero;
    }
}
