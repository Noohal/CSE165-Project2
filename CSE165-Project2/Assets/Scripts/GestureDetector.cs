using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;
using static OVRPlugin;

[System.Serializable]
public struct Gesture
{
    public string name;
    public int hand;
    public List<Vector3> posData;
    public List<Quaternion> rotData;
    public List<Vector3> scaData;
}

public class GestureDetector : MonoBehaviour
{
    public float threshold = 0.1f;
    public OVRSkeleton leftHandSkeleton;
    public OVRSkeleton rightHandSkeleton;
    public List<Gesture> gestures;

    public LineRenderer lineRenderer;

    private Gesture currentLeftGesture;
    private Gesture currentRightGesture;
    private Gesture previousLeftGesture;
    private Gesture previousRightGesture;
    private bool begin = false;
    private List<OVRBone> leftBones;
    private List<OVRBone> rightBones;

    public bool debugMode;

    // -- Drone Variables
    public Rigidbody rb;

    private List<Vector3> previousRightHandPos;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeBones());
        previousLeftGesture = new Gesture();
        currentLeftGesture = new Gesture();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };
    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SaveLeft();
            } 
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SaveRight();
            }
        }
        
        if (begin)
        {
            FindGesture();
            PerformAction();
        }

    }

    IEnumerator InitializeBones()
    {
        yield return new WaitUntil(() => (leftHandSkeleton.Bones.Count != 0) && (rightHandSkeleton.Bones.Count != 0));
        leftBones = new List<OVRBone>(leftHandSkeleton.Bones);
        rightBones = new List<OVRBone>(rightHandSkeleton.Bones);
        begin = true;
        Debug.Log("Left: " + leftBones.Count + " | Right: " + rightBones.Count);
    }

    void SaveLeft()
    {
        Gesture g = new Gesture();
        g.name = "New L Gesture";
        g.hand = 0;
        List<Vector3> posData = new List<Vector3>();
        List<Quaternion> rotData = new List<Quaternion>();
        List<Vector3> scaData = new List<Vector3>();

        foreach (var bone in leftBones)
        {
            posData.Add(leftHandSkeleton.transform.InverseTransformPoint(bone.Transform.position));
            rotData.Add(Quaternion.Inverse(bone.Transform.rotation));
            scaData.Add(leftHandSkeleton.transform.InverseTransformVector(bone.Transform.localScale));
            //data.Add(leftHandSkeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.posData = posData;
        g.rotData = rotData;
        g.scaData = scaData;
        gestures.Add(g);
    }

    void SaveRight()
    {
        Gesture g = new Gesture();
        g.name = "New R Gesture";
        g.hand = 1;
        List<Vector3> posData = new List<Vector3>();
        List<Quaternion> rotData = new List<Quaternion>();
        List<Vector3> scaData = new List<Vector3>();

        foreach (var bone in rightBones)
        {
            posData.Add(rightHandSkeleton.transform.InverseTransformPoint(bone.Transform.position));
            rotData.Add(Quaternion.Inverse(bone.Transform.rotation));
            scaData.Add(rightHandSkeleton.transform.InverseTransformVector(bone.Transform.localScale));
        }

        g.posData = posData;
        g.rotData = rotData;
        g.scaData = scaData;
        gestures.Add(g);
    }

    Gesture Recognize()
    {
        Gesture current = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach(var gesture in gestures)
        {
            float sumDistance = 0;
            bool discarded = false;
            if (gesture.hand == 0)
            {
                for (int i = 0; i < leftBones.Count; i++)
                {
                    Vector3 currentData = leftHandSkeleton.transform.InverseTransformPoint(leftBones[i].Transform.position);
                    float distance = Vector3.Distance(currentData, gesture.posData[i]);

                    if (distance > threshold)
                    {
                        discarded = true;
                        break;
                    }

                    sumDistance += distance;
                }

                if (!discarded && sumDistance < currentMin)
                {
                    currentMin = sumDistance;
                    current = gesture;
                }
            } else
            {
                for (int i = 0; i < rightBones.Count; i++)
                {
                    Vector3 currentData = rightHandSkeleton.transform.InverseTransformPoint(rightBones[i].Transform.position);
                    float distance = Vector3.Distance(currentData, gesture.posData[i]);

                    if (distance > threshold)
                    {
                        discarded = true;
                        break;
                    }

                    sumDistance += distance;
                }

                if (!discarded && sumDistance < currentMin)
                {
                    currentMin = sumDistance;
                    current = gesture;
                }
            }
        }

        return current;
    }

    void FindGesture()
    {
        previousLeftGesture = currentLeftGesture;
        previousRightGesture = currentRightGesture;
        Gesture current = Recognize();
        bool hasRecognized = !current.Equals(new Gesture());

        if (hasRecognized)
        {
            if (current.hand == 0)
            {
                if (!current.Equals(previousLeftGesture))
                {
                    Debug.Log("Left Hand Gesture Found : " + current.name);
                    currentLeftGesture = current;
                }
            } 
            else
            {
                if (!current.Equals(previousRightGesture))
                {
                    Debug.Log("Right Hand Gesture Found : " + current.name);
                    currentRightGesture = current;
                }
            }
        }
    }

    void PerformAction()
    {
        switch(currentLeftGesture.name)
        {
            case "LPoint":
                MoveDrone();
                break;
            default:
                break;
        }

        switch (currentRightGesture.name)
        {
            case "RFist":
                break;
            default:
                break;
        }
    }

    // --- Gesture Actions
    public void MoveDrone()
    {
        //    Vector3 indexTipPosition = leftBones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position;
        //    Vector3 indexTipDirection = leftBones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.forward;
        //    Vector3 direction = new Vector3(0f, indexTipDirection.y / 2f, indexTipDirection.z);
        //    Debug.Log($"Index Finger Direction {direction}");
        //    lineRenderer.SetPosition(0, indexTipPosition);
        //    lineRenderer.SetPosition(1, indexTipPosition + direction * 10f);
        //    Vector3 movement = new Vector3(direction.x, direction.y, direction.z);
        Vector3 movement = Vector3.one;
        movement = movement.normalized * 10f * Time.deltaTime;

        Vector3 newPosition = rb.position + movement;
        rb.MovePosition(newPosition);
        Debug.Log("Move!");
    }
}
