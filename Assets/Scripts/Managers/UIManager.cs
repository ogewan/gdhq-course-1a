using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Sprite[] _lifeSprites;

    void Start()
    {
        updateScore(0);
    }

    IEnumerator gameOverFlicker()
    {
        while (true)
        {
            float timeA = Random.Range(0f, 1f);
            float timeB = Random.Range(0f, 1f);
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(timeA);
            _gameOverText.text = "";
            yield return new WaitForSeconds(timeB);
        }
    }

    public void updateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void updateLives(int currentLives)
    {
        _livesImage.sprite = _lifeSprites[currentLives];
    }

    public void gameOver()
    {
        _gameOverText.enabled = true;
        _restartText.enabled = true;
        StartCoroutine(gameOverFlicker());
    }
}
