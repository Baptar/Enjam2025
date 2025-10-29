using System.Collections.Generic;
using UnityEngine;

public class InfinitySystem : MonoBehaviour
{
    // spawn 5-10 corridors
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private GameObject spawnPointNext;
    
    private GameObject previsousNew;
    
    public void Spawn5Corridors(GameObject firstPrevious)
    {
        previsousNew = firstPrevious;
        
        GameObject go = CorridorsManager.instance.GetCorridorAssociated(4);
        if (go ==null) return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject newObj = Instantiate(go, spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
            if (newObj.TryGetComponent(out HelperInfinity inj))
            {
                inj.RemoveElement();
            }
            if (newObj.TryGetComponent(out CorridorGenerated corridorGenerated))
            {
                corridorGenerated.previousCorridor = previsousNew;
                previsousNew = newObj;
            }
        }

        CorridorsManager.instance.nextTransformCorridorSpawnINFINITY = spawnPointNext.transform;
        CorridorsManager.instance.lastInfinityCorridorCreated = previsousNew;
    }
}
