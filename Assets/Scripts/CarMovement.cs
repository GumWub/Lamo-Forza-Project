/*Script written by gumwub

Before you judge the questionnable quality of the code, the magic numbers and unused half assed methods, just bare in mind that i managed to write this 
Monolithic god class with methods that are the equivalent of the scars of quasimodo's face in a single week with little to no prior mechanical engineering knowledge/background, 
the class will be suparated into smaller classes and refactored properly once I'm done with the logic on my to do list


the software architecture scares me, i wake up in my sleep afraid that the unity gods will hunt me for the war crimes i commited in this class

Lord forgive me for the sins i commited and for the blood i have on my hand after butchering this class's software architecture 
(ego note: it's not a lack of software architecture knowledge, it's a lack of time)
*/
using TMPro;
using UnityEngine;

public class CarMovement : CarMovementBluePrint
{
    [SerializeField] private CarPartsGetter _carPartsGetter;
    [SerializeField] private Rigidbody _currentCarRb;
    [SerializeField] private float _wheelRotMaxSpeed = 1f;
    [SerializeField] private TMP_Text _currentSpeed;


    //------------DO NOT DELETE YET------------
    [SerializeField] private TMP_Text _EngineRpmTMP;
    private TMP_Text _DrivetrainRpmTMP;
    private TMP_Text _EngineStatus;
    private TMP_Text _EngineOutputTorque;
    private TMP_Text _currentGearTMP;
    private TMP_Text _batteryStatusTMP;

    private AxleData[] _axles;
    private CarStats _currentCarStats;
    //------------------------------------------



    /* [SerializeField]private EnvironmentData _currentEvironment; */
    /* private EngineOilData _currentEngineOil; */

    private void Start()
    {
        GetWheels();
        _currentSpeed = GameObject.FindGameObjectWithTag("speed").GetComponent<TMP_Text>();
        _EngineRpmTMP = GameObject.FindGameObjectWithTag("erpm").GetComponent<TMP_Text>();
        _DrivetrainRpmTMP = GameObject.FindGameObjectWithTag("drpm").GetComponent<TMP_Text>();
        _EngineStatus = GameObject.FindGameObjectWithTag("engines").GetComponent<TMP_Text>();
        _EngineOutputTorque = GameObject.FindGameObjectWithTag("engineo").GetComponent<TMP_Text>();
        _currentGearTMP = GameObject.FindGameObjectWithTag("gear").GetComponent<TMP_Text>();
        _batteryStatusTMP = GameObject.FindGameObjectWithTag("batterys").GetComponent<TMP_Text>();
    }

    private void Update()
    {
    }


    //Talos Handled separately

    public override void Accelerate(float throttle)
    {
        foreach(var axle in _axles)
        {
            if (axle.IsDrivenAxle)
            {
                foreach(var wheel in axle.Wheels)
                {

                }
            }
        }
    }

    public override void Startup()
    {
    }

    public override void Shutdown()
    {
    }

    public override void Brake(float Throttle)
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

    public override void SwitchGears(int newGear)
    {
    }

    public override void Clutch(float clutchEngagement)
    {
    }

    private void GetWheels()//Can be better
    {
        _axles = _carPartsGetter.GetAxlesData();

        foreach (var axle in _axles)
        {
            foreach (var wheel in axle.Wheels)
            {
                wheel.inertia = (float)(0.5 * wheel.WheelCol.mass * (wheel.WheelCol.radius * wheel.WheelCol.radius));
            }
        }
    }

    private void SetCarWeight()//can be better
    {
        _currentCarRb.mass = _currentCarStats.CarWheight;
    }

    #region Public API
    public void GetCurrentCarStats(CarStats newCarStats)//can be better
    {
        _currentCarStats = newCarStats;
        SetCarWeight();
    }

    public void GetNewEnvironment(EnvironmentData newEnvironment)//To fix
    {
        /*_currentEvironment = newEnvironment;*/
    }

    public void GetNewEngineOil(EngineOilData newEngineOil)//To fix
    {
        /*_currentEngineOil = newEngineOil;*/
    }
    #endregion
}