using UnityEngine;

public static class InteractionQueries
{
    public static bool TryGetWoodTree(Camera camera, float reach, out WoodTree tree)
    {
        tree = null;
        if (camera == null)
            return false;

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, reach);
        float closest = float.PositiveInfinity;

        for (int i = 0; i < hits.Length; i++)
        {
            WoodTree candidate = hits[i].collider.GetComponentInParent<WoodTree>();
            if (candidate == null)
                continue;

            if (hits[i].distance >= closest)
                continue;

            closest = hits[i].distance;
            tree = candidate;
        }

        return tree != null;
    }
}
