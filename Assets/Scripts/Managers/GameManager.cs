using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _gameOver = false;

    void Update()
    {
        bool restart = Input.GetKeyDown(KeyCode.R);
        bool escape = Input.GetKeyDown(KeyCode.Escape);
        if (_gameOver && restart)
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single); //current game scene
        }
        if (escape)
        {
            Application.Quit();
        }
    }

    public void endGame()
    {
        _gameOver = true;
    }
}
