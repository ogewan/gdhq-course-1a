using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausible : MonoBehaviour
{
    public GameManager gameManager;

    void start()
    {
        if (!gameManager)
        {
            //Debug.LogWarning("GameManager for Pausible component is not set. Using slower Find method. Fix this before production.");
            //gameManager = registerManager<GameManager>("GameManager");
        }
    }

    public bool isPaused()
    {
        return !gameManager ? false : gameManager.isPaused();
    }

    T registerManager<T>(string name)
    {
        GameObject manager = GameObject.Find(name);
        if (manager)
        {
            return manager.GetComponent<T>();
        }
        return default(T);
    }
}
