using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerData;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour
{
    public float threshold = 0.1f;
    public OVRSkeleton skeleton;
    public List<Gesture> gestures;

    private Gesture prevGesture;
    private bool begin = false;
    private List<OVRBone> fingerBones;

    public bool debugMode;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeBones());
        prevGesture = new Gesture();
    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            Save();
        }
        
        if (begin)
        {
            FindGesture();
        }
    }

    IEnumerator InitializeBones()
    {
        yield return new WaitUntil(() => skeleton.Bones.Count != 0);
        fingerBones = new List<OVRBone>(skeleton.Bones);
        begin = true;
        Debug.Log("BONE COUNT: " + fingerBones.Count);
    }

    void Save()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();

        foreach (var bone in fingerBones)
        {
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerData = data;
        gestures.Add(g);
    }

    Gesture Recognize()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach(var gesture in gestures)
        {
            float sumDistance = 0;
            bool discarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);

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
                currentGesture = gesture;
            }
        }

        return currentGesture;
    }

    void FindGesture()
    {
        Gesture currentGesture = Recognize();
        bool hasRecognized = !currentGesture.Equals(new Gesture());

        if (hasRecognized && !currentGesture.Equals(prevGesture))
        {
            Debug.Log("New Gesture Found : " + currentGesture.name);
            prevGesture = currentGesture;
        }
    }
}
