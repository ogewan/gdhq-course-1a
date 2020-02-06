using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _gameOver = false;
    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private int _highScore;
    private UIManager _uIManager;
    private GameManager _newManager;


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Awake()
    {
        _uIManager = registerManager<UIManager>("Canvas");
    }

    void Update()
    {
        scoreCheck();
        handleControlKeys();
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

    public void setHighScore(int score)
    {
        _highScore = score;
        _uIManager.updateHighScore(_highScore);
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
        if (gameObject.name == "old" && _newManager)
        {
            _newManager.setHighScore(_highScore);
            //Destroy(gameObject);
        }
    }

    void handleControlKeys()
    {
        bool restart = Input.GetKeyDown(KeyCode.R);
        bool escape = Input.GetKeyDown(KeyCode.Escape);
        if (_gameOver && restart)
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene(1, LoadSceneMode.Single); //current game scene
            gameObject.name = "old";
        }
        if (escape)
        {
            Application.Quit();
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
