using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Left-click rifle fire from the view camera.</summary>
public class HuntingRifleFire : MonoBehaviour
{
    [SerializeField] Camera viewCamera;
    [SerializeField] FirstPersonWeaponEquip weaponEquip;
    [SerializeField] HuntingRifleMagazine magazine;
    [SerializeField] float range = 120f;
    [SerializeField] float harvestCrosshairReach = 6f;
    [SerializeField] AudioClip shootClip;
    [SerializeField] float shootVolume = 0.9f;

    InventoryPanelUI _inventoryPanel;
    AudioSource _audioSource;

    void Awake()
    {
        if (weaponEquip == null)
            weaponEquip = GetComponent<FirstPersonWeaponEquip>();
        if (magazine == null)
            magazine = GetComponent<HuntingRifleMagazine>();

        if (viewCamera == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                viewCamera = cam;
        }

        _inventoryPanel = FindFirstObjectByType<InventoryPanelUI>();
        EnsureAudioSource();
    }

    void EnsureAudioSource()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource != null)
            return;

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (Cursor.lockState != CursorLockMode.Locked || Cursor.visible)
            return;

        if (_inventoryPanel != null && _inventoryPanel.IsOpen)
            return;

        if (weaponEquip == null || !weaponEquip.IsRifleEquipped)
            return;

        if (magazine == null || magazine.LoadedRounds <= 0)
            return;

        ResolveViewCamera();
        if (viewCamera == null)
            return;

        if (InteractionQueries.TryGetWoodTree(viewCamera, harvestCrosshairReach, out _))
            return;

        if (!magazine.TryConsumeRound())
            return;

        PlayShootSound();
        TryHitTarget();
    }

    void PlayShootSound()
    {
        if (shootClip == null || _audioSource == null)
            return;

        _audioSource.PlayOneShot(shootClip, shootVolume);
    }

    void ResolveViewCamera()
    {
        if (viewCamera != null)
            return;

        viewCamera = GetComponentInChildren<Camera>();
        if (viewCamera == null)
            viewCamera = Camera.main;
    }

    void TryHitTarget()
    {
        Ray ray = new Ray(viewCamera.transform.position, viewCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            range,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        if (hits.Length == 0)
            return;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            Wolf wolf = hits[i].collider.GetComponentInParent<Wolf>();
            if (wolf != null)
            {
                wolf.Kill();
                return;
            }

            return;
        }
    }
}
