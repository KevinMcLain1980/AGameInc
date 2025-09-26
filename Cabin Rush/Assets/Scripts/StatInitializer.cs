using UnityEngine;

public class StatInitializer : MonoBehaviour
{
    [SerializeField] private PlayerStat health;
    [SerializeField] private PlayerStat stamina;
    [SerializeField] private PlayerStat oxygen;

    private void Awake()
    {
        health?.ResetStat();
        stamina?.ResetStat();
        oxygen?.ResetStat();
    }
}
