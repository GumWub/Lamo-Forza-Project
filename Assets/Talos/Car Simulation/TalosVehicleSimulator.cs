using System.Collections;


//This is a part of Talos that exposes the public API
public static class TalosVehicleSimulator
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

    private static void HandleEngine()
    {
        if (_engineState == EngineStates.Running)
        {
            if (_engineRpm < _carStats.Engine.StallRpm)
            {
                Stall();
            }
        }
    }

    private static void StartUpEngine()
    {   
        EngineStartupRoutine();   
    }

    private static void Stall()
    {
        _engineRpm = 0;
        _drivetrainRpm = 0;
        _engineState = EngineStates.Stalled;
    }


    static IEnumerator EngineStartupRoutine()
    {
        if (_engineState == EngineStates.Running || (!Mathf.Approximately(_clutchTorque, 0) && !Mathf.Approximately(_totalGearRatio, 0)))
            yield return null;

        _engineState = EngineStates.Starting;
        while (_engineRpm < _carStats.Engine.IdleRpm)
        {
            yield return new WaitForFixedUpdate();
        }
        _engineState = EngineStates.Running;
    }

    private static void SendOutputTorqueToWheels()
    {

    }
}