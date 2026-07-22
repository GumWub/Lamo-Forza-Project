using System;
public static class ECU
{
    //Let's Keep it Stateless !
    public static float TreatRequestedThrottle(bool canTreatThrottle, float throttle, float engineRpm, float rpmCap, float idleRpm)//canTreatThrottle is a flag that checks wether or not the engine can be treated (e.g. if the engine is Off, the ecu can't treat the throttle)
    {
        if (!canTreatThrottle)
        {
            throttle = 0;
            return throttle;
        }

        throttle = TalosMath.Clamp01(throttle);
        throttle = HandleRedline(throttle, engineRpm, rpmCap);
        throttle = HandleIdle(throttle, engineRpm, idleRpm);

        return throttle;
    }

    private static float HandleIdle(float throttle, float engineRpm, float idleRpm)
    {
        if (engineRpm < idleRpm)
        {
            throttle = 1f;//Horrible Ecu Approach, Temporary fix
        }

        return throttle;
    }

    private static float HandleRedline(float throttle, float engineRpm, float rpmCap)
    {
        if (engineRpm >= rpmCap)
        {
            throttle = 0;
        }

        return throttle;
    }
}
