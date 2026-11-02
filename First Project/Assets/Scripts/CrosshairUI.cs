using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class CrosshairUI : MonoBehaviour
{
    [SerializeField] float dotSize = 6f;
    [SerializeField] Color dotColor = new Color(1f, 1f, 1f, 0.9f);

    Image _dot;

    void Awake()
    {
        EnsureDot();
    }

    void Update()
    {
        if (_dot == null)
            return;

        _dot.enabled = Cursor.lockState == CursorLockMode.Locked && !Cursor.visible;
    }

    void EnsureDot()
    {
        if (_dot != null)
            return;

        var existing = transform.Find("CrosshairDot");
        if (existing != null)
        {
            _dot = existing.GetComponent<Image>();
            if (_dot != null)
                return;
        }

        var go = new GameObject("CrosshairDot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(transform, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(dotSize, dotSize);

        _dot = go.GetComponent<Image>();
        _dot.raycastTarget = false;
        _dot.color = dotColor;
        _dot.sprite = CreateWhiteSprite();
    }

    static Sprite CreateWhiteSprite()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
    }
}
