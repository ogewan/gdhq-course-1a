using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private Queue<GameObjectWP> spawnQueue = new Queue<GameObjectWP>();
    [SerializeField]
    private Registry _managers;
    [SerializeField]
    private GameObject _portalContainer;
    [SerializeField]
    private Transform _player;
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
    private struct GameObjectWP
    {
        public GameObject item;
        public Transform parent;

        public GameObjectWP (GameObject item, Transform parent)
        {
            this.item = item;
            this.parent = parent;
        }
    }

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
        public int oldSpawnCount;
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
            this.oldSpawnCount = 0;
            this.useParentSpawnPosition = true;
            this.spawnPosition = Vector3.zero;
            this.randomSpawn = new Vector3Bool(false, false, false);
            this.spawnActive = true;
            this.timeRange = _timeRange;
            this.spawnTimer = null;
            this.item = null;
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

        public void freezeSpawn()
        {
            oldSpawnCount = spawnCount;
            spawnCount = 0;
        }

        public void restoreSpawn()
        {
            spawnCount = oldSpawnCount;
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

    public class Randomizer
    {
        [SerializeField]
        private List<GameObject> spawnTable;

        public Randomizer(Spawnlet[] spawnlets)
        {
            this.spawnTable = buildSpawnTable(spawnlets);
        }

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

        public void rebuild(Spawnlet[] items)
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
        //Debug.Log("SM min " + new Vector2(_boundManager.bbox().xMin, _boundManager.bbox().xMax) + " max " + new Vector2(_boundManager.bbox().yMin, _boundManager.bbox().yMax));
        _bbox = _boundManager.bbox();
        _gameManager = _managers.gameManager;
    }

    void enemyHasFirstSpawn(GameObject item, Spawnable spawnable, Transform container)
    {
        // ENEMY UNIQUE
        Enemy enemy;
        if (item.tag == "Enemy")
        {
            enemy = item.GetComponent<Enemy>();
            enemy.getType();
            if (_gameManager.firstSpawn(enemy.getType()))
            {
                specialSpawn(item, spawnable, container);
            }
        }
    }

    float getSpawnTime(Vector2 time)
    {
        Vector2 range = time;
        float minTime = Mathf.Max(0, Mathf.Min(range[0], range[1]));
        float maxTime = Mathf.Min(1, Mathf.Max(range[0], range[1]));
        return Random.Range(minTime, maxTime);
    }

    Vector3 generateSpawnPosition(Spawnable spawnable)
    {
        BoundManager.boundingBox bbox = _boundManager.bbox();
        float spawnableX = spawnable.useParentSpawnPosition ? _spawnPosition.x : spawnable.spawnPosition.x;
        float spawnableY = spawnable.useParentSpawnPosition ? _spawnPosition.y : spawnable.spawnPosition.y;
        float spawnX = spawnable.randomSpawn.x || _randomSpawn.x ? Random.Range(_bbox.xMin, _bbox.xMax) : Mathf.Clamp(spawnableX, _bbox.xMin, _bbox.xMax);
        float spawnY = spawnable.randomSpawn.y || _randomSpawn.y ? Random.Range(_bbox.yMin, _bbox.yMax) : Mathf.Clamp(spawnableY, _bbox.yMin, _bbox.yMax);
        //Debug.Log("Random " + new Vector2(spawnX, spawnY) + " X " + new Vector2(_bbox.xMin, _bbox.xMax) + " Y " + new Vector2(_bbox.yMin, _bbox.yMax));
        return new Vector3(spawnX, spawnY, 0);
    }

    void generateContainerIfNoneAvaliable(Spawnable spawnable)
    {
        if (spawnable.container == null)
        {
            spawnable.container = Instantiate(new GameObject("container"), transform);
        }
    }

    IEnumerator TimedSpawn(Spawnable spawnable)
    {
        yield return new WaitForSeconds(3f);
        generateContainerIfNoneAvaliable(spawnable);
        while (_spawnActive)
        {
            Transform container = spawnable.container.transform;
            bool iAmActive = spawnable.spawnActive;
            float spawnTime = getSpawnTime(spawnable.timeRange);

            yield return new WaitForSeconds(spawnTime);
            if (_gameManager.isPaused()) continue;

            GameObject item;
            Transform parent = null;
            if (spawnQueue.Count > 1)
            {
                GameObjectWP special = spawnQueue.Dequeue();
                item = special.item;
                parent = special.parent;
            }
            else
            {
                item = spawnable.getObject();
            }
            enemyHasFirstSpawn(item, spawnable, container);
            int spawnCount = spawnable.spawnCount;

            int itemCount = container.childCount;
            bool canSpawn = spawnCount >= itemCount || spawnCount == -1 || _gameManager.isEndlessModeUnlocked();
            if (item && canSpawn && iAmActive && _spawnActive)
            {
                Vector3 spawnPos = generateSpawnPosition(spawnable);
                setPausible(item, _managers);
                setEnemy(item, _managers);
                GameObject newItem = Instantiate(item, spawnPos, Quaternion.identity, parent ? parent : container);
                //Debug.Log(newItem.transform + " " + newItem.transform.position);
            }
        }
    }

    void setEnemy(GameObject item, Registry manager)
    {
        if (item.tag == "Enemy")
        {
            item.GetComponent<Enemy>().managers = _managers;
            item.GetComponent<Enemy>().player = _player.transform;
            item.GetComponent<Enemy>().setPortalContainer(_portalContainer);
        }
    }

    public static void setPausible(GameObject item, Registry manager)
    {
        if (!item) return;
        Pausible pauseControl = item.GetComponent<Pausible>();
        if (pauseControl)
        {
            pauseControl.gameManager = manager.gameManager;
        }
    }

    public static void setPausible(GameObject item, GameManager gManager)
    {
        if (!item) return;
        Pausible pauseControl = item.GetComponent<Pausible>();
        if (pauseControl)
        {
            pauseControl.gameManager = gManager;
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

    public void specialSpawn(GameObject item, Spawnable spawnable, Transform parent)
    {
        oldSpawnCount = spawnable.spawnCount;
        spawnable.spawnCount = 1;
        spawnQueue.Enqueue(new GameObjectWP(item, parent));
    }

    public Spawnable[] getItems()
    {
        return items;
    }
}
