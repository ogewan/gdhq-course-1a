using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private GameManager.mode _mode;
    private bool _debug;
    private GameManager _newManager;
    public GameObject endlessButton;

    private void Awake()
    {
        GameManager oldManager = registerManager<GameManager>("old");
        if (oldManager)
        {
            endlessButton.SetActive(oldManager.isEndlessModeUnlocked());
            Destroy(oldManager.gameObject);
        }
    }

    public void waveGame()
    {
        _mode = GameManager.mode.wave;
        loadGame();
    }

    public void classicGame()
    {
        _mode = GameManager.mode.classic;
        loadGame();
    }

    public void endlessGame()
    {
        _mode = GameManager.mode.endless;
        loadGame();
    }

    public void waveDebug()
    {
        _debug = true;
        waveGame();
    }

    public void classicDebug()
    {
        _debug = true;
        classicGame();
    }

    public void helpMenu()
    {
        SceneManager.LoadScene(2);
    }

    public GameManager.mode getMode()
    {
        return _mode;
    }

    public bool debugMode()
    {
        return _debug;
    }

    public void loadGame()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(1); //main game scene
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

