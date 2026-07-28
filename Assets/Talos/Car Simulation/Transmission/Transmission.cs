using System;

public static class Transmission
{
    //Let's keep it stateless!

    public static TransmissionData Shift( TransmissionData transmissionData, int shiftDirection)//ShiftDirection can either be 1 or -1, means it's either an upshift or a downshift
    {
        if (!transmissionData.CanShift||shiftDirection == 0)
            return transmissionData;

       transmissionData.Gear = Math.Clamp(transmissionData.Gear + shiftDirection, -1, transmissionData.TotalGears.Length);
       transmissionData = SetGear(transmissionData);

        return transmissionData;
    }

    private static TransmissionData SetGear(TransmissionData transmissionData)
    {
        if (transmissionData.Gear < -1 || transmissionData.Gear > transmissionData.TotalGears.Length)
            return transmissionData;
        

        if (transmissionData.Gear == -1)
        {
            transmissionData.TotalGearRatio = transmissionData.ReverseGear * transmissionData.FinalDrive;
            transmissionData.TransmissionState = TransmissionStates.InGear;
            return transmissionData;
        }
        if (transmissionData.Gear == 0)
        {
            transmissionData.TotalGearRatio = 0;
            transmissionData.TransmissionState = TransmissionStates.Neutral;
            return transmissionData;
        }

        transmissionData.TotalGearRatio = transmissionData.TotalGears[transmissionData.Gear - 1] * transmissionData.FinalDrive;
        transmissionData.TransmissionState = TransmissionStates.InGear;
        return transmissionData;
    }
}

public struct TransmissionData
{
    public bool CanShift;

    public int Gear;

    public float TotalGearRatio;

    public float ReverseGear;

    public float FinalDrive;

    public float[] TotalGears;

    public TransmissionStates TransmissionState;
}
