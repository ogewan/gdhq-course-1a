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
    private GameObject _powerupContainer;
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

    void enemyHasFirstSpawn_superCheck(GameObject item, Spawnable spawnable)
    {
        // ENEMY UNIQUE
        Enemy enemy;
        if (item.tag == "Enemy" && _gameManager.getMode() == GameManager.mode.classic)
        {
            enemy = item.GetComponent<Enemy>();
            Enemy.type type = enemy.getType();
            if (type == Enemy.type.Super && _gameManager.firstSpawn(type))
            {
                spawnable.setActive(false);
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
            bool iAmActive = spawnable.getActive();
            float spawnTime = getSpawnTime(spawnable.timeRange);

            yield return new WaitForSeconds(spawnTime);
            if (_gameManager.isPaused()) continue;

            GameObject item;
            Transform parent = null;
            bool specialSpawn = spawnQueue.Count > 0;
            if (specialSpawn)
            {
                GameObjectWP special = spawnQueue.Dequeue();
                item = special.item;
                parent = special.parent;
            }
            else
            {
                item = spawnable.getObject();
            }
            enemyHasFirstSpawn_superCheck(item, spawnable);
            int spawnCount = spawnable.spawnCount;

            int itemCount = container.childCount + 1;
            bool canSpawn =  spawnCount >= itemCount || spawnCount == -1 || _gameManager.isEndlessModeUnlocked();
            if (item && (specialSpawn || canSpawn && iAmActive) && _spawnActive)
            {
                Vector3 spawnPos = generateSpawnPosition(spawnable);
                setPausible(item, _managers);
                setEnemy(item, _managers);
                setPowerup(item, _managers);
                GameObject newItem = Instantiate(item, spawnPos, Quaternion.identity, parent ? parent : container);
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
            item.GetComponent<Enemy>().setPowerupContainer(_powerupContainer);
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

    void setPowerup(GameObject item, Registry manager)
    {
        if (item.tag == "Powerup" || item.tag == "Star")
        {
            Player player = _player.GetComponent<Player>();
            if (player)
            {
                Powerup powerup = item.GetComponent<Powerup>();
                powerup.player = player;
                powerup.playerPosition = _player;
            } else
            {
                Debug.Log("NO PLAYER SET");
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

    public void specialSpawn(GameObject item, Transform parent)
    {
        spawnQueue.Enqueue(new GameObjectWP(item, parent));
    }

    [System.Serializable]
    public struct spawnIngredient
    {
        public GameObject item;
        public Transform parent;
        public int count;

        public spawnIngredient(GameObject item, Transform parent, int count = 1)
        {
            this.item = item;
            this.parent = parent;
            this.count = count;
        }
    }

    public void spawnRecipe(spawnIngredient[] ingredients)
    {
        foreach (spawnIngredient ingredient in ingredients)
        {
            for (int i = 0; i < ingredient.count; i++)
            {
                spawnQueue.Enqueue(new GameObjectWP(ingredient.item, ingredient.parent));
            }
        }
    }

    public Spawnable[] getItems()
    {
        return items;
    }
}
