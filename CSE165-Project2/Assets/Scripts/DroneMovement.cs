using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"ENTERED: {other.gameObject.tag}");
        if (other.gameObject.CompareTag("Environment"))
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public void Move(Vector3 direction, float speed)
    {
        Debug.Log($"Current Direction: {direction}");
        Vector3 movement = direction;
        movement = movement.normalized * speed * Time.deltaTime;

        Vector3 newPosition = rb.position + movement;
        rb.MovePosition(newPosition);
        Debug.Log("Move!");
    }
}
