using UnityEngine;

/// <summary>Marker on tree roots for click-to-harvest raycasts.</summary>
public class WoodTree : MonoBehaviour, IInteractable
{
    [SerializeField] string promptLabel = "Tree";

    public string PromptLabel => promptLabel;

    void Awake()
    {
        EnsureFoliageCollider();
    }

    void EnsureFoliageCollider()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name != "Foliage")
                continue;

            if (child.GetComponent<Collider>() != null)
                return;

            var sphere = child.gameObject.AddComponent<SphereCollider>();
            sphere.radius = 0.5f;
            return;
        }
    }
}
