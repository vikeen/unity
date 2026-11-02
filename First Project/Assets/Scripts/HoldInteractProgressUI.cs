using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HoldInteractProgressUI : MonoBehaviour
{
    [SerializeField] float ringSize = 44f;
    [SerializeField] Color trackColor = new Color(1f, 1f, 1f, 0.22f);
    [SerializeField] Color fillColor = new Color(0.35f, 0.95f, 0.45f, 0.95f);

    RectTransform _root;
    Image _fill;

    void Awake()
    {
        EnsureRing();
    }

    public void SetProgress(float normalized, bool visible)
    {
        if (_fill == null || _root == null)
            return;

        _root.gameObject.SetActive(visible);
        if (!visible)
        {
            _fill.fillAmount = 0f;
            return;
        }

        _fill.fillAmount = Mathf.Clamp01(normalized);
    }

    void EnsureRing()
    {
        if (_root != null)
            return;

        var existing = transform.Find("HoldProgress");
        if (existing != null)
        {
            _root = existing as RectTransform;
            _fill = existing.Find("Fill")?.GetComponent<Image>();
            if (_root != null && _fill != null)
            {
                _root.gameObject.SetActive(false);
                return;
            }
        }

        var rootGo = new GameObject(
            "HoldProgress",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));
        rootGo.transform.SetParent(transform, false);
        _root = rootGo.GetComponent<RectTransform>();
        _root.anchorMin = new Vector2(0.5f, 0.5f);
        _root.anchorMax = new Vector2(0.5f, 0.5f);
        _root.pivot = new Vector2(0.5f, 0.5f);
        _root.anchoredPosition = Vector2.zero;
        _root.sizeDelta = new Vector2(ringSize, ringSize);

        var track = rootGo.GetComponent<Image>();
        track.raycastTarget = false;
        track.sprite = WhiteSprite;
        track.color = trackColor;

        var fillGo = new GameObject(
            "Fill",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));
        fillGo.transform.SetParent(_root, false);

        var fillRect = fillGo.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        _fill = fillGo.GetComponent<Image>();
        _fill.raycastTarget = false;
        _fill.sprite = WhiteSprite;
        _fill.color = fillColor;
        _fill.type = Image.Type.Filled;
        _fill.fillMethod = Image.FillMethod.Radial360;
        _fill.fillOrigin = (int)Image.Origin360.Top;
        _fill.fillClockwise = true;
        _fill.fillAmount = 0f;

        _root.gameObject.SetActive(false);
    }

    static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite != null)
                return _whiteSprite;

            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _whiteSprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            return _whiteSprite;
        }
    }

    static Sprite _whiteSprite;
}