using System;
public static class Clutch
{
    //Let's keep it Stateless !
    internal static clutchData EngageClutch(Rpm carRpm, float clutchEngagement, float engineOutputTorque, float maxClutchTorque)
    {
        clutchData clutchData;

        //treat Clutch Engagement input;
        clutchEngagement =(float) TalosMath.Clamp01(clutchEngagement);

        //determine the sign of the torque
        float sign = Math.Sign(TalosMath.RpmToRadS(carRpm.engineRpm) - TalosMath.RpmToRadS(carRpm.drivetrainRpm));

        clutchData.clutchTorque =(sign * Math.Min(Math.Abs(engineOutputTorque), maxClutchTorque * clutchEngagement));
        clutchData.clutchState = SetClutchState(clutchEngagement);

        return clutchData;
    }

    private static ClutchStates SetClutchState(float clutchEngagement)
    {
        if (TalosMath.Approximately(clutchEngagement, 1))
        {
            return ClutchStates.Engaged;

        }

        if (TalosMath.Approximately(clutchEngagement, 0))
        {
            return ClutchStates.Disengaged;
        }

        return ClutchStates.Slipping;
    }
}

public struct clutchData
{
    public float clutchTorque;
    public ClutchStates clutchState;
}