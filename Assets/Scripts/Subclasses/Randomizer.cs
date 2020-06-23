using System.Collections.Generic;
using UnityEngine;

public class Randomizer
{
    [SerializeField]
    private List<GameObject> spawnTable;

    public Randomizer(Spawnable.Spawnlet[] spawnlets)
    {
        this.spawnTable = buildSpawnTable(spawnlets);
    }

    private static List<GameObject> buildSpawnTable(Spawnable.Spawnlet[] items)
    {
        List<GameObject> table = new List<GameObject>();
        foreach (Spawnable.Spawnlet item in items)
        {
            int count = item.probability;
            for (int i = 0; i < count; i++)
            {
                table.Add(item.obj);
            }
        }
        return table;
    }

    public void rebuild(Spawnable.Spawnlet[] items)
    {
        this.spawnTable = buildSpawnTable(items);
    }

    public GameObject Get()
    {
        if (this.spawnTable.Count <= 0)
        {
            return null;
        }
        int target = Random.Range(0, this.spawnTable.Count);
        return this.spawnTable[target];
    }
}