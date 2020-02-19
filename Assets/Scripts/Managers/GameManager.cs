using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum mode { easy, hard, endless };
    public Registry _managers;
    public int scoreMult = 1;
    [SerializeField]
    private bool _gameOver = false;
    [SerializeField]
    private bool _pause = false;
    [SerializeField]
    private bool _unlockedEndless = false;
    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private int _highScore;
    private mode _mode;
    private UIManager _uIManager;
    private StoryManager _storyManager;
    private SpawnManager _spawnManager;
    private MainMenu _mainMenu;

    void Awake()
    {
        _uIManager = _managers.uiManager;
        _storyManager = _managers.storyManager;
        _spawnManager = _managers.spawnManager;

        GameManager oldManager = registerManager<GameManager>("old");
        if (oldManager)
        {
            setHighScore(oldManager.getHighScore());
            setMode(oldManager.getMode());
            if (oldManager.isEndlessModeUnlocked()) unlockEndlessMode();
            Destroy(oldManager.gameObject);
        }
        MainMenu mainMenu = registerManager<MainMenu>("Canvas");
        if (mainMenu)
        {
            setMode(mainMenu.getMode());
            Destroy(mainMenu.gameObject);
        }
    }

    void Start()
    {
        if (_mode == mode.easy)
        {
            scoreMult = 3;
        }
    }

    void Update()
    {
        scoreCheck();
        handleControlKeys();
    }

    void scoreCheck()
    {
        if (_score > _highScore)
        {
            setHighScore(_score);
        }
    }

    void ascend(int sceneNumber)
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single); //current game scene
        gameObject.name = "old";
    }

    void handleControlKeys()
    {
        bool restart = Input.GetKeyDown(KeyCode.R);
        bool mainmenu = Input.GetKeyDown(KeyCode.M);
        bool pause = Input.GetKeyDown(KeyCode.P);
        bool escape = Input.GetKeyDown(KeyCode.Escape);
        if (_gameOver)
        {
            if (restart)
            {
                ascend(1);
            }
        } else if (pause)
        {
            _pause = !_pause;
            _uIManager.pauseToggle();
        }
        if (mainmenu)
        {
            ascend(0);
        }

        if (escape)
        {
            Application.Quit();
        }
    }

    public bool isPaused()
    {
        return _pause;
    }

    public void endGame()
    {
        _gameOver = true;
    }

    public void addScore(int score)
    {
        _score += score * scoreMult;
        _uIManager.updateScore(_score);
        _storyManager.scoreCheck(_score);
    }

    public void addKill(Enemy.type type, int kill=1)
    {
        switch(type)
        {
            case Enemy.type.Super:
                _uIManager.yellowToken(kill);
                break;
            case Enemy.type.Hyper:
                _uIManager.redToken(kill);
                break;
            case Enemy.type.Ultra:
                _uIManager.greenToken(kill);
                break;
        }
        _storyManager.addKill(type);
    }

    public void setHighScore(int score)
    {
        _highScore = score;
        _uIManager.updateHighScore(_highScore);
    }

    public int getHighScore()
    {
        return _highScore;
    }

    public mode getMode()
    {
        return _mode;
    }

    public void setMode(mode gameMode)
    {
        _mode = gameMode;
    }

    public bool isEndlessModeUnlocked()
    {
        return _unlockedEndless;
    }

    public bool firstSpawn(Enemy.type type)
    {
        switch(type)
        {
            case Enemy.type.Super:
                _uIManager.toggleYellow();
                break;
            case Enemy.type.Hyper:
                _uIManager.toggleRed();
                break;
            case Enemy.type.Ultra:
                _uIManager.toggleGreen();
                break;
        }
        return _storyManager.firstSpawn(type);
    }

    public void unlockEndlessMode()
    {
        _unlockedEndless = true;
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
