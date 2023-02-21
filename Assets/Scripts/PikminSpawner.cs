using System.Collections.Generic;
using UnityEngine;

public class PikminSpawner : MonoBehaviour
{
    [SerializeField] private int spawnNum = 1;
    public float radius = 1;
    public Pikmin PikminPrefab;

    public void SpawnStartPikmin(ref List<Pikmin> pikminList)
    {
        for (int i = 0; i < spawnNum; i++)
        {
            Pikmin newPikmin = Instantiate(PikminPrefab);
            newPikmin.transform.position = transform.position + (Random.insideUnitSphere * radius);
            pikminList.Add(newPikmin);
        }
    }

    public void SpawnPikmin(ref List<Pikmin> pikminList, int NumberToSpawn)
    {
        for (int i = 0; i < NumberToSpawn; i++)
        {
            Pikmin newPikmin = Instantiate(PikminPrefab);

            newPikmin.transform.position = transform.position + (Random.insideUnitSphere * radius);
            pikminList.Add(newPikmin);
        }
    }
}