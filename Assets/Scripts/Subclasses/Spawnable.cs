using System.Collections;
using System.Linq;
using UnityEngine;
/// <summary>
/// Summary description for Class1
/// </summary>
[System.Serializable]
public class Spawnable
{
    public Spawnlet[] pool;
    // if no container, send to top level
    public GameObject container;
    public Vector2 timeRange;
    public int spawnCount;
    public bool useParentSpawnPosition;
    public Vector2 spawnPosition;
    public SpawnManager.Vector3Bool randomSpawn;
    public bool spawnActive;
    [HideInInspector]
    public IEnumerator spawnTimer;
    public Randomizer item;

    [System.Serializable]
    public struct Spawnlet
    {
        public GameObject obj;
        public int probability;

        public Spawnlet(GameObject _obj, int _probability = 1)
        {
            this.obj = _obj;
            this.probability = _probability;
        }
    }

    Spawnable(Spawnlet[] _pool, GameObject _container, Vector2 _timeRange)
    {
        pool = _pool;
        container = _container;
        spawnCount = -1;
        useParentSpawnPosition = true;
        spawnPosition = Vector3.zero;
        randomSpawn = new SpawnManager.Vector3Bool(false, false, false);
        spawnActive = true;
        timeRange = _timeRange;
        spawnTimer = null;
        item = null;
    }

    public GameObject getObject()
    {
        init();
        GameObject target = item.Get();
        if (target == null)
        {
            rebuild();
            target = item.Get();
        }
        return target;
    }

    public void rebuild()
    {
        init();
        item.rebuild(pool);
    }

    public void changeOdds(int index, int probability)
    {
        pool[index].probability = probability;
        rebuild();
    }

    private void init()
    {
        if (item == null)
        {
            this.item = new Randomizer(pool);
        }
    }

    public bool getActive()
    {
        return this.spawnActive;
    }

    public void setActive(bool active)
    {
        this.spawnActive = active;
    }

    public void clearSpawned()
    {
        if (container)
        {
            foreach (Transform child in container.transform.Cast<Transform>().ToArray())
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}