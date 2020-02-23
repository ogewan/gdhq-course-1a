//using System.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Story Manager is responsible for change spawn behavior based on certain conditions
 */
public class StoryManager : MonoBehaviour
{
    public Registry managers;
    public GameObject hyperEnemy;
    public GameObject ultraEnemy;
    public GameObject tohouEnemy;
    public GameObject starCollectible;
    public int enemySpawnableIndex = 0;
    public GameObject _portalContainer;

    private SpawnManager _spawnManager;
    private BoundManager _boundManager;
    [SerializeField]
    private Transform _playerPosition;
    [SerializeField]
    private Transform _enemyContainer;
    [SerializeField]
    private Transform _powerupContainer;
    [SerializeField]
    private enemyStatus _enemyTriggers = new enemyStatus(0);
    [SerializeField]
    private int _superSpawnIndex = 1;
    [SerializeField]
    private int _superSpawnCount = 1;
    [SerializeField]
    private int _hyperSpawnIndex = 2;
    [SerializeField]
    private int _hyperSpawnCount = 0;
    [SerializeField]
    private int _ultraSpawnIndex = 3;
    [SerializeField]
    private int _ultraSpawnCount = 0;
    [SerializeField]
    private int _stage = 1;
    [SerializeField]
    private int _recordedScore = 0;
    private int[] _scoreGates = { 0, 100, 200, 300, 400, 500, 1000, 1100, 1200, 1300, 1400, 1500, 2000, 3000, 6000, 9000, 10000, 20000, 30000, 40000, 50000, 60000, 70000, 80000, 90000, 100000 };
    private Dictionary<Enemy.type, int> _killTriggers = new Dictionary<Enemy.type, int>
    {
        { Enemy.type.Normal, 0 },
        { Enemy.type.Super, 3 },
        { Enemy.type.Hyper, 3 },
        { Enemy.type.Ultra, 3 },
        { Enemy.type.Tohou, 1 }
    };

    [System.Serializable]
    public struct enemyStatus
    {
        public Dictionary<Enemy.type, bool> first;
        public Dictionary<Enemy.type, int> killed;

        public enemyStatus(int init = 0)
        {
            this.first = new Dictionary<Enemy.type, bool>();
            this.killed = new Dictionary<Enemy.type, int>();
        }
    }

    void Start()
    {
        _spawnManager = managers.spawnManager;
        _boundManager = managers.boundManager;
    }

    bool multipleOf3(Enemy.type type)
    {
        Dictionary<Enemy.type, int> killed = _enemyTriggers.killed;
        return killed[type] % 3 == 0 && killed[type] == 0;
    }

    int floorMinima(int value, int[] sortedArray)
    {
        int minima = value;

        foreach (int i in sortedArray)
        {
            if (value <= i)
            {
                return minima;
            }
            minima = i;
        }
        return minima;
    }

    public bool firstSpawn(Enemy.type type)
    {
        bool status = _enemyTriggers.first.ContainsKey(type);
        if (status)
        {
            return false;
        }
        _enemyTriggers.first.Add(type, true);
        return true;
    }

    public void addKill(Enemy.type type)
    {
        Dictionary<Enemy.type, int> killed = _enemyTriggers.killed;
        SpawnManager.Spawnable[] items = _spawnManager.getItems();
        _spawnManager.getItems()[enemySpawnableIndex].freezeSpawn();

        if (!killed.ContainsKey(type))
        {
            killed[type] = 0;
        }
        killed[type]++;
        if (killed[type] == _killTriggers[type])
        {
            //Generally, the stage will be change to a specific number for a killed type
            //However, enemy types are spawned sequentially, so it shouldn't be possible to hit gates out of order
            _stage++;
            switch (_stage)
            {
                case 1:
                    break;
                case 2:
                    _spawnManager.specialSpawn(hyperEnemy, items[0], _enemyContainer);
                    break;
                case 3:
                    _spawnManager.specialSpawn(ultraEnemy, items[0], _enemyContainer);
                    break;
                case 4:
                    GameObject tohou = _boundManager.bsInstantiate(tohouEnemy, new Vector3(0, 4), Quaternion.identity);

                    SpawnManager.setPausible(tohou, managers);
                    tohou.GetComponent<Enemy>().managers = managers;
                    tohou.GetComponent<Enemy>().player = _playerPosition;
                    tohou.transform.parent = _enemyContainer;
                    tohou.GetComponent<Enemy>().setPortalContainer(_portalContainer);
                    _spawnManager.getItems()[enemySpawnableIndex].freezeSpawn();
                    break;
                case 5:
                    _spawnManager.getItems()[enemySpawnableIndex].restoreSpawn();
                    //Spawn star
                    GameObject star =_boundManager.bsInstantiate(starCollectible, new Vector3(0, 6), Quaternion.identity);
                    star.transform.parent = _powerupContainer;
                    break;
            }
        }
    }

    public void scoreCheck(int score)
    {
        int quantizedScore = floorMinima(score, _scoreGates);
        SpawnManager.Spawnable[] items = _spawnManager.getItems();

        if (_recordedScore != quantizedScore)
        {
            _recordedScore = quantizedScore;
            GameObject target = null;
            bool changeOdds = true;
            int targetIndex = 0;
            int targetCount = 0;
            if (quantizedScore < 2000 && _superSpawnCount < 11)
            {
                targetIndex = _superSpawnIndex;
                targetCount = ++_superSpawnCount;
            }
            else if (quantizedScore < 10000 && _hyperSpawnCount < 6)
            {
                target = hyperEnemy;
                targetIndex = _hyperSpawnIndex;
                targetCount = ++_hyperSpawnCount;
            }
            else if (_ultraSpawnCount < 6)
            {
                target = ultraEnemy;
                targetIndex = _ultraSpawnIndex;
                targetCount = ++_ultraSpawnCount;
            }
            else
            {
                changeOdds = false;
            }
            if (changeOdds) items[0].changeOdds(targetIndex, targetCount);
            if (target != null) _spawnManager.specialSpawn(target, items[0], _enemyContainer);
        }
    }
}

/*
Game modes limit enemy count to 5
When first spawn of non normal enemies, enemy is added to spawnQueue, and spawnCount is set to 1

Stage 1:
9/10 normal 1/10 super
score+100 = +1/10
@ score 100 9/11 normal 2/11 super
@ score 200 9/12 normal 3/12 super
@ score 300 9/13 normal 4/13 super
@ score 400 9/14 normal 5/14 super
@ score 500 9/15 normal 6/15 super (final)
@ score 1000 0/1 normal 1/1 super (Force Spawn)

Stage 2: super
9/15 normal 6/15 super
@ score 1100 9/16 normal 7/16 super
@ score 1200 9/17 normal 8/17 super
@ score 1300 9/18 normal 9/18 super
@ score 1400 9/19 normal 10/19 super
@ score 1500 9/20 normal 11/20 super
@ score 2000 0/1 normal 0/1 super 1/1 hyper (Force Spawn)

Stage 3: hyper, at each score gate, the hyper enemy will be forced to spawn and then it will go back to the normal
9/21 normal 11/21 super 1/1 hyper
@ score 3000 0/1 normal 0/1 super 1/1 hyper (Force Spawn)
@ score 6000
@ score 9000

Stage 4: ultra, at each score gate, the ultra enemy will be forced to spawn and then it will go back to the normal
9/21 hyper 11/21 super 1/1 ultra
@ score 10000 0/1 normal 0/1 super 1/1 hyper (Force Spawn)
@ score 20000
@ score 30000

Stage 5: Once 3 Ultra Killed
1/1 asteroid

Stage 6:
Star Collectible
Display Message: Endless Mode Unlocked

*/
