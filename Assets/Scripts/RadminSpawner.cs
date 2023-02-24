using System.Collections.Generic;
using UnityEngine;

public class RadminSpawner : MonoBehaviour
{
    [SerializeField] private int spawnNum = 1;
    public float radius = 1;
    public Radmin RadminPrefab;

    public void SpawnStartRadmin(ref List<Radmin> radminList)
    {
        for (int i = 0; i < spawnNum; i++)
        {
            Radmin newRadmin = Instantiate(RadminPrefab);
            newRadmin.transform.position = transform.position + (Random.insideUnitSphere * radius);
            radminList.Add(newRadmin);
        }
    }

    public void SpawnRadmin(ref List<Radmin> radminList, int NumberToSpawn)
    {
        for (int i = 0; i < NumberToSpawn; i++)
        {
            Radmin newRadmin = Instantiate(RadminPrefab);

            newRadmin.transform.position = transform.position + (Random.insideUnitSphere * radius);
            radminList.Add(newRadmin);
        }
    }
}