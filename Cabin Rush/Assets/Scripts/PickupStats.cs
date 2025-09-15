using UnityEngine;

[CreateAssetMenu]

public class PickupStats : ScriptableObject
{
    public GameObject model;
    [Range(1, 10)] public int speed;
    [Range(5, 1500)] public int Distance;
    [Range(0.1f, 3)] public float shootRate;
   

    public ParticleSystem hitEffect;
    public AudioClip[] PickupSounds;
    [Range(0, 1)] public float shootVol;
}

