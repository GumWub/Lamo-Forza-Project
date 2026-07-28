using System;
using System.Collections;
using UnityEngine;

public class TalosVehicleSimulator
{
    //This is a dongle Between Unity and Talos -> Must be instantiated per vehicle/Stateful instance, unlike Talos
    //This Entire Goddamn Class is a mess, I'm using a Coroutine inside POCO class (smells like homeless doodoo)
    //Update: this class knows Way too much, like, inertia and torque musn't be here:: TODO: purge this class and strip it of physics notions, conditions, and useless private methods

    //Input Related Fields
    private float _throttle;
    private float _serviceBrakeEngagement;
    private float _parkingBrakeEngagement;
    private float _engineOutputTorque;
    private float _steeringSpeed = 1;
    private float _steeringDirection = 0;
    private float _engineInertia;
    private float _drivetrainInertia;
    private float[] _frictionCoefficients;

    //Car Modules Related Fields
    private TransmissionData _transmissionData = new TransmissionData();
    private ClutchDataStruct _clutchData = new ClutchDataStruct();
    private EngineStates _engineState;
    private Rpm _carRpm = new Rpm();

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
    public float engineOutputTorque => _engineOutputTorque;
    public float ClutchTorque => _clutchData.clutchTorque;
    public ClutchStates CS => _clutchData.clutchState;
    public TransmissionStates  TS => _transmissionData.TransmissionState;
    public float _steerAngle;

    //Methods
    public void Init()//Must be called before the first frame. (Awake)
    {
        InitInputs();
        InitFrictionCoefficients();
        InitInertia();
        InitTransmission();
        InitClutch();
        InitEngine();
        InitRpm();
    }

    public void OnTick(float physicsTick)//Must be Called Once Per Physics Tick. (FixedUpdate)
    {
        Talos.Tick(physicsTick);//Plug Talos To unity's fixed Delta time

        ComputeEngineOutputTorque();//used for RPM computation

        UpdateRpm();//Compute Rpm - needed for other operations.

        HandleEngine();//Engine State Stuff

        //This is necessary, we don't want the stupid dev to fuck up our work, we know better right? so WE (yes we) will handle sending our torque to the wheels.
        SendOutputTorqueToWheels();//constinuously send torque to wheels
    }

    public void Accelerate(float throttle)
    {
        _throttle = Talos.TreatThrottle(CanTreatThrottle(), throttle, _carRpm.engineRpm, _carStats.Engine.RpmCap, _carStats.Engine.IdleRpm);
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
        _steeringDirection = Math.Clamp(_steeringDirection, -1, 1);
    }

    public void StartUpEngine()
    {   
    }

    public void StopEngine()
    {
        Stall();//must shut down both the engine and the battery, Needs a better implementation.
    }

    public void ToggleLights()//I have been dragging the lights for too long, needs to be implemented ASAP.
    {
        //Implement it ASAP twin
    }

#region Init                    //I'm lowkey proud of this class, It still can be improved, some methods may seem unecessary but they can be still be used to implement new stuff, it's for the sake of modularity, scalability and readability.
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
    _transmissionData = new TransmissionData();
    _transmissionData.TotalGearRatio = 0;
    _transmissionData.CanShift = true;
    _transmissionData.Gear = 0;
    _transmissionData.ReverseGear = _carStats.Gearbox.ReverseGear;
    _transmissionData.FinalDrive = _carStats.Gearbox.FinalDrive;
    _transmissionData.TotalGears = _carStats.Gearbox.GearRatio;
    _transmissionData.TransmissionState = TransmissionStates.Neutral;
}

private void InitClutch()
{
    _clutchData.clutchTorque = 0;
    _clutchData.clutchState = ClutchStates.Disengaged;
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

private void InitInertia()//horrible stuff, needs a class for initialisation - preferably a static one
{
    //init engine Inertia
    float[] engineInertia = new float[4];

    engineInertia[0] = _carStats.Engine.FlyWheel.Inertia;
    engineInertia[1] = _carStats.Engine.CrankShaft.Inertia;
    engineInertia[2] = _carStats.Gearbox.EngineSideInertia;
    engineInertia[3] = _carStats.Clutch.Inertia;

    _engineInertia = Talos.ComputeEngineSideInertia(engineInertia);


    //init drivetrain Inertia
    float[] drivetrainInertia = new float[1];

    drivetrainInertia[0] = _carStats.Gearbox.DrivetrainSideInertia;

    _drivetrainInertia = Talos.ComputeDrivetrainInertia(drivetrainInertia);
}
#endregion

#region private Methods         //90% of this region will Go.

    private void UpdateRpm()
    {
        //Setup Rpm Arguments
        RpmArguments rpmArgs = new RpmArguments();
        rpmArgs.AxleData = SetAxleRpmData();
        rpmArgs.PreviousRpm = _carRpm;

        rpmArgs.NetTorque = TalosPhysics.ComputeNetTorque(TalosPhysics.ComputeMotorTorque(IsEngineStarting(), _carStats.Engine.Starter.Torque, _engineOutputTorque),TalosPhysics.ComputeLoadTorque(IsNeutral(), _clutchData.clutchTorque));
        rpmArgs.TotalInertia = Talos.ComputeTotalInertia(IsEngineAndDrivetrainUnlocked(), _engineInertia, _drivetrainInertia);
        
        rpmArgs.TotalGearRatio = _transmissionData.TotalGearRatio;
        rpmArgs.CanComputeRpm = CanComputeRpm();
        rpmArgs.IsEngineAndDrivetrainLocked = IsEngineAndDrivetrainLocked();

        //Compute Rpm
        _carRpm = Talos.ComputeRpm(rpmArgs);
    }

    private AxleRpmData[] SetAxleRpmData()
    {
        AxleRpmData[] axleRpmData = new AxleRpmData[_axles.Length];

        //my brain, my eyes, and my hands are screaming, i don't have neither auto suggestions nor autocomplete, i typed this entire bs manually

        for(int axle = 0; axle < _axles.Length; axle++)
        {
            axleRpmData[axle].WheelRpm = new float[_axles[axle].Wheels.Length];

            for (int wheel = 0; wheel < _axles[axle].Wheels.Length; wheel++)
            {
                axleRpmData[axle].WheelRpm[wheel] = _axles[axle].Wheels[wheel].WheelCol.rpm;
            }

            axleRpmData[axle].TorqueBias = _axles[axle].TorqueAxleSplit;
        }

        return axleRpmData;
    }

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

    private void ComputeEngineOutputTorque()
    {
        float netGeneratedTorque = TalosPhysics.ComputeGeneratedEngineTorque(_carStats.Engine.TorqueCurve.Evaluate(_carRpm.engineRpm), _throttle);
        float netFrictionTorque = TalosPhysics.ComputeFrictionTorque(_frictionCoefficients, 1/*Oil Viscosity, Will be replaced later*/, TalosMath.RpmToRadS(_carRpm.engineRpm), _throttle);

        _engineOutputTorque = TalosPhysics.EngineOutputTorque(netGeneratedTorque, netFrictionTorque);
    }

    private void SendOutputTorqueToWheels()//includes the handbrakes and brakes, there's a better way to do this, will think about it as soon as the game runs.
    {
        foreach(var axle in _axles)
        {
            foreach(var wheel in axle.Wheels)
            {
                if(axle.IsDrivenAxle)
                    wheel.WheelCol.motorTorque = Talos.ComputeDrivetrainOutputTorque(_engineOutputTorque, _clutchData.clutchTorque, _transmissionData.TotalGearRatio, IsClutchEngaged());
                if(wheel.Brake.IsServiceBrake)
                    wheel.WheelCol.brakeTorque = Talos.ComputeServiceBrakeTorque(_serviceBrakeEngagement, wheel.Brake.ServiceBrakeTorque);
                if(wheel.Brake.IsParkingBrake)
                    wheel.WheelCol.brakeTorque = Talos.ComputeParkingBrakeTorque(_parkingBrakeEngagement, wheel.Brake.ParkingBrakeTorque);
                if(axle.CvAxleData.IsCvShaft){
                    wheel.WheelCol.steerAngle = Talos.ComputeSteeringAngle(wheel.WheelCol.steerAngle, axle.CvAxleData.MaxSteerAngle * _steeringDirection, _steeringSpeed);
                    _steerAngle = wheel.WheelCol.steerAngle;}           
            }
        }
    }
#endregion

#region Coroutines//EW vibes, needs to go and be implemented inside Talos.
    public IEnumerator EngineStartupRoutine()
    {
        if (_engineState != EngineStates.Stalled || (_clutchData.clutchState != ClutchStates.Disengaged && _transmissionData.TransmissionState != TransmissionStates.Neutral))
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
    private bool CanTreatThrottle()
    {
        if(_engineState == EngineStates.Stalled)
            return false;

        return true;
    }

    private bool IsClutchEngaged()
    {
        if(_clutchData.clutchState == ClutchStates.Engaged)
            return true;

        return false;
    }

    private bool CanComputeRpm()
    {
        if(_engineState != EngineStates.Stalled)
            return true;

        return false;
    }

    private bool IsEngineAndDrivetrainLocked()
    {
        if(_clutchData.clutchState == ClutchStates.Engaged && _transmissionData.TransmissionState != TransmissionStates.Neutral)
            return true;

        return false;
    }

    private bool IsEngineAndDrivetrainUnlocked()
    {
        if(_clutchData.clutchState == ClutchStates.Disengaged || _transmissionData.TransmissionState == TransmissionStates.Neutral)
            return true;

        return false;
    }

    private bool IsEngineStarting()
    {
        if(_engineState == EngineStates.Starting)
            return true;

        return false;
    }

    private bool IsNeutral()
    {
        if(_transmissionData.TransmissionState == TransmissionStates.Neutral)
            return true;

        return false;
    }
#endregion
}

//Lolz monolithic class maxxing all over again