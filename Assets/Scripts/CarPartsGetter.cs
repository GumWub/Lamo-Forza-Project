using UnityEngine;

public class CarPartsGetter : MonoBehaviour
{
    [SerializeField] private AxleData[] _axleData;
    [SerializeField] private LightData[] _lightData;


    [SerializeField] private Vector3 _wheelRotationOffset;

    private void Awake()
    {
        if (!CheckTorqueSplit())
        {
            Quit();
            throw new System.Exception("Incorrect torque split: Torque split total must be 1");
        }
        if (!CheckNumberOfWheelsPerAxle())
        {
            Quit();
            throw new System.Exception("Incorrect number of wheels per axle - max must be 2");
        }
    }

    private void Update()
    {
        foreach(var axle in _axleData)
        {
            foreach (var wheel in axle.Wheels)
            {
                wheel.WheelCol.GetWorldPose(out Vector3 pos, out Quaternion rot);


                Vector3 rotation = rot.eulerAngles;
                rotation += _wheelRotationOffset;

                wheel.WheelTransform.SetPositionAndRotation(pos,Quaternion.Euler(rotation));
            }
        }
    }

    private bool CheckTorqueSplit()
    {
        float sum = 0;

        foreach (var axle in _axleData)
        {
            sum += axle.TorqueAxleSplit;
        }

        if (Mathf.Abs(sum - 1f) > 0.001f)
        {
            return false;     
        }

        return true;
    }

    private bool CheckNumberOfWheelsPerAxle()
    {
        foreach(var axle in _axleData)
        {
            if(axle.Wheels.Length > 2 || axle.Wheels.Length <= 0)
            {
                return false;
            }
        }

        return true;
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    #region Public API
    public AxleData[] GetAxlesData() {
        return _axleData;
    }
#endregion
}
