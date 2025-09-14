using NUnit.Framework.Interfaces;
using UnityEngine;

public interface IPickup
{
    public void getProperties(PickupStats Stats);

    public void getSpeedProp(SpeedPickup Speed);

    public void getHealthProp(HealthPickup Health);
}
