using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Player/Stat")]
public class PlayerStat : ScriptableObject
{
    [Header("Stat Values")]
    [SerializeField] private float maxValue = 100f;
    [SerializeField] private float currentValue = 100f;

    public float Max => maxValue;


    // Clamp current value between 0 and max
    public float Current
    {
        get => Mathf.Clamp(currentValue, 0f, maxValue);
        set => currentValue = Mathf.Clamp(value, 0f, maxValue);
    }

    public float Normalized => Mathf.Clamp01(Current / maxValue);

    // Static registry of all PlayerStat instances
    private static readonly List<PlayerStat> allStats = new List<PlayerStat>();

    private void OnEnable()
    {
        if (!allStats.Contains(this))
            allStats.Add(this);

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
        Current += amount;
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
