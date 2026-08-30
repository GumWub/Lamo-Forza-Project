using System;
public static class Clutch
{
    //Let's keep it Stateless !
    public static ClutchDataStruct EngageClutch(Rpm carRpm, float clutchEngagement, float engineOutputTorque, float maxClutchTorque)
    {
        ClutchDataStruct clutchData = new ClutchDataStruct();

        //treat Clutch Engagement input;
        clutchEngagement =(float) TalosMath.Clamp01(clutchEngagement);

        //determine the sign of the torque
        float sign = Math.Sign(carRpm.engineRpm - carRpm.drivetrainRpm);

        clutchData.clutchTorque =(sign * Math.Min(Math.Abs(engineOutputTorque), maxClutchTorque * clutchEngagement));//horrible clutch approach, needs to be modeled better.
        clutchData.clutchState = SetClutchState(clutchEngagement);

        return clutchData;
    }

    private static ClutchStates SetClutchState(float clutchEngagement)
    {
        if (TalosMath.Approximately(clutchEngagement, 1))
            return ClutchStates.Engaged;

        if (TalosMath.Approximately(clutchEngagement, 0))
            return ClutchStates.Disengaged;

        return ClutchStates.Slipping;
    }
}

public struct ClutchDataStruct
{
    public float clutchTorque;
    public ClutchStates clutchState;
}