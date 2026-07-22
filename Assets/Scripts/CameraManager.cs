using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CinemachineBasicMultiChannelPerlin _noisePresets;

    [SerializeField] private NoiseSettings[] _noiseProfiles;

    private float _speed;
    public void HandleNewTarget(GameObject target)
    { 
        _target = target;
        _camera.Target.LookAtTarget = _camera.Target.TrackingTarget = _target.transform;
    }

    private void HandleShake(float speed)
    {
        if (speed > 150)
        {
            _noisePresets.NoiseProfile = _noiseProfiles[0];
            _noisePresets.AmplitudeGain = (speed - 150) / 100;
        }
        else
        {
            _noisePresets.NoiseProfile = null;
        }
    }

    public void GetData(float speed)
    {
        _speed = speed;

        HandleShake(speed);
    }
}