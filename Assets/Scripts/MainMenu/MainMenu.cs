using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private GameManager.mode _mode;
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

    public void easyGame()
    {
        _mode = GameManager.mode.easy;
        loadGame();
    }

    public void hardGame()
    {
        _mode = GameManager.mode.hard;
        loadGame();
    }

    public void endlessGame()
    {
        _mode = GameManager.mode.endless;
        loadGame();
    }

    public void debugMode()
    {
        _mode = GameManager.mode.debug;
        loadGame();
    }

    public GameManager.mode getMode()
    {
        return _mode;
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

