using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Spawnable[] items = { Spawnable.init() };
    [SerializeField]
    private int[] shared;
    [SerializeField]
    private Vector2 _spawnBounds = new Vector2(-11.5f, 10.5f);
    [SerializeField]
    private float _spawnHeight = 7f;
    [SerializeField]
    private bool _spawnActive = true;

    [System.Serializable]
    public struct Spawnlet
    {
        public GameObject obj;
        public int probability;

        Spawnlet(GameObject _obj, int _probability = 1)
        {
            this.obj = _obj;
            this.probability = _probability;
        }
    }

    [System.Serializable]
    public struct Spawnable
    {
        public Spawnlet[] pool;
        // if no container, send to top level
        public GameObject container;
        public Vector2 timeRange;
        public int spawnCount;
        public Vector2 spawnBounds;
        public float spawnHeight;
        public bool spawnActive;
        [HideInInspector]
        public IEnumerator spawnTimer;
        [HideInInspector]
        public List<GameObject> probabilityTable;

        Spawnable(Spawnlet[] _pool, GameObject _container, Vector2 _timeRange)
        {
            this.pool = _pool;
            this.container = _container;
            this.spawnCount = -1;
            this.spawnBounds = new Vector2(-11.5f, 10.5f);
            this.spawnHeight = 7f;
            this.spawnActive = true;
            this.timeRange = _timeRange;
            this.spawnTimer = null;
            this.probabilityTable = null;
            buildProbabilityTable();
        }
        // whenever the pool is modified, including adding new items or changing probabilities
        // buildProbabilityTable must be called to update the table used to get objects
        public void buildProbabilityTable()
        {
            if (this.pool != null)
            {
                this.probabilityTable = new List<GameObject>();
                foreach (Spawnlet item in this.pool)
                {
                    int count = item.probability;
                    for (int i = 0; i < count; i++)
                    {
                        this.probabilityTable.Add(item.obj);
                    }
                }
            }
        }
        public static Spawnable init()
        {
            return new Spawnable(null, null, new Vector2(5f, 6f));
        }
        public GameObject getObject()
        {
            if (this.probabilityTable == null || this.probabilityTable.Count == 0)
            {
                this.buildProbabilityTable();
            }
            int target = Random.Range(0, this.probabilityTable.Count);
            return this.probabilityTable[target];
        }
    }

    void setTimer(Spawnable item)
    {
        if (item.pool != null && item.pool.Length > 0)
        {
            item.spawnTimer = TimedSpawn(item);
            StartCoroutine(item.spawnTimer);
        } else
        {
            Debug.LogError("Spawnable has not been set");
        }
    }

    IEnumerator TimedSpawn(Spawnable spawnable)
    {
        yield return new WaitForSeconds(3f);
        while (_spawnActive)
        {
            GameObject item = spawnable.getObject();
            Transform container = (spawnable.container) ? spawnable.container.transform : null;
            Vector2 range = spawnable.timeRange;
            int spawnCount = spawnable.spawnCount;
            Vector2 spawnBounds = (spawnable.spawnBounds != new Vector2()) ? spawnable.spawnBounds : _spawnBounds;
            float spawnHeight = (spawnable.spawnHeight != 0f) ? spawnable.spawnHeight : _spawnHeight;
            bool iAmActive = spawnable.spawnActive;

            float minTime = Mathf.Max(0, Mathf.Min(range[0], range[1]));
            float maxTime = Mathf.Max(1, Mathf.Max(range[0], range[1]));
            float spawnTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(spawnTime);
            int itemCount = GameObject.FindGameObjectsWithTag(item.tag).Length;
            bool canSpawn = spawnCount >= itemCount || spawnCount == -1;
            if (item && canSpawn && iAmActive && _spawnActive)
            {
                float xMin = spawnBounds[0];
                float xMax = spawnBounds[1];
                float spawnX = Random.Range(xMin, xMax);
                Vector3 spawnPos = new Vector3(spawnX, spawnHeight, 0);
                Instantiate(item, spawnPos, Quaternion.identity, container);
            }
        }
    }

    public void playerDeath()
    {
        _spawnActive = false;
    }

    public void startSpawning()
    {
        foreach (Spawnable item in items)
        {
            setTimer(item);
        }
    }
}
