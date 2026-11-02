using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class InteractPromptUI : MonoBehaviour
{
    [SerializeField] Camera viewCamera;
    [SerializeField] float reach = 5f;
    [SerializeField] float offsetBelowCrosshair = 36f;
    [SerializeField] int fontSize = 22;

    Text _label;
    InventoryPanelUI _inventoryPanel;

    void Awake()
    {
        ResolveViewCamera();
        _inventoryPanel = FindFirstObjectByType<InventoryPanelUI>();
        EnsureLabel();
    }

    void Update()
    {
        if (_label == null)
            return;

        if (!ShouldShowPrompt())
        {
            _label.enabled = false;
            return;
        }

        ResolveViewCamera();

        if (viewCamera == null || !TryGetTargetLabel(out string label))
        {
            _label.enabled = false;
            return;
        }

        _label.text = label;
        _label.enabled = true;
    }

    bool ShouldShowPrompt()
    {
        if (Cursor.lockState != CursorLockMode.Locked || Cursor.visible)
            return false;

        return _inventoryPanel == null || !_inventoryPanel.IsOpen;
    }

    bool TryGetTargetLabel(out string label)
    {
        label = null;
        if (!InteractionQueries.TryGetWoodTree(viewCamera, reach, out WoodTree tree))
            return false;

        label = tree.PromptLabel;
        return !string.IsNullOrEmpty(label);
    }

    void ResolveViewCamera()
    {
        if (viewCamera != null)
            return;

        viewCamera = Camera.main;
        if (viewCamera != null)
            return;

        viewCamera = FindFirstObjectByType<Camera>();
    }

    void EnsureLabel()
    {
        if (_label != null)
            return;

        var existing = transform.Find("InteractPrompt");
        if (existing != null)
        {
            _label = existing.GetComponent<Text>();
            if (_label != null)
                return;
        }

        var go = new GameObject("InteractPrompt", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(transform, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -offsetBelowCrosshair);
        rect.sizeDelta = new Vector2(240f, 32f);

        _label = go.GetComponent<Text>();
        _label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _label.fontSize = fontSize;
        _label.alignment = TextAnchor.UpperCenter;
        _label.color = Color.white;
        _label.raycastTarget = false;
        _label.horizontalOverflow = HorizontalWrapMode.Overflow;
        _label.verticalOverflow = VerticalWrapMode.Overflow;

        if (_label.GetComponent<Outline>() == null)
        {
            var outline = _label.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.85f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }

        _label.enabled = false;
    }
}
