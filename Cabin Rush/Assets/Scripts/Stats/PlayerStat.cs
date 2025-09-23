using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("Stat Configuration")]
    [SerializeField] private string statName = "Health";
    [SerializeField] private float maxValue = 100f;
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float currentValue = 100f;

    public float CurrentValue => currentValue;
    public float MaxValue => maxValue;
    public float MinValue => minValue;
    public string StatName => statName;

    /// <summary>
    /// Modifies the stat by a given amount. Negative values reduce the stat.
    /// </summary>
    public void ModifyStat(float amount)
    {
        currentValue += amount;
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);
    }

    /// <summary>
    /// Resets the stat to its maximum value.
    /// </summary>
    public void ResetStat()
    {
        currentValue = maxValue;
    }

    /// <summary>
    /// Returns true if the stat has reached its minimum.
    /// </summary>
    public bool IsDepleted()
    {
        return currentValue <= minValue;
    }
}
