using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class WoodUI : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] Text woodLabel;

    void Awake()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<PlayerInventory>();
        if (woodLabel == null)
            woodLabel = GetComponentInChildren<Text>(true);

        EnsureReadableStyle();
        EnsureHudVisible();
    }

    void Start()
    {
        if (inventory != null)
            OnWoodChanged(inventory.Wood);
    }

    void OnEnable()
    {
        if (inventory != null)
        {
            inventory.WoodChanged += OnWoodChanged;
            OnWoodChanged(inventory.Wood);
        }
    }

    void OnDisable()
    {
        if (inventory != null)
            inventory.WoodChanged -= OnWoodChanged;
    }

    void OnWoodChanged(int amount)
    {
        if (woodLabel != null)
            woodLabel.text = $"Wood: {amount}";
    }

    void EnsureReadableStyle()
    {
    }

    public void EnsureHudVisible()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas != null)
            canvas.sortingOrder = 100;

        if (woodLabel == null)
            return;

        woodLabel.color = Color.white;
        if (woodLabel.GetComponent<Outline>() == null)
        {
            var outline = woodLabel.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }
    }
}
