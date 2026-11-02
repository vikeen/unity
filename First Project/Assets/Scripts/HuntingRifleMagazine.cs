using System;
using UnityEngine;

/// <summary>Magazine state for the equipped hunting rifle.</summary>
public class HuntingRifleMagazine : MonoBehaviour
{
    public const int Capacity = 10;

    [SerializeField] int loadedRounds = 4;

    public int LoadedRounds => loadedRounds;

    public event Action<int> LoadedRoundsChanged;

    void OnValidate()
    {
        loadedRounds = Mathf.Clamp(loadedRounds, 0, Capacity);
    }

    public bool TryConsumeRound()
    {
        if (loadedRounds <= 0)
            return false;

        loadedRounds--;
        LoadedRoundsChanged?.Invoke(loadedRounds);
        return true;
    }

    public void SetLoadedRounds(int count)
    {
        int clamped = Mathf.Clamp(count, 0, Capacity);
        if (clamped == loadedRounds)
            return;

        loadedRounds = clamped;
        LoadedRoundsChanged?.Invoke(loadedRounds);
    }
}
