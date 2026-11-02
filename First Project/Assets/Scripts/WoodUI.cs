using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class WoodUI : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] Text woodLabel;
    [SerializeField] Image woodIcon;
    [SerializeField] Sprite woodIconSprite;
    [SerializeField] float iconSize = 32f;

    void Awake()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<PlayerInventory>();
        if (woodLabel == null)
            woodLabel = GetComponentInChildren<Text>(true);

        EnsureWoodHudRow();
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
            woodLabel.text = amount.ToString();
    }

    void EnsureWoodHudRow()
    {
        if (woodLabel == null || woodIconSprite == null)
            return;

        if (woodIcon != null)
        {
            woodIcon.sprite = woodIconSprite;
            return;
        }

        Transform hudRoot = woodLabel.transform.parent;
        if (hudRoot == null)
            return;

        var row = hudRoot.Find("WoodHudRow");
        if (row == null)
        {
            var rowGo = new GameObject(
                "WoodHudRow",
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup));
            rowGo.transform.SetParent(hudRoot, false);

            var rowRect = rowGo.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(0f, 1f);
            rowRect.pivot = new Vector2(0f, 1f);
            rowRect.anchoredPosition = new Vector2(24f, -24f);
            rowRect.sizeDelta = new Vector2(160f, iconSize);

            var layout = rowGo.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            woodLabel.transform.SetParent(row.transform, false);
            var labelRect = woodLabel.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(72f, iconSize);

            var iconGo = new GameObject(
                "WoodIcon",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            iconGo.transform.SetParent(row.transform, false);
            iconGo.transform.SetAsFirstSibling();

            var iconRect = iconGo.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(iconSize, iconSize);

            var layoutElement = iconGo.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = iconSize;
            layoutElement.preferredHeight = iconSize;

            woodIcon = iconGo.GetComponent<Image>();
            woodIcon.preserveAspect = true;
            woodIcon.raycastTarget = false;
        }
        else if (woodIcon == null)
        {
            woodIcon = row.GetComponentInChildren<Image>(true);
        }

        if (woodIcon != null)
            woodIcon.sprite = woodIconSprite;
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
