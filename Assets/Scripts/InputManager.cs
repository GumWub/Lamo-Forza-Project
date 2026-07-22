using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InputManager : MonoBehaviour
{
    const int _right = 1, _left = -1;

    [SerializeField] private CarMovementBluePrint _movementBluePrint;
    [SerializeField] private TMP_Text HandBrake;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private Slider _clutchVal;

    private bool _isPause = false;
    private bool _handbrakeToggle = true;

    private Gears _currentGear;

    private void Start()
    {
        HandBrake.text = "On";
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        int throttle = 0, turn = 0, brake = 0, handbrake = 0, gearShift = 0;
        float clutch = _clutchVal.value;

        if (_currentGear == Gears.P)
        {
            handbrake += 1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            throttle += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            brake += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            turn += _left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            turn += _right;
        }

        if (Input.GetKey(KeyCode.K))
        {
            handbrake += 1;
        }
        if (Input.GetKeyDown(KeyCode.L)) 
        {
            _movementBluePrint.ToggleLights();
        }
        if (Input.GetKey(KeyCode.C))
        {
            clutch = 1;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            gearShift++;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            gearShift--;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _movementBluePrint.Startup();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            _movementBluePrint.Shutdown();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (_handbrakeToggle)
                _handbrakeToggle = false;

            else
                _handbrakeToggle = true;
        }

        if(_handbrakeToggle == true)
        {
            handbrake = 1;
        }

        if (handbrake >= 1)
        {
            HandBrake.text = "On";
        }
        else
        {
            HandBrake.text = "Off";
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_isPause)
            {
                Time.timeScale = 0f;
                PauseMenu.SetActive(true);
                _isPause = true;
            }
            else
            {
                Time.timeScale = 1f;
                PauseMenu.SetActive(false);
                _isPause = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        _movementBluePrint.Accelerate(throttle);
        _movementBluePrint.Brake(brake);
        _movementBluePrint.Turn(turn);
        _movementBluePrint.HandBrake(handbrake);
        _movementBluePrint.Clutch(clutch);
        _movementBluePrint.SwitchGears(gearShift);
    }

    public void GetCarController(CarMovementBluePrint carController)
    {
        _movementBluePrint = carController;
    }
}