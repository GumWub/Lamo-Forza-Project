using System.Collections;


//This is a part of Talos that exposes the public API
public static class Talos
{
    public static void Tick(float  physicsTick)
    {
        TalosTime.SetFixedDeltaTime(physicsTick);
    }

    //Car Modules
    public static float TreatThrottle(bool canTreatThrottle, float throttle, float engineRpm, float rpmCap, float idleRpm)
    {
        return ECU.TreatRequestedThrottle(canTreatThrottle, throttle, engineRpm, rpmCap, idleRpm);
    }

    public static clutchData TreatClutch(Rpm carRpm, float clutchEngagement, float engineOutputTorque, float maxClutchTorque)
    {
        return Clutch.EngageClutch(carRpm, clutchEngagement, engineOutputTorque, maxClutchTorque);
    }

    public static TransmissionData ShiftGear(TransmissionData transmissionData, int shiftDirection)
    {
        return Transmission.Shift(transmissionData, shiftDirection);
    }

    public static float ComputeDrivetrainOutputTorque(float engineOutputTorque, float clutchTorque, float totalGearRatio, bool isClutchEngaged)
    {
        return TalosPhysics.DriveTrainOutputTorque(engineOutputTorque, clutchTorque, totalGearRatio, isClutchEngaged);
    }

    public static float ComputeServiceBrakeTorque(float brakeEngagement, float maxBrakeTorque)
    {
        return brakeEngagement * maxBrakeTorque;
    }

    public static float ComputeParkingBrakeTorque(float handbrakeEngagement, float maxBrakeTorque)
    {
        return handbrakeEngagement * maxBrakeTorque;
    }

    public static float ComputeSteeringAngle(float currentSteeringAngle, float targetSteeringAngle, float steeringSpeed)
    {
        return TalosMath.Lerp(currentSteeringAngle, targetSteeringAngle, steeringSpeed * TalosTime.FixedDeltaTime);
    }

    public static Rpm ComputeRpm(RpmArguments data)
    {
        return RpmMaths.ComputeRPM(data);
    }

    public static float ComputeTotalInertia(bool isEngineAndDrivetrainUnlocked, float engineGroupInertia, float drivetrainGroupInertia)
    {
        return TalosPhysics.ComputeTotalInertia(isEngineAndDrivetrainUnlocked, engineGroupInertia, drivetrainGroupInertia);
    }

    public static float ComputeEngineSideInertia(float[] engineInertias)
    {
        return TalosPhysics.ComputeEngineGroupInertia(engineInertias);
    }

    public static float ComputeDrivetrainInertia(float[] drivetrainInertias)
    {
        return TalosPhysics.ComputeDrivetrainGroupInertia(drivetrainInertias);
    }
}