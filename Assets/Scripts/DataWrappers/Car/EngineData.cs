using UnityEngine;

[System.Serializable]
public class EngineData
{
    public FlyWheelData FlyWheel;

    public CrankShaftData CrankShaft;

    public AnimationCurve TorqueCurve;

    public StarterData Starter;

    public EngineFrictionData Friction;

    public float RpmCap;
    public float RedZoneRpmStart;
    public float IdleRpm;
    public float StallRpm;
}
