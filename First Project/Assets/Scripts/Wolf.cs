using System.Collections;
using UnityEngine;

/// <summary>Playground wolf target; rifle shots call <see cref="Kill"/>.</summary>
public class Wolf : MonoBehaviour
{
    [SerializeField] float colliderPadding = 0.05f;
    [SerializeField] float fallDuration = 0.28f;
    [SerializeField] Vector3 flatLocalEulerOffset = new(180f, 90f, 90f);
    [SerializeField] float groundRayStartHeight = 4f;
    [SerializeField] float groundRayDistance = 12f;

    bool _dying;
    Collider _hitCollider;

    public bool IsDead => _dying;

    void Awake()
    {
        FitHitColliderToMesh();
    }

    void FitHitColliderToMesh()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            EnsureFallbackCapsule();
            return;
        }

        Bounds worldBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            worldBounds.Encapsulate(renderers[i].bounds);

        worldBounds.Expand(colliderPadding);

        if (_hitCollider != null)
            Destroy(_hitCollider);

        var box = gameObject.AddComponent<BoxCollider>();
        Vector3 lossy = transform.lossyScale;
        box.center = transform.InverseTransformPoint(worldBounds.center);
        box.size = new Vector3(
            worldBounds.size.x / Mathf.Max(lossy.x, 0.0001f),
            worldBounds.size.y / Mathf.Max(lossy.y, 0.0001f),
            worldBounds.size.z / Mathf.Max(lossy.z, 0.0001f));
        _hitCollider = box;
    }

    void EnsureFallbackCapsule()
    {
        if (_hitCollider != null)
            return;

        var capsule = gameObject.AddComponent<CapsuleCollider>();
        capsule.center = new Vector3(0f, 0.55f, 0f);
        capsule.radius = 0.45f;
        capsule.height = 1.1f;
        capsule.direction = 1;
        _hitCollider = capsule;
    }

    public void Kill()
    {
        if (_dying)
            return;

        _dying = true;
        if (_hitCollider != null)
            _hitCollider.enabled = false;

        StartCoroutine(FallFlatOnGround());
    }

    IEnumerator FallFlatOnGround()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(flatLocalEulerOffset);

        float duration = Mathf.Max(0.01f, fallDuration);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
        AlignBottomToGround();
    }

    void AlignBottomToGround()
    {
        Bounds bounds = GetWorldRendererBounds();
        float groundY = SampleGroundHeight(bounds.center);
        float lift = groundY - bounds.min.y;
        if (Mathf.Abs(lift) > 0.0001f)
            transform.position += new Vector3(0f, lift, 0f);
    }

    float SampleGroundHeight(Vector3 around)
    {
        Vector3 origin = around + Vector3.up * groundRayStartHeight;
        if (Physics.Raycast(
                origin,
                Vector3.down,
                out RaycastHit hit,
                groundRayDistance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
        {
            return hit.point.y;
        }

        return 0f;
    }

    Bounds GetWorldRendererBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return bounds;
    }
}
