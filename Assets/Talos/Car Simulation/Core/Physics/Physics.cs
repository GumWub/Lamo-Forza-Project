using System;

public static class TalosPhysics
{
    internal static float ComputeTotalInertia(bool isClutchEngaged,float engineGroupInertia, float drivetrainGroupInertia)    
    {
        if (!isClutchEngaged)
        {
            return engineGroupInertia;
        }
        return engineGroupInertia + drivetrainGroupInertia;
    }

    internal static float ComputeEngineGroupInertia(float[] engineGroupInertiaElements)
    {
        float engineGroupInertia = 0;

        for (int i = 0; i < engineGroupInertiaElements.Length; i++)
        {
            engineGroupInertia += engineGroupInertiaElements[i];
        }

        return engineGroupInertia;//much better
    }

    internal static float ComputeDrivetrainInertia(float[] drivetrainInertiaElements)
    {
        float drivetrainInertia = 0;

        for (int i = 0; i < drivetrainInertiaElements.Length; i++)
        {
            drivetrainInertia += drivetrainInertiaElements[i];
        }

        return drivetrainInertia;
    }

    //To Do: Fix the stuff flagged below -> merge this class with the inertia class to create one monolithic physics class --> rewrite it in C and turn it into a library
    public static float ComputeNetTorque(float motorTorque, float loadTorque)
    {
        return (motorTorque - loadTorque);
    }

    public static float ComputeMotorTorque(bool isEngineStarting, float starterTorque, float engineOutputTorque)
    {
        float motorTorque = (isEngineStarting) ? starterTorque : engineOutputTorque;
        return motorTorque;
    }

    public static float ComputeLoadTorque(bool isNeutral, float clutchTorque)
    {
        float loadTorque = (isNeutral) ? 0 : clutchTorque;
        return loadTorque;
    }

    public static float EngineOutputTorque(float engineTorque, float frictionTorque)
    {
        return (engineTorque - frictionTorque);
    }

    public static float DriveTrainOutputTorque(float engineOutputTorque, float clutchTorque, float totalGearRatio, bool isClutchEngaged)// Can still be upgraded further **
    {
        float outputTorque = (!isClutchEngaged) ? clutchTorque : engineOutputTorque;

        return (outputTorque * totalGearRatio);
    }

    public static void SendTorqueToWheels(AxleData[] axles, bool isNeutral, float drivetrainOutputTorque)//Should be removed from this class -- GameEngine related ***
    {
        if (isNeutral)
        {
            return;
        }

        foreach (var axle in axles)
        {
            if (axle.IsDrivenAxle)
            {
                foreach (var wheel in axle.Wheels)
                {
                    wheel.WheelCol.motorTorque = drivetrainOutputTorque;
                }

            }
        }
    }
    public static float ComputeGeneratedEngineTorque(float maxEngineTorque, float throttle)
    {
        return maxEngineTorque * throttle;
    }

    public static float ComputeFrictionTorque(float[] frictionCoefficients, float oilViscosity, float engineOmega, float throttle)
    {
        float precision = 0.001f;

        if (Math.Abs(engineOmega) <= precision)
            return 0;

        float sign = Math.Sign(engineOmega);

        float frictionTorque = (1 - throttle) * (frictionCoefficients[0] + (frictionCoefficients[1] * (oilViscosity) * (Math.Abs(engineOmega))) + (frictionCoefficients[2] * (engineOmega * engineOmega)));

        return (sign * frictionTorque);
    }
}
