using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Registry _managers;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Queue<GameObject> spawnQueue = new Queue<GameObject>();
    [SerializeField]
    private Spawnable[] items;
    [SerializeField]
    private Vector3 _spawnPosition = Vector3.zero;
    [SerializeField]
    private Vector3Bool _randomSpawn = new Vector3Bool(true, false, false);
    [SerializeField]
    private bool _spawnActive = true;
    [SerializeField]
    private int oldSpawnCount;
    private BoundManager _boundManager;
    private BoundManager.boundingBox _bbox;
    private GameManager _gameManager;

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

    [System.Serializable]
    public struct Spawnable
    {
        public Spawnlet[] pool;
        // if no container, send to top level
        public GameObject container;
        public Vector2 timeRange;
        public int spawnCount;
        public bool useParentSpawnPosition;
        public Vector2 spawnPosition;
        public Vector3Bool randomSpawn;
        public bool spawnActive;
        [HideInInspector]
        public IEnumerator spawnTimer;
        public Randomizer item;

        Spawnable(Spawnlet[] _pool, GameObject _container, Vector2 _timeRange)
        {
            this.pool = _pool;
            this.container = _container;
            this.spawnCount = -1;
            this.useParentSpawnPosition = true;
            this.spawnPosition = Vector3.zero;
            this.randomSpawn = new Vector3Bool(false, false, false);
            this.spawnActive = true;
            this.timeRange = _timeRange;
            this.spawnTimer = null;
            this.item = default;
        }

        public GameObject getObject()
        {
            GameObject item = this.item.Get();
            if (item == null)
            {
                this.item = new Randomizer(this.pool);
                item = this.item.Get();
            }
            return item;
        }

        public void init()
        {
            this.item = new Randomizer(this.pool);
        }
    }

    [System.Serializable]
    public struct Vector3Bool
    {
        public bool x;
        public bool y;
        public bool z;

        public Vector3Bool(bool x, bool y, bool z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    [System.Serializable]
    public struct Randomizer
    {
        [SerializeField]
        private Spawnlet[] pool;
        [SerializeField]
        private List<GameObject> spawnTable;

        private static List<GameObject> buildSpawnTable(Spawnlet[] items)
        {
            List<GameObject> table = new List<GameObject>();
            foreach (Spawnlet item in items)
            {
                int count = item.probability;
                for (int i = 0; i < count; i++)
                {
                    table.Add(item.obj);
                }
            }
            return table;
        }

        public void changeOdds(int index, int probability)
        {
            this.pool[index].probability = probability;
            this.spawnTable = buildSpawnTable(this.pool);
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

        public Randomizer(Spawnlet[] spawnlets)
        {
            this.pool = spawnlets;
            this.spawnTable = buildSpawnTable(spawnlets);
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

    void Start()
    {
        _boundManager = _managers.boundManager;
        _bbox = _boundManager.bbox();
        _gameManager = _managers.gameManager;
    }

    IEnumerator TimedSpawn(Spawnable spawnable)
    {
        yield return new WaitForSeconds(3f);

        if (spawnable.container == null)
        {
            spawnable.container = Instantiate(new GameObject("container"), transform);
        }
        while (_spawnActive)
        {
            Transform container = spawnable.container.transform;
            Vector2 range = spawnable.timeRange;
            int spawnCount = spawnable.spawnCount;
            bool iAmActive = spawnable.spawnActive;
            float minTime = Mathf.Max(0, Mathf.Min(range[0], range[1]));
            float maxTime = Mathf.Min(1, Mathf.Max(range[0], range[1]));
            float spawnTime = Random.Range(minTime, maxTime);

            yield return new WaitForSeconds(spawnTime);

            GameObject item = spawnQueue.Count > 1 ? spawnQueue.Dequeue() : spawnable.getObject();
            // ENEMY UNIQUE
            Enemy enemy;
            if (item.tag == "Enemy")
            {
                enemy = item.GetComponent<Enemy>();
                enemy.getType();
                if (_gameManager.firstSpawn(enemy.getType()))
                {
                    oldSpawnCount = spawnCount;
                    spawnCount = 1;
                    spawnQueue.Enqueue(item);
                }
            }
            int itemCount = container.childCount;
            bool canSpawn = spawnCount >= itemCount || spawnCount == -1;
            if (item && canSpawn && iAmActive && _spawnActive)
            {
                BoundManager.boundingBox bbox = _boundManager.bbox();
                float spawnableX = spawnable.useParentSpawnPosition ? _spawnPosition.x : spawnable.spawnPosition.x;
                float spawnableY = spawnable.useParentSpawnPosition ? _spawnPosition.y : spawnable.spawnPosition.y;
                float spawnX = spawnable.randomSpawn.x || _randomSpawn.x ? Random.Range(_bbox.xMin, _bbox.xMax) : Mathf.Clamp(spawnableX, _bbox.xMin, _bbox.xMax);
                float spawnY = spawnable.randomSpawn.y || _randomSpawn.y ? Random.Range(_bbox.yMin, _bbox.yMax) : Mathf.Clamp(spawnableY, _bbox.yMin, _bbox.yMax);
                Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);
                GameObject newItem = Instantiate(item, spawnPos, Quaternion.identity, container);
                setPausible(newItem, _managers);
                if (newItem.tag == "Enemy")
                {
                    newItem.GetComponent<Enemy>().managers = _managers;
                    newItem.GetComponent<Enemy>().player = _player.transform;
                }
            }
        }
    }

    public static void setPausible(GameObject item, Registry manager)
    {
        Pausible pauseControl = item.GetComponent<Pausible>();
        if (pauseControl)
        {
            pauseControl.gameManager = manager.gameManager;
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
