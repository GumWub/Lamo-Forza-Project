using System;

public static class RpmMaths
{
    //Let's keep it stateless
    private static Rpm ComputeRPM(RpmArguments data)
    {

        if (!data.CanComputeRpm)//if the engine is not stalled and other conditions
        {
            return data.PreviousRpm;
        }

        data.PreviousRpm.drivetrainRpm = ComputeDrivetrainRPM(data.AxleData, data.PreviousRpm.drivetrainRpm, data.TotalGearRatio, data.IsHighBandFlag);

        if (data.IsEngineAndDrivetrainLocked)// if the clutch is engaged and the transmission is in gear
        {
            data.PreviousRpm.engineRpm = data.PreviousRpm.drivetrainRpm;
            return data.PreviousRpm;
        }

        data.PreviousRpm.engineRpm += ComputeRPMDelta(data.NetTorque, data.TotalInertia);
        return data.PreviousRpm;
    }

    public static float ComputeRPMDelta(float netTorque, float totalInertia)
    {
        float angularAcceleration = (netTorque / totalInertia);

        return TalosMath.RadSToRpm(angularAcceleration * TalosTime.FixedDeltaTime);
    }

    public static float ComputeDrivetrainRPM(AxleRpmData[] axleRpm, float previousDrivetrainRpm,float totalGearRatio, bool highBandFlag)//there's a better solution but i can't figure it out yet
    {
        float drivetrainRPM = 0;
        foreach (var axle in axleRpm)
        {
            float axleRPM = 0;
            foreach(var wheelRpm in axle.WheelRpm)
            {
                axleRPM += wheelRpm;
            }

            axleRPM /= axle.WheelRpm.Length;
            drivetrainRPM += axleRPM * axle.TorqueBias;
        }
        drivetrainRPM *= totalGearRatio;
        return TalosMath.DynamicFilter(drivetrainRPM , previousDrivetrainRpm, highBandFlag);
    }
}

public struct AxleRpmData
{
    public float[] WheelRpm;// the size of the array will mostly either be 1 or 2. Uses WheelCollider's RPM
    public float TorqueBias;
}

public struct Rpm
{
    public float engineRpm;
    public float drivetrainRpm;
}

public struct RpmArguments
{
    //StateData
    public AxleRpmData[] AxleData;
    public Rpm PreviousRpm;

    //physics Values
    public float NetTorque;
    public float TotalInertia;

    //Gear Ratio
    public float TotalGearRatio;

    //flags
    public bool CanComputeRpm;
    public bool IsEngineAndDrivetrainLocked;
    public bool IsHighBandFlag;
}