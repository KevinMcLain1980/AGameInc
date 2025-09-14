using UnityEngine;

[CreateAssetMenu]

public class SpeedPickup : ScriptableObject
{
    public GameObject model;

    [Range(1, 10)] public int SpeedAmount;
}