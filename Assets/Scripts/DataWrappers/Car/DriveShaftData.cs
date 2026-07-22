using UnityEngine;

[System.Serializable]

public class DriveShaftData
{
    [Tooltip("If this field is unchecked, the following variables will not be used")]
    public bool IsCvShaft;

    [Tooltip("If this field is checked, this wheel will rotate in the opposite direction of the rotation")]
    public bool MirrorRotation;

    public float MaxRotationAngle;
}
