using System;
public static class TalosMath
{
    //This class adds Unity namespace APIs that I found useful and extra APIs that I will need across Talos.
    //NaN is returned, it's your job to Handle the errors and guard against them

    public static float Clamp01(float value)
    { //I need Clamp01 quite a lot.
        if (float.IsNaN(value))//I can clamp infinity. No need to check it.
            return value;

        if (value <= 0)
            return 0;

        if (value >= 1)
            return 1;

        return value;
    }

    public static bool Approximately(float a, float b)
    {
        if (float.IsNaN(a) || float.IsInfinity(a))
            return false;
        
        if (float.IsNaN(b) || float.IsInfinity(b))
            return false;

        float precision = 0.001f;

        if (Math.Abs(a - b) < precision)
        {
            return true;
        }

        return false;
    }

    public static float Lerp(float a, float b, float t)
    {
        if(float.IsNaN(a)||float.IsInfinity(a))  
            return a;

        if (float.IsNaN(b)||float.IsInfinity(b))
            return b;

        if (float.IsNaN(t) || float.IsInfinity(t))
            return t;

        //I HATE THESE FUCKASS GUARDS.
        return a + (b - a) * t;
    }

    public static float RpmToRadS(float value)//I've followed the strtol/atoi naming structure.
    {//meh
        return (float)(value * ((2 * Math.PI) / 60));
    }

    public static float RadSToRpm(float value)
    {//meh^-1 (Lame math joke)
        return (float)(value * (60 / (2 * Math.PI)));
    }

    public static float DynamicFilter(float newValue, float previousValue, bool isHighBand)
    {//this is a band filter. It's a dynamic low band filter that can become a high band filter under some conditions.
        if (float.IsNaN(newValue) || float.IsInfinity(newValue))
            return previousValue;

        if (float.IsNaN(newValue) || float.IsInfinity(newValue))
            return previousValue;//Simply return the shit value to the sender

        float lerpFactor = 0.7f;
        float bandTolerance = 500;

        if (isHighBand)
        {
            bandTolerance = 4500;
            lerpFactor = 0.9f;
        }

        float fluctuation = (float)Math.Abs(previousValue - newValue);



        if (fluctuation >= bandTolerance)
        {
            int zeros = (int)Math.Floor((float)Math.Log10(fluctuation));//computes the number of digits after the digit with the highest weight in a number (e.g 600, zeroes = 2 (we skip the 6 and count the rest)/ 7600, zeroes = 3/ 54355, zeroes = 4)
            lerpFactor = (float) Math.Clamp(7 * Math.Pow(10, 1 - zeros), 0, 1);
        }

        newValue = Lerp(previousValue, newValue, lerpFactor);
        return newValue;
    }
}