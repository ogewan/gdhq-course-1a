using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public Registry managers;
    private SpawnManager _spawnManager;
    [SerializeField]
    private enemyStatus _enemyTriggers = new enemyStatus(0);
    [SerializeField]
    private int _stage = 1;
    private int[][] _scoreTriggers = {
        new int[] { },
        new int[] { 100, 200, 300, 400, 500 },
        new int[] { 1100, 1200, 1300, 1400, 1500, 2000 },
        new int[] { 3000, 6000, 9000 },
        new int[] { 100000, 20000, 30000 },
        new int[] { },
        new int[] { }
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

    public void killCount(Enemy.type type)
    {
        Dictionary<Enemy.type, int> killed = _enemyTriggers.killed;
        bool status = killed.ContainsKey(type);
        
        if (status)
        {
            killed[type]++;
            if (killed[type] % 3 == 0 && killed[type] == 0)
            {

            }
        }
    }

    bool multipleOf3(Enemy.type type)
    {
        Dictionary<Enemy.type, int> killed = _enemyTriggers.killed;
        return killed[type] % 3 == 0 && killed[type] == 0;
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
