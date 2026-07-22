using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private EnvironmentData _currentEnvironment;
    [SerializeField] private EnvironmentDB _environmentDatabase;
    [SerializeField] private GameManager _gameManager;

    private void Start()
    {
        _currentEnvironment = _environmentDatabase.Environmemts[0];
        InjectNewEnvironmentToGameManager(_currentEnvironment);
    }

    private void InjectNewEnvironmentToGameManager(EnvironmentData newEnvironment)
    {
        _gameManager.GetNewEnvironment(newEnvironment);
    }
}
