using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] int wood;

    public int Wood => wood;

    public event Action<int> WoodChanged;

    public void AddWood(int amount)
    {
        if (amount <= 0)
            return;
        wood += amount;
        WoodChanged?.Invoke(wood);
    }
}
