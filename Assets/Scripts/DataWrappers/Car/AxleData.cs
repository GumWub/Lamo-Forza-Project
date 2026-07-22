using UnityEngine;

[System.Serializable]
public class AxleData
{
    public string AxleName;

    [Space(5f)]
    public bool IsDrivenAxle;
    public float TorqueAxleSplit;
    public float Inertia;

    [Space(5f)]
    public DriveShaftData CvAxleData;

    [Space(5f)]
    public WheelData[] Wheels = new WheelData[2];
}