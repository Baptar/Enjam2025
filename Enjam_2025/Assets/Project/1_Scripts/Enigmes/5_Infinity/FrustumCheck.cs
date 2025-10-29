using UnityEngine;

public class FrustumCheck : MonoBehaviour
{
    private static Camera cam;          // ta caméra (sinon Camera.main)

    
    private void Start() => cam = PlayerController.instance.playerCamera;

    public static bool IsInFrustum(Camera cam, Renderer renderer)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
    
    public static bool IsInFront(Transform obj, Vector3 point)
    {
        // Direction de l'objet vers le point
        Vector3 dirToPoint = (point - obj.position).normalized;

        // Produit scalaire entre le forward de l'objet et la direction vers le point
        float dot = Vector3.Dot(obj.forward, dirToPoint);

        // Si dot > 0, le point est devant (dans le champ "forward" de l’objet)
        // Si dot < 0, le point est derrière
        return dot > 0f;
    }

    public static bool IsFarAway(Transform obj, Vector3 point, float distanceMin)
    {
        float distance = Vector3.Distance(obj.position, point);
        return distance >= distanceMin;
    }

    public static bool CanSpawnDoor(Renderer renderer) => !IsInFrustum(cam, renderer) &&
                                        IsInFront(renderer.gameObject.transform, cam.transform.position) && 
                                        IsFarAway(renderer.gameObject.transform, cam.transform.position, 8.0f);
}