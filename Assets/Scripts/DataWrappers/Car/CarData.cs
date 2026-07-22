using UnityEngine;

[System.Serializable]

public class CarData
{
    public string CarName;

    [Space(5f)]
    public CarStats carStats;

    [Space(5f)]
    public CarModelData carModelData;
}