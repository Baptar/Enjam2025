using UnityEngine;

public class FrustumCheck : MonoBehaviour
{
    private static Camera cam;

    private void Start()
    {
        cam = PlayerController.instance?.playerCamera ?? Camera.main;
    }

    /// <summary>
    /// Vérifie si le renderer est visible dans le frustum de la caméra.
    /// </summary>
    public static bool IsInFrustum(Camera cam, Renderer renderer)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    /// <summary>
    /// Détecte si le point est derrière l'objet par rapport à son forward.
    /// </summary>
    // Vérifie si un point est derrière l'objet (par rapport à son avant visuel)
    // Fonction qui renvoie true si la position est devant l'objet
    public static bool IsBehind(Transform obj, Vector3 targetPos, Vector3 lookDir)
    {
        Vector3 toTarget = targetPos - obj.position;
        toTarget.y = 0; // ignore la hauteur si nécessaire

        if (toTarget.sqrMagnitude < 0.001f)
            return false;

        Vector3 forward = lookDir;
        forward.y = 0; // s'assurer que c'est horizontal
        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.forward; // fallback

        forward.Normalize();

        float dot = Vector3.Dot(forward, toTarget.normalized);
        return dot < 0f;
    }


    /// <summary>
    /// Vérifie si un point est suffisamment loin de l'objet
    /// </summary>
    public static bool IsFarAway(Transform obj, Vector3 point, float distanceMin)
    {
        float distance = Vector3.Distance(obj.position, point);
        return distance >= distanceMin;
    }

    /// <summary>
    /// Vérifie si la porte peut être spawnée : non visible, derrière l'objet et assez loin
    /// </summary>
    public static bool CanSpawnDoor(Renderer renderer, Vector3 lookDir, float minDistance = 8.0f)
    {
        bool isInFrustum = IsInFrustum(cam, renderer);
        bool isBehind = IsBehind(renderer.transform, cam.transform.position, lookDir); // par rapport à l’objet lui-même
        bool isFarAway = IsFarAway(renderer.transform, cam.transform.position, minDistance);

        Debug.Log($"isInFrustum: {isInFrustum}, isBehind: {isBehind}, isFarAway: {isFarAway}");

        return !isInFrustum && isBehind && isFarAway;
    }
}
