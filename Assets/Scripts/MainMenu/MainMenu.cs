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

    public void loadGame()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(1); //main game scene
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _newManager = registerManager<GameManager>("GameManager");
        if (gameObject.name == "old" && _newManager)
        {
            _newManager.setMode(_mode);
            Destroy(gameObject);
        }
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

