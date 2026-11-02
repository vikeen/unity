using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class RifleAmmoHUD : MonoBehaviour
{
    [SerializeField] FirstPersonWeaponEquip weaponEquip;
    [SerializeField] HuntingRifleMagazine magazine;
    [SerializeField] float bulletWidth = 8f;
    [SerializeField] float bulletHeight = 18f;
    [SerializeField] float spacing = 4f;
    [SerializeField] Vector2 margin = new(24f, 24f);
    [SerializeField] Color loadedBulletColor = new(0.92f, 0.82f, 0.38f, 1f);
    [SerializeField] Color emptyBulletColor = new(0.08f, 0.08f, 0.08f, 1f);

    Image[] _slots;
    RectTransform _root;
    static Sprite _bulletSprite;

    void Awake()
    {
        if (weaponEquip == null)
            weaponEquip = FindFirstObjectByType<FirstPersonWeaponEquip>();
        if (magazine == null)
            magazine = FindFirstObjectByType<HuntingRifleMagazine>();

        EnsureBulletRow();
    }

    void OnEnable()
    {
        if (magazine != null)
        {
            magazine.LoadedRoundsChanged += OnLoadedChanged;
            OnLoadedChanged(magazine.LoadedRounds);
        }

        RefreshVisibility();
    }

    void OnDisable()
    {
        if (magazine != null)
            magazine.LoadedRoundsChanged -= OnLoadedChanged;
    }

    void Update()
    {
        RefreshVisibility();
    }

    void OnLoadedChanged(int loaded)
    {
        if (_slots == null)
            return;

        int emptySlots = HuntingRifleMagazine.Capacity - loaded;
        for (int i = 0; i < _slots.Length; i++)
        {
            bool filled = i >= emptySlots;
            _slots[i].color = filled ? loadedBulletColor : emptyBulletColor;
        }
    }

    void RefreshVisibility()
    {
        if (_root == null)
            return;

        bool show = weaponEquip != null && weaponEquip.IsRifleEquipped;
        if (_root.gameObject.activeSelf != show)
            _root.gameObject.SetActive(show);
    }

    void EnsureBulletRow()
    {
        if (_root != null)
            return;

        var existing = transform.Find("RifleAmmoRow");
        if (existing != null)
        {
            _root = existing.GetComponent<RectTransform>();
            CacheSlots(existing);
            OnLoadedChanged(magazine != null ? magazine.LoadedRounds : 0);
            return;
        }

        var rowGo = new GameObject(
            "RifleAmmoRow",
            typeof(RectTransform),
            typeof(HorizontalLayoutGroup));
        rowGo.transform.SetParent(transform, false);

        _root = rowGo.GetComponent<RectTransform>();
        _root.anchorMin = new Vector2(1f, 0f);
        _root.anchorMax = new Vector2(1f, 0f);
        _root.pivot = new Vector2(1f, 0f);
        _root.anchoredPosition = new Vector2(-margin.x, margin.y);
        _root.sizeDelta = new Vector2(
            HuntingRifleMagazine.Capacity * bulletWidth + (HuntingRifleMagazine.Capacity - 1) * spacing,
            bulletHeight);

        var layout = rowGo.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleRight;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        _slots = new Image[HuntingRifleMagazine.Capacity];
        Sprite sprite = GetBulletSprite();

        for (int i = 0; i < _slots.Length; i++)
        {
            var slotGo = new GameObject(
                $"Bullet_{i + 1}",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            slotGo.transform.SetParent(rowGo.transform, false);

            var rect = slotGo.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(bulletWidth, bulletHeight);

            var layoutElement = slotGo.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = bulletWidth;
            layoutElement.preferredHeight = bulletHeight;

            var image = slotGo.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            _slots[i] = image;
        }

        OnLoadedChanged(magazine != null ? magazine.LoadedRounds : 0);
        RefreshVisibility();
    }

    void CacheSlots(Transform row)
    {
        _slots = new Image[HuntingRifleMagazine.Capacity];
        for (int i = 0; i < _slots.Length; i++)
        {
            var child = row.Find($"Bullet_{i + 1}");
            if (child != null)
                _slots[i] = child.GetComponent<Image>();
        }
    }

    static Sprite GetBulletSprite()
    {
        if (_bulletSprite != null)
            return _bulletSprite;

        const int w = 8;
        const int h = 18;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float nx = (x + 0.5f) / w;
                float ny = (y + 0.5f) / h;
                bool inside = BulletSilhouette(nx, ny);
                tex.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
        }

        tex.Apply();
        _bulletSprite = Sprite.Create(tex, new Rect(0f, 0f, w, h), new Vector2(0.5f, 0.5f), 100f);
        return _bulletSprite;
    }

    static bool BulletSilhouette(float nx, float ny)
    {
        if (ny > 0.72f)
        {
            float t = (ny - 0.72f) / 0.28f;
            float radius = Mathf.Lerp(0.22f, 0.08f, t);
            return Mathf.Abs(nx - 0.5f) <= radius;
        }

        if (ny < 0.12f)
        {
            float t = ny / 0.12f;
            float half = Mathf.Lerp(0.14f, 0.28f, t);
            return Mathf.Abs(nx - 0.5f) <= half;
        }

        return Mathf.Abs(nx - 0.5f) <= 0.28f;
    }
}
