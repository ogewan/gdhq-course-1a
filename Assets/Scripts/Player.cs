using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject laserPrefab;
    public GameObject tripleShotPrefab;
    public GameObject shield;
    public GameObject[] engines;
    public AudioClip laserFire;
    public AudioClip powerUpSound;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private Vector2 _xBound = new Vector2(-11.5f, 10.5f);
    [SerializeField]
    private Vector2 _yBound = new Vector2(-6f, 7f);
    [SerializeField]
    private float _laserOffset = 0.8f;
    [SerializeField]
    private float _fireRate = 0.5f;
    [SerializeField]
    private float _fireCooldown = 0f;
    [SerializeField]
    private int _health = 3;
    [SerializeField]
    private int _tripleShotInstances = 0;
    [SerializeField]
    private int _speedBoostInstances = 0;
    [SerializeField]
    private float _boostedSpeed = 8.5f;
    [SerializeField]
    private int _shieldInstances = 0;
    [SerializeField]
    private float _powerUpDuration = 5f;

    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private IEnumerator _powerUpTimer;
    private UIManager _uIManager;
    private AudioSource _audioPlayer;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = registerManager<SpawnManager>("SpawnManager");
        _uIManager = registerManager<UIManager>("Canvas");
        _gameManager = registerManager<GameManager>("GameManager");
        _uIManager.updateLives(_health);
        _audioPlayer = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        Movement();
        Shoot();
    }

    void OnTriggerEnter2D(Collider2D laser)
    {
        bool hitLaser = laser.tag == "EnemyLaser";
        if (hitLaser)
        {
            Destroy(laser.gameObject);
            Damage();
        }
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float speed = _speedBoostInstances > 0 ? _boostedSpeed : _speed;
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * speed * Time.deltaTime);
        //bounds: -11.5 - 10.5 (x), -6 - 7 (y)
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        float xMin = _xBound[0];
        float xMax = _xBound[1];
        float yMin = _yBound[0];
        float yMax = _yBound[1];

        if (xPos <= xMin || xPos >= xMax)
        {
            transform.position = new Vector3(xPos <= xMin ? xMax : xMin, yPos, 0);
        }

        if (yPos <= yMin || yPos >= yMax)
        {
            transform.position = new Vector3(xPos, yPos <= yMin ? yMax : yMin, 0);
        }
    }

    void Shoot()
    {
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);
        bool canFire = Time.time > _fireCooldown;
        if (spacePressed && canFire)
        {
            FireLaser();
        }
    }

    void FireLaser()
    {
        Vector3 offsetPosition = new Vector3(0, _laserOffset, 0);
        if (_tripleShotInstances > 0)
        {
            Instantiate(tripleShotPrefab, transform.position, Quaternion.identity);
        } else
        {
            Instantiate(laserPrefab, transform.position + offsetPosition, Quaternion.identity);
        }
        _audioPlayer.clip = laserFire;
        _audioPlayer.Play();
        _fireCooldown = Time.time + _fireRate;
    }

    void handlePowerUpBool(string type, int instances)
    {
        switch (type)
        {
            case "tripleShot":
                _tripleShotInstances += instances;
                break;
            case "speedBoost":
                _speedBoostInstances += instances;
                break;
            case "shield":
                _shieldInstances += instances;
                handleShield();
                break;
            default:
                break;
        }
    }

    void handleShield()
    {
        if (_shieldInstances > 0)
        {
            shield.SetActive(true);
        }
        else if (_shieldInstances == 0)
        {
            shield.SetActive(false);
        }
        else {
            _shieldInstances = 0;
        }
    }
    
    void playerDies()
    {
        _uIManager.gameOver();
        _spawnManager.playerDeath();
        _gameManager.endGame();
        Instantiate(_explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void damagedEngine()
    {
        GameObject activeEngine;
        int activeID = Random.Range(0, engines.Length);
        activeEngine = engines[activeID];
        if (activeEngine.activeInHierarchy)
        {
            engines[activeID == 1 ? 0 : 1].SetActive(true);
        }
        else
        {
            activeEngine.SetActive(true);
        }
    }

    IEnumerator powerUpRoutine(string type)
    {
        handlePowerUpBool(type, 1);
        yield return new WaitForSeconds(_powerUpDuration);
        handlePowerUpBool(type, -1);
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

    public void Damage()
    {
        if (_shieldInstances > 0)
        {
            _shieldInstances = 0;
            handleShield();
            return;
        }
        _health--;
        _uIManager.updateLives(_health);
        if (_health <= 0)
        {
            playerDies();
        }
        damagedEngine();
    }

    public void activatePowerUp(string type)
    {
        AudioSource.PlayClipAtPoint(powerUpSound, transform.position);
        _powerUpTimer = powerUpRoutine(type);
        StartCoroutine(_powerUpTimer);
    }

    public void addScore(int score)
    {
        _score += score;
        _uIManager.updateScore(_score);
    }

}
