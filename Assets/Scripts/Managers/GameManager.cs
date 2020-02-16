using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum mode { easy, hard, endless };
    public Registry _managers;
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
    private GameManager _newManager;
    private StoryManager _storyManager;
    private MainMenu _mainMenu;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        _uIManager = _managers.uiManager;
        _storyManager = _managers.storyManager;
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _newManager = registerManager<GameManager>("GameManager");
        _mainMenu = registerManager<MainMenu>("Canvas");
        if (gameObject.name == "old" && _newManager)
        {
            _newManager.setHighScore(_highScore);
            _newManager.setMode(_mode);
            //Destroy(gameObject);
        }
        if (_mainMenu && _unlockedEndless)
        {
            _mainMenu.endlessButton.SetActive(true);
            Destroy(gameObject);
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
        _score += score;
        _uIManager.updateScore(_score);
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
    }

    public void setHighScore(int score)
    {
        _highScore = score;
        _uIManager.updateHighScore(_highScore);
    }

    public void setMode(mode gameMode)
    {
        _mode = gameMode;
    }

    public bool firstSpawn(Enemy.type type)
    {
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
