using UnityEngine;
using UnityEngine.Rendering;

/// <summary>Spawns and orients a weapon view model under the FPS camera.</summary>
public class FirstPersonWeaponEquip : MonoBehaviour
{
    [SerializeField] Transform viewCamera;
    [SerializeField] GameObject weaponViewPrefab;
    [Header("FPS hold (camera-local)")]
    [SerializeField] Vector3 localPosition = new(0.28f, -0.22f, 0.52f);
    [SerializeField] Vector3 localEulerAngles = new(-4f, 0f, 0f);
    [SerializeField] float uniformScale = 1f;
    [SerializeField] float viewCameraNearClip = 0.01f;

    Transform _weaponRoot;

    public bool IsRifleEquipped => _weaponRoot != null;

    void Awake()
    {
        if (weaponViewPrefab == null)
            return;

        ResolveViewCamera();
        if (viewCamera == null)
            return;

        var cam = viewCamera.GetComponent<Camera>();
        if (cam != null && viewCameraNearClip > 0f)
            cam.nearClipPlane = viewCameraNearClip;

        var anchor = new GameObject("WeaponViewAnchor").transform;
        anchor.SetParent(viewCamera, false);
        anchor.localPosition = localPosition;
        anchor.localRotation = Quaternion.Euler(localEulerAngles);
        anchor.localScale = Vector3.one;

        GameObject instance = Instantiate(weaponViewPrefab, anchor);
        instance.name = weaponViewPrefab.name;
        _weaponRoot = instance.transform;
        _weaponRoot.localPosition = Vector3.zero;
        _weaponRoot.localRotation = Quaternion.identity;
        _weaponRoot.localScale = Vector3.one * uniformScale;

        foreach (Renderer r in instance.GetComponentsInChildren<Renderer>())
        {
            r.shadowCastingMode = ShadowCastingMode.Off;
            r.receiveShadows = false;
        }
    }

    void ResolveViewCamera()
    {
        if (viewCamera != null)
            return;

        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
            viewCamera = cam.transform;
    }
}
