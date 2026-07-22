using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CarDB _carDatabase;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private EngineOilDb _engineOilDb;

    [Space(20f)]
    [SerializeField] private int _desiredCar;

    private GameObject _currentCarGameObject;
    private CarData _currentCarData;
    private CarMovement _carController;

    private EnvironmentData _currentEnvironment;

    private void Start()
    {
        SpawnNewCar(_desiredCar);//Temporary, would be set via another class later
        GetCarController();
        BindCarStatsToCarController();
        BindInputToCar();
        BindCameraToTarget();
        SetNewEnvironment(_currentEnvironment);
        SetNewEngineOil(_engineOilDb.EngineOil[0]);
    }

    public void SpawnNewCar(int index)
    {
        if (index < 0 || index >= _carDatabase.carData.Length) 
        {
            throw new System.IndexOutOfRangeException();
        }

        _currentCarData = _carDatabase.carData[index];
        _currentCarGameObject = GameObject.Instantiate(_currentCarData.carModelData.CarPrefab);
    }

    private void BindInputToCar()
    {
        if (_carController == null)
        {
            throw new System.NullReferenceException();
        }
        _inputManager.GetCarController(_carController);
    }

    private void BindCarStatsToCarController()
    {
        if (_carController == null)
        {
            throw new System.NullReferenceException();
        }

        _carController.GetCurrentCarStats(_currentCarData.carStats);
    }

    private void DespawnCurrentCar() {
        if (_currentCarGameObject == null)
        {
            throw new System.InvalidOperationException("Current car has not been spawned.");
        }
        GameObject.Destroy(_currentCarGameObject);
    }

    private void GetCarController()
    {
        if (_currentCarGameObject == null)
        {
            throw new System.InvalidOperationException("Current car has not been spawned.");
        }
        _carController = _currentCarGameObject.GetComponent<CarMovement>();
    }

    private void BindCameraToTarget()
    {
        _cameraManager.HandleNewTarget(_currentCarGameObject);
    }

    public void GetNewEnvironment(EnvironmentData newEnvironment)
    {
        _currentEnvironment = newEnvironment;
    }

    private void SetNewEnvironment(EnvironmentData newEnvironment)
    {
        _carController.GetNewEnvironment(newEnvironment);
    }

    private void SetNewEngineOil(EngineOilData newEngineOil)
    {
        _carController.GetNewEngineOil(newEngineOil);
    }
}