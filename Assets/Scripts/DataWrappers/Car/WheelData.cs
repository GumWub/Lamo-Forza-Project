using UnityEngine;

[System.Serializable]

public class WheelData
{
    public string WheelName;

    [Space(5f)]
    public float inertia;

    [Space(5f)]
    public Transform WheelTransform;

    [Space(5f)]
    public WheelCollider WheelCol;

    [Space(5f)]
    public BrakeData Brake;
}