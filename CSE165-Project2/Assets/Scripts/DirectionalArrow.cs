using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalArrow : MonoBehaviour
{
    public Transform tracking;
    public LineRenderer lineRenderer;
    public float offset;

    public Vector3 newpos;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.blue };
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = tracking.position + new Vector3(tracking.forward.x, 0.0f, tracking.forward.z).normalized * offset;
        newPosition.y -= 0.1f;
        transform.position = newPosition;
    }

    public void LookAtCheckpoint(Vector3 newPosition)
    {
        newpos = newPosition;
        Vector3 direction = newPosition - transform.position;
        transform.forward = direction;
        //lineRenderer.SetPosition(0, transform.position);
        //lineRenderer.SetPosition(1, transform.position + direction * 5f);
    }
}
