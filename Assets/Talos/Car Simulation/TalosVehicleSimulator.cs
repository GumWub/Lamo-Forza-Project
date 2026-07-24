using UnityEngine;
using System.Collections;

public class TalosVehicleSimulator
{
    //This is a dongle Between Unity and Talos -> Must be instantiated per vehicle/Stateful instance, unlike Talos
    //This Entire Goddamn Class is a mess, I'm using a fuckass Coroutine inside POCO class (smells like homeless doodoo)


    //Input Related Fields
    private float _throttle;
    private float _serviceBrakeEngagement;
    private float _parkingBrakeEngagement;
    private float _engineOutputTorque;
    private float _steeringSpeed;
    private float _steeringDirection;
    private float[] _frictionCoefficients;

    //Car Modules Related Fields
    private TransmissionStates _transmissionState;
    private TransmissionData _transmissionData;
    private clutchData _clutchData;
    private EngineStates _engineState;
    private Rpm _carRpm;

    //Model Related Fields
    private CarStats _carStats;
    private AxleData[] _axles;

    //Constructor
    public TalosVehicleSimulator(CarStats carStats, AxleData[] axles)
    {
        _carStats = carStats;
        _axles = axles;
    }

    //Properties
    public TransmissionData TransmissionData => _transmissionData;
    public Rpm CarRpm => _carRpm;
    
    //Methods
    public void Init()//Must be called before the first frame. (Awake)
    {
        InitInputs();
        InitFrictionCoefficients();
        InitTransmission();
        InitClutch();
        InitEngine();
        InitRpm();
    }

    public void OnTick(float physicsTick)//Must be Called Once Per Physics Tick. (FixedUpdate)
    {
        Talos.Tick(physicsTick);//Plug Talos To unity's fixed Delta time
        HandleEngine();//Engine State Stuff

        //This is necessary, we don't want the stupid dev to fuck up our work, we know better right? so WE (yes we) will handle sending our torque to the wheels.
        SendOutputTorqueToWheels();//constinuously send torque to wheels
    }

    public void Accelerate(float throttle)
    {
        _throttle = Talos.TreatThrottle(CanTreatThrottle(_engineState), throttle, _carRpm.engineRpm, _carStats.Engine.RpmCap, _carStats.Engine.IdleRpm);
    }

    public void Clutch(float throttle)
    {
        _clutchData =  Talos.TreatClutch(_carRpm, throttle, _engineOutputTorque, _carStats.Clutch.MaxClutchTorque);
    }

    public void ShiftGear(int shiftDirection)
    {
        _transmissionData = Talos.ShiftGear(_transmissionData, shiftDirection);
    }

    public void Brake(float throttle)
    {
        _serviceBrakeEngagement = throttle;
    }

    public void HandBrake(float throttle)
    {
        _parkingBrakeEngagement = throttle;
    }

    public void Turn(float direction)
    {
        _steeringDirection = direction;
    }

    public void StartUpEngine()
    {   
        EngineStartupRoutine();//Both Startup and shutdown must be handled inside Talos.
    }

    public void StopEngine()
    {
        Stall();//must shut down both the engine and the battery, Needs a better implementation.
    }

    public void ToggleLights()//I have been dragging the lights for too long, needs to be implemented ASAP.
    {
        //Implement it ASAP twin
    }

#region Init//I'm lowkey proud of this class, It still can be improved, some methods may seem unecessary but they can be still be used to implement new stuff, it's for the sake of modularity, scalability and readability.
private void InitInputs()
{
    _throttle = 0;
    _serviceBrakeEngagement = 0;
    _parkingBrakeEngagement = 0;
    _engineOutputTorque = 0;
    _steeringSpeed = 10;
    _steeringDirection = 0;
}

private void InitFrictionCoefficients()
{
    _frictionCoefficients = new float[3];

    _frictionCoefficients[0] = _carStats.Engine.Friction.ConstantFrictionCoefficient;
    _frictionCoefficients[1] = _carStats.Engine.Friction.ViscousFrictionCoefficient;
    _frictionCoefficients[2] = _carStats.Engine.Friction.QuadraticCoefficient;

    //I'm aware that this is a bad implementation, but It's the only way to stop an idiot from Shitting more than 3 coefficients.
}

private void InitTransmission()//May the wind guide my hand (There's something missing but i can't prove it)
{
    _transmissionState = TransmissionStates.Neutral;

    _transmissionData = new TransmissionData();
    _transmissionData.TotalGearRatio = 0;
    _transmissionData.CanShift = true;
    _transmissionData.Gear = 0;
    _transmissionData.ReverseGear = _carStats.Gearbox.ReverseGear;
    _transmissionData.FinalDrive = _carStats.Gearbox.FinalDrive;
    _transmissionData.TotalGears = _carStats.Gearbox.GearRatio;
}

private void InitClutch()
{
    _clutchData.clutchTorque = 0;
    _clutchData.clutchState = ClutchStates.Engaged;
}

private void InitEngine()
{
    _engineState = EngineStates.Stalled;
}

private void InitRpm()
{
    _carRpm = new Rpm();
    _carRpm.engineRpm = 0;
    _carRpm.drivetrainRpm = 0;
}
#endregion

#region private Methods //90% of this class will Go.

    private void HandleEngine()//Horrible Class, Needs To go ASAP and be replaced by real Fucking Physics.
    {
        if (_engineState == EngineStates.Running)
        {
            if (_carRpm.engineRpm < _carStats.Engine.StallRpm)
            {
                Stall();
            }
        }
    }

    private void Stall()//Horrible Class Too, Needs To Go too
    {
        _carRpm.engineRpm = 0;
        _carRpm.drivetrainRpm = 0;
        _engineState = EngineStates.Stalled;
    }

    private void SendOutputTorqueToWheels()//includes the handbrakes and brakes, there's a better way to do this, will think about it as soon as the game runs.
    {
        foreach(var axle in _axles)
        {
            foreach(var wheel in axle.Wheels)
            {
                if(axle.IsDrivenAxle)
                    wheel.WheelCol.motorTorque = Talos.ComputeDrivetrainOutputTorque(TalosPhysics.EngineOutputTorque(TalosPhysics.ComputeGeneratedEngineTorque(_carStats.Engine.TorqueCurve.Evaluate(_carRpm.engineRpm), _throttle), TalosPhysics.ComputeFrictionTorque(_frictionCoefficients, 1, TalosMath.RpmToRadS(_carRpm.engineRpm), _throttle)), _clutchData.clutchTorque, _transmissionData.TotalGearRatio, IsClutchEngaged(_clutchData.clutchState));
                if(wheel.Brake.IsServiceBrake)
                    wheel.WheelCol.brakeTorque = Talos.ComputeServiceBrakeTorque(_serviceBrakeEngagement, wheel.Brake.ServiceBrakeTorque);
                if(wheel.Brake.IsParkingBrake)
                    wheel.WheelCol.brakeTorque = Talos.ComputeParkingBrakeTorque(_parkingBrakeEngagement, wheel.Brake.ParkingBrakeTorque);
                if(axle.CvAxleData.IsCvShaft)
                    wheel.WheelCol.steerAngle = Talos.ComputeSteeringAngle(wheel.WheelCol.steerAngle, axle.CvAxleData.MaxSteerAngle*_steeringDirection, _steeringSpeed);
            }
        }
    }
#endregion

#region Coroutines//EW vibes, needs to go and be implemented inside Talos.
    IEnumerator EngineStartupRoutine()
    {
        if (_engineState != EngineStates.Stalled || (_clutchData.clutchState != ClutchStates.Disengaged || _transmissionState != TransmissionStates.Neutral))
            yield break;

        _engineState = EngineStates.Starting;
        while (_carRpm.engineRpm < _carStats.Engine.IdleRpm)
        {
            yield return new WaitForFixedUpdate();
        }
        _engineState = EngineStates.Running;
    }
#endregion

#region Conditions//Will make a static class for this.
    private bool CanTreatThrottle(EngineStates engineState)
    {
        if(engineState == EngineStates.Stalled)
            return false;

        return true;
    }

    private bool IsClutchEngaged(ClutchStates clutchState)
    {
        if(clutchState == ClutchStates.Engaged)
            return true;

        return false;
    }
#endregion
}