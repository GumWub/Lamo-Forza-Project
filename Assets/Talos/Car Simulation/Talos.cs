using System.Collections;


//This is a part of Talos that exposes the public API
public static class Talos
{
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

    public static void Tick(float  physicsTick)
    {
        TalosTime.SetFixedDeltaTime(physicsTick);
    }
}