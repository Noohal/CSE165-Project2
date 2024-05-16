using Oculus.Interaction.Body.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public Rigidbody rb;
    public bool isColliding = false;

    private Vector3 lastValidPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"ENTERED: {other.gameObject.tag}");
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

    public void Move(Vector3 direction, float speed)
    {
        Debug.Log($"Current Direction: {direction}");
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
