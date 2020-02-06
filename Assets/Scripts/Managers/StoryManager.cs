using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public SpawnManager spawnManager;
    public int enemySpawnletIndex = 0;

    void Start()
    {
        //_spawnManager = registerManager<SpawnManager>("SpawnManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    T registerManager<T>(string name)
    {
        GameObject manager = GameObject.Find(name);
        if (manager)
        {
            return manager.GetComponent<T>();
        }
        Debug.LogError(name + " not found");
        return default(T);
    }
}
