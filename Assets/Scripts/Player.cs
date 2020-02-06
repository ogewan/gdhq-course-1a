using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject arsenal;
    [SerializeField]
    private LaserArsenal armory;
    public GameObject shield;
    public GameObject mediumShield;
    public GameObject weakShield;
    public GameObject[] engines;
    public GameObject thruster;
    public GameObject boostThruster;
    public GameObject rotateAim;
    public GameObject enemyGroup;

    public AudioClip laserFire;
    public AudioClip laserFail;
    public AudioClip powerUpSound;

    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _thrusterFuel = 100f;
    [SerializeField]
    private float _drainRate = 0.3f;
    [SerializeField]
    private float _drainReduction = 0.95f;
    [SerializeField]
    private float _rechargeRate = 8f;
    [SerializeField]
    private float _drainAccumulation = 0f;
    [SerializeField]
    private float _boostedSpeed = 8.5f;
    [SerializeField]
    private bool _overHeat = false;
    [SerializeField]
    private float _laserOffset = 0.8f;
    [SerializeField]
    private float _fireRate = 0.5f;
    [SerializeField]
    private float _tripleFR = 0.05f;
    [SerializeField]
    private float _rotateFR = 0.15f;
    [SerializeField]
    private float _homingFR = 0.3f;
    [SerializeField]
    private float _glitchFR = 0.5f;
    [SerializeField]
    private float _fireCooldown = 0f;
    [SerializeField]
    private int _health = 3;
    [SerializeField]
    private int _shield = 0;
    [SerializeField]
    private int _ammo = 15;
    [SerializeField]
    private float _powerUpDuration = 5f;
    [SerializeField]
    private InstanceCounter _powerUpStatus = new InstanceCounter(0);

    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private BoundManager _boundManager;
    private IEnumerator _powerUpTimer;
    private UIManager _uIManager;
    private AudioSource _audioPlayer;

    [System.Serializable]
    public struct InstanceCounter
    {
        public int _tripleShot;
        public int _rotateShot;
        public int _homingShot;
        public int _glitchShot;

        public InstanceCounter(int init)
        {
            this._tripleShot = 0;
            this._rotateShot = 0;
            this._homingShot = 0;
            this._glitchShot = 0;
        }
    }

    void Start()
    {
        armory = arsenal.GetComponent<LaserArsenal>();
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = registerManager<SpawnManager>("SpawnManager");
        _uIManager = registerManager<UIManager>("Canvas");
        _gameManager = registerManager<GameManager>("GameManager");
        _boundManager = registerManager<BoundManager>("BoundManager");
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
        bool hitLaser = laser.tag == "EnemyLaser" || laser.tag == "EnemySuperLaser";
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
        bool boostMode = Input.GetKey(KeyCode.LeftShift) && !_overHeat;
        thrusterControl();
        float speed = boostMode ? _boostedSpeed : _speed;
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void thrusterControl()
    {
        bool boostMode = Input.GetKey(KeyCode.LeftShift) && !_overHeat;
        if (boostMode && !boostThruster.activeSelf)
        {
            boostThruster.SetActive(true);
            thruster.SetActive(false);
        }
        else if (!boostMode && boostThruster.activeSelf)
        {
            boostThruster.SetActive(false);
            thruster.SetActive(true);
        }

        if (boostMode)
        {
            activeThruster();
        } else
        {
            inactiveThruster();
        }
        _uIManager.updateThruster(_thrusterFuel);
    }

    void inactiveThruster()
    {
        if (_drainAccumulation > 0)
        {
            _drainAccumulation -= _drainRate * Time.deltaTime * _drainReduction;
        }
        else
        {
            _drainAccumulation = 0;
        }

        if (_thrusterFuel < 100)
        {
            _thrusterFuel += _rechargeRate * Time.deltaTime;
        }
        else
        {
            _overHeat = false;
            _thrusterFuel = 100;
        }
    }

    void activeThruster()
    {
        _drainAccumulation += _drainRate * Time.deltaTime;
        if (_thrusterFuel > 0)
        {
            _thrusterFuel -= _drainAccumulation;
        }
        else
        {
            _overHeat = true;
        }
    }

    void Shoot()
    {
        bool tripleShotActive = _powerUpStatus._tripleShot > 0;
        bool rotateShotActive = _powerUpStatus._rotateShot > 0;
        bool homingShotActive = _powerUpStatus._homingShot > 0;
        bool glitchShotActive = _powerUpStatus._glitchShot > 0;
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);
        bool canFire = Time.time > _fireCooldown;
        bool superAmmo =  tripleShotActive || rotateShotActive || homingShotActive || glitchShotActive;
        bool avaliableAmmo = _ammo > 0 || superAmmo;
        if (spacePressed && canFire) 
        {
            if (avaliableAmmo)
            {
                Fire();
                if (!superAmmo)
                {
                    _ammo--;
                }
            }
            else
            {
                Debug.Log("OUT OF AMMO");
                _audioPlayer.clip = laserFail;
                _audioPlayer.Play();
            }
        }
        
    }

    void Fire()
    {
        Vector3 offsetPosition = new Vector3(0, _laserOffset, 0);
        bool tripleShotActive = _powerUpStatus._tripleShot > 0;
        bool rotateShotActive = _powerUpStatus._rotateShot > 0;
        bool homingShotActive = _powerUpStatus._homingShot > 0;
        bool glitchShotActive = _powerUpStatus._glitchShot > 0;

        GameObject readyShot = glitchShotActive ? armory.glitchShotPrefab : homingShotActive ? armory.homingShotPrefab : (rotateShotActive ? armory.rotateShotPrefab : (tripleShotActive ? armory.tripleShotPrefab : armory.normalPrefab));
        GameObject laser = _boundManager.bsInsantiate(readyShot, transform.position, (rotateShotActive && !homingShotActive) ? rotateAim.transform.rotation : Quaternion.identity);
        float modifierFireRate = glitchShotActive ? _glitchFR : (homingShotActive ? _homingFR : (rotateShotActive ? _rotateFR : (tripleShotActive ? _tripleFR : 0f)));

        if (laser && homingShotActive || glitchShotActive)
        {
            chooseTarget(laser);
        }

        _audioPlayer.clip = laserFire;
        _audioPlayer.Play();
        _fireCooldown = Time.time + _fireRate + modifierFireRate;
    }

    void chooseTarget(GameObject laser)
    {
        int enemyCount = enemyGroup.transform.childCount;
        int target = Random.Range(0, enemyCount);
        int iter = 0;
        HomingLaser laserCPU = laser.GetComponent<HomingLaser>();

        foreach (Transform child in enemyGroup.transform)
        {
            if (iter == target)
            {
                laserCPU.setTarget(child);
                break;
            }
            iter++;
        }
    }

    void handlePowerUpBool(Powerup.type type, int instances)
    {
        switch (type)
        {
            case Powerup.type.triple:
                _powerUpStatus._tripleShot += instances;
                break;
            case Powerup.type.rotate:
                _powerUpStatus._rotateShot += instances;
                break;
            case Powerup.type.homing:
                _powerUpStatus._homingShot += instances;
                break;
            case Powerup.type.glitch:
                _powerUpStatus._glitchShot += instances;
                break;
            case Powerup.type.repair:
                if (_health < 3 && instances > 0)
                {
                    _health++;
                    _uIManager.updateLives(_health);
                    restoreEngine();
                }
                break;
            case Powerup.type.shield:
                if (instances > 0)
                {
                    _shield = 3;
                    handleShield();
                }
                break;
            case Powerup.type.ammo:
                if (instances > 0)
                {
                    _ammo = 15;
                }
                break;
            default:
                break;
        }
        bool rotateActive = _powerUpStatus._rotateShot > 0;
        bool homingActive = _powerUpStatus._homingShot > 0;
        bool glitchActive = _powerUpStatus._glitchShot > 0;
        rotateAim.SetActive(rotateActive && !(homingActive || glitchActive));
    }

    void handleShield()
    {
        shield.SetActive(_shield == 3);
        mediumShield.SetActive(_shield == 2);
        weakShield.SetActive(_shield == 1);
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

    void restoreEngine()
    {
        GameObject damagedEngine;
        int activeID = Random.Range(0, engines.Length);
        damagedEngine = engines[activeID];
        if (!damagedEngine.activeInHierarchy)
        {
            engines[activeID == 1 ? 0 : 1].SetActive(false);
        }
        else
        {
            damagedEngine.SetActive(false);
        }
    }

    IEnumerator powerUpRoutine(Powerup.type type)
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
        if (_shield > 0)
        {
            _shield--;
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

    public void activatePowerUp(Powerup.type type)
    {
        Vector3 camera = new Vector3(0, 0, -10);
        AudioSource.PlayClipAtPoint(powerUpSound, camera);
        _powerUpTimer = powerUpRoutine(type);
        StartCoroutine(_powerUpTimer);
    }
}
