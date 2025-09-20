using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Player/Stat")]
public class PlayerStat : ScriptableObject
{
    [Header("Stat Values")]
    public float maxValue = 100f;
    public float currentValue = 100f;

    public float Normalized => Mathf.Clamp01(currentValue / maxValue);

    // Static registry of all PlayerStat instances
    private static readonly List<PlayerStat> allStats = new List<PlayerStat>();

    private void OnEnable()
    {
        if (currentValue <= 0)
            currentValue = maxValue;
    }

    private void OnDisable()
    {
        allStats.Remove(this);
    }

    /// <summary>
    /// Modify the current stat value by a given amount.
    /// </summary>
    public void Modify(float amount)
    {
        currentValue = Mathf.Clamp(currentValue + amount, 0, maxValue);
    }

    /// <summary>
    /// Reset this stat to its maximum value.
    /// </summary>
    public void ResetStat()
    {
        currentValue = maxValue;
    }

    /// <summary>
    /// Reset all registered PlayerStat instances to their maximum values.
    /// Call this at game start or game end.
    /// </summary>
    public static void ResetAllStats()
    {
        foreach (var stat in allStats)
        {
            stat.ResetStat();
        }
    }
}
