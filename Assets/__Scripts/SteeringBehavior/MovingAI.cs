using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAI : MonoBehaviour
{
    [SerializeField]
    public List<SteeringBehaviour> steeringBehaviours;

    [SerializeField]
    private List<Detector> detectors;

    [SerializeField]
    public AIData aiData;

    [SerializeField]
    private float detectionDelay = 0.05f;


    [SerializeField]
    public ContextSolver movementDirectionSolver;


    private void Start()
    {
        InvokeRepeating("PerformDetection", 0, detectionDelay);
    }

    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
    }
}
