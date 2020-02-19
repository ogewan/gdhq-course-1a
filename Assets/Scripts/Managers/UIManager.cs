using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _highScoreText;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Sprite[] _lifeSprites;
    [SerializeField]
    private Slider _thrusterBar;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private KillToken _killIcon;
    [SerializeField]
    private Image _starIcon;
    [SerializeField]
    private Text _pauseText;
    [SerializeField]
    private Transform mainCamera;
    [SerializeField]
    private CameraShake shakeControl = new CameraShake(0);
    private Vector3 cameraInitPos;

    [System.Serializable]
    public struct CameraShake
    {
        public float shakeDuration;
        public float shakeDampen;
        public float shakeMagnitude;
        public bool shakeActive;

        public CameraShake(int init)
        {
            this.shakeDuration = 0f;
            this.shakeDampen = 1f;
            this.shakeMagnitude = 0.7f;
            this.shakeActive = false;
        }
    }

    void Start()
    {
        updateScore(0);
        cameraInitPos = mainCamera.position;
    }

    void Update()
    {
        handleCameraShake();
    }

    void handleCameraShake()
    {
        float shakeDuration = shakeControl.shakeDuration;
        float shakeDampen = shakeControl.shakeDampen;
        float shakeMagnitude = shakeControl.shakeMagnitude;
        bool shakeActive = shakeControl.shakeActive;

        if (shakeDuration > 0)
        {
            shakeControl.shakeActive = true;
            Vector3 randomInsideUnitCircle3D = Vector3.Scale(Random.insideUnitSphere, new Vector3(1, 1, 0));
            mainCamera.position = cameraInitPos + randomInsideUnitCircle3D * shakeMagnitude;
            shakeControl.shakeDuration -= Time.deltaTime * shakeDampen;
        }
        else if (shakeActive)
        {
            shakeControl.shakeActive = false;
            shakeDuration = 0f;
            mainCamera.position = cameraInitPos;
        }
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

    public void shakeScreen(float shakeTime)
    {
        shakeControl.shakeDuration = shakeTime;
    }

    public void updateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void updateHighScore(int score)
    {
        _highScoreText.text = "High Score: " + score;
    }

    public void updateLives(int currentLives)
    {
        _livesImage.sprite = _lifeSprites[currentLives];
    }

    public void updateThruster(float fuel)
    {
        _thrusterBar.value = fuel;
    }

    public void updateAmmo(int ammo, Player.shotType shot)
    {
        //Debug.Log("ammo " + ammo + " shot " + shot);
        switch (shot)
        {
            case Player.shotType.glitch:
                _ammoText.text = "Ammo: <color=red>G</color><color=orange>l</color><color=yellow>i</color><color=green>t</color><color=blue>c</color><color=purple>h</color> ";
                //_ammoText.text = "Ammo: 4a";
                break;
            case Player.shotType.homing:
                _ammoText.text = "Ammo: <color=purple>Homing</color> ";
                //_ammoText.text = "Ammo: 3a";
                break;
            case Player.shotType.rotate:
                _ammoText.text = "Ammo: <color=yellow>Rotate</color> ";
                //_ammoText.text = "Ammo: 2a";
                break;
            case Player.shotType.triple:
                _ammoText.text = "Ammo: <color=red>Triple</color> ";
                //_ammoText.text = "Ammo: 1a";
                break;
            default:
                _ammoText.text = "Ammo: " + ammo;
                break;
        }
        //Debug.Log(_ammoText.text);
    }

    public void yellowToken(int count)
    {
        _killIcon.yellowToken(count);
    }

    public void redToken(int count)
    {
        _killIcon.redToken(count);
    }

    public void greenToken(int count)
    {
        _killIcon.greenToken(count);
    }

    public void toggleStar()
    {
        bool status = _starIcon.gameObject.activeInHierarchy;
        _starIcon.gameObject.SetActive(!status);
    }

    public void toggleYellow()
    {
        _killIcon.toggleYellow();
    }

    public void toggleRed()
    {
        _killIcon.toggleRed();
    }

    public void toggleGreen()
    {
        _killIcon.toggleGreen();
    }

    public void pauseToggle()
    {
        bool paused = _pauseText.gameObject.activeInHierarchy;
        _pauseText.gameObject.SetActive(!paused);
    }

    public void gameOver()
    {
        _gameOverText.enabled = true;
        _restartText.enabled = true;
        StartCoroutine(gameOverFlicker());
    }
}
