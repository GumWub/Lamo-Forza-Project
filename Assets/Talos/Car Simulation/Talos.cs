using UnityEngine;
using UnityEngine.Rendering;

public class Talos : MonoBehaviour
{
    //This is a dongle Between Unity and Talos -> Must be instantiated per vehicle

    //car instance fields
    private float _throttle;

    private Rpm _carRpm;

    private float _engineOutputTorque;
    private float _totalGearRatio;

    private float[] _frictionCoefficients;

    private clutchData _clutchData;
    TransmissionData transmissionData;
    private EngineStates _engineState;

    private CarStats _carStats;

    void FixedUpdate()
    {
        TalosTime.SetFixedDeltaTime(Time.fixedDeltaTime);//Plug Talos To unity's fixed Delta time
    }

    public void Accelerate(float throttle)
    {
        _throttle = TalosVehicleSimulator.TreatThrottle(CanTreatThrottle(_engineState), throttle, _carRpm.engineRpm, _carStats.Engine.RpmCap, _carStats.Engine.IdleRpm);
    }

    public void Clutch(float throttle)
    {
        _clutchData =  TalosVehicleSimulator.TreatClutch(throttle, engineOutputTorque, engineOmega, engineOmega, _carStats.Clutch.MaxClutchTorque);
    }

    public void ShiftGear(TransmissionData transmissionData, int shiftDirection)
    {
        TalosVehicleSimulator.ShiftGear(transmissionData, shiftDirection);
    }

    private bool CanTreatThrottle(EngineStates engineState)
    {
        if(engineState == EngineStates.Stalled)
            return false;

        return true;
    }

    private void ComputeEngineOutputTorque()
    {
        _engineOutputTorque = TalosPhysics.EngineOutputTorque(TalosPhysics.ComputeGeneratedEngineTorque(_carStats.Engine.TorqueCurve.Evaluate(_engineRpm), _throttle), TalosPhysics.ComputeFrictionTorque(_frictionCoefficients, 1, TalosMath.RpmToRadS(_engineRpm), _throttle));
    }
}
