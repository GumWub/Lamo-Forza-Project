using UnityEngine;
using System.Collections;

public class TalosVehicleSimulator : MonoBehaviour
{
    //This is a dongle Between Unity and Talos -> Must be instantiated per vehicle

    //car instance fields
    private float _throttle;

    private Rpm _carRpm;

    private float _drivetrainOutputTorque;
    private float _engineOutputTorque;
    private float _totalGearRatio;

    private float[] _frictionCoefficients;

    private TransmissionStates _transmissionState;
    private clutchData _clutchData;
    private TransmissionData _transmissionData;
    private EngineStates _engineState;

    private CarStats _carStats;

    public void OnTick(float physicsTick)
    {
        Talos.Tick(physicsTick);//Plug Talos To unity's fixed Delta time
        SendOutputTorqueToWheels();
    }

    public void Accelerate(float throttle)
    {
        _throttle = Talos.TreatThrottle(CanTreatThrottle(_engineState), throttle, _carRpm.engineRpm, _carStats.Engine.RpmCap, _carStats.Engine.IdleRpm);
    }

    public void Clutch(float throttle)
    {
        _clutchData =  Talos.TreatClutch(_carRpm, throttle, _engineOutputTorque, _carStats.Clutch.MaxClutchTorque);
    }

    public void ShiftGear(TransmissionData transmissionData, int shiftDirection)
    {
        Talos.ShiftGear(_transmissionData, shiftDirection);
    }

    public void Brake(float throttle)
    {

    }

    public override void HandBrake(float input)
    {

    }

    public override void Turn(float direction)
    {

    }

    public override void ToggleLights()
    {

    }

#region Conditions
    private bool CanTreatThrottle(EngineStates engineState)
    {
        if(engineState == EngineStates.Stalled)
            return false;

        return true;
    }
#endregion

#region private Methods
    private void ComputeEngineOutputTorque()
    {
        _engineOutputTorque = TalosPhysics.EngineOutputTorque(TalosPhysics.ComputeGeneratedEngineTorque(_carStats.Engine.TorqueCurve.Evaluate(_carRpm.engineRpm), _throttle), TalosPhysics.ComputeFrictionTorque(_frictionCoefficients, 1, TalosMath.RpmToRadS(_carRpm.engineRpm), _throttle));
    }

    private void HandleEngine()
    {
        if (_engineState == EngineStates.Running)
        {
            if (_carRpm.engineRpm < _carStats.Engine.StallRpm)
            {
                Stall();
            }
        }
    }

    private void StartUpEngine()
    {   
        StartCoroutine(EngineStartupRoutine());   
    }

    private void Stall()
    {
        _carRpm.engineRpm = 0;
        _carRpm.drivetrainRpm = 0;
        _engineState = EngineStates.Stalled;
    }

    IEnumerator EngineStartupRoutine()
    {
        if (_engineState != EngineStates.Stalled || (_clutchData.clutchState != ClutchStates.Disengaged || _transmissionState != TransmissionStates.Neutral))
            yield return null;

        _engineState = EngineStates.Starting;
        while (_carRpm.engineRpm < _carStats.Engine.IdleRpm)
        {
            yield return new WaitForFixedUpdate();
        }
        _engineState = EngineStates.Running;
    }

    private static void SendOutputTorqueToWheels()
    {
        foreach(var axle in _axles)
        {
            if (axle.IsDrivenAxle)
            {
                foreach(var wheel in axle.Wheels)
                {
                    wheel.WheelCol.motorTorque = _drivetrainOutputTorque * _totalGearRatio;
                }
            }
        }
    }
    #endregion
}
