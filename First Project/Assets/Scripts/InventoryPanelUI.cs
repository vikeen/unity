using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryPanelUI : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] FirstPersonController playerController;
    [SerializeField] GameObject panelRoot;
    [SerializeField] Text contentsLabel;

    public bool IsOpen { get; private set; }

    void Awake()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<PlayerInventory>();
        if (playerController == null)
            playerController = FindFirstObjectByType<FirstPersonController>();
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    void OnEnable()
    {
        if (inventory != null)
            inventory.WoodChanged += OnInventoryChanged;
    }

    void OnDisable()
    {
        if (inventory != null)
            inventory.WoodChanged -= OnInventoryChanged;
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.iKey.wasPressedThisFrame)
            Toggle();

        if (IsOpen && Keyboard.current.escapeKey.wasPressedThisFrame)
            Close();
    }

    public void Toggle()
    {
        if (IsOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        IsOpen = true;
        if (panelRoot != null)
            panelRoot.SetActive(true);
        Refresh();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        IsOpen = false;
        if (panelRoot != null)
            panelRoot.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnInventoryChanged(int _)
    {
        if (IsOpen)
            Refresh();
    }

    void Refresh()
    {
        if (contentsLabel == null || inventory == null)
            return;
        contentsLabel.text = $"Inventory\n\nWood  x{inventory.Wood}";
    }
}
