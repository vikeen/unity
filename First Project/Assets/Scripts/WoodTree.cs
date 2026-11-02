using UnityEngine;

/// <summary>Marker on tree roots for hold-to-harvest raycasts.</summary>
public class WoodTree : MonoBehaviour, IInteractable
{
    [SerializeField] string promptLabel = "Tree";
    [SerializeField] float harvestHoldSeconds = 3f;

    public string PromptLabel => promptLabel;
    public float HarvestHoldSeconds => harvestHoldSeconds;

    public void CompleteHarvest(PlayerInventory inventory)
    {
        if (inventory != null)
            inventory.AddWood(1);

        Destroy(gameObject);
    }

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
