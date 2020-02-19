using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool _immortal = false;
    public enum shotType { normal, triple, rotate, homing, glitch };
    public LaserArsenal armory;
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
    private float dmgShake = 1f;
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _boostedSpeed = 8.5f;
    [SerializeField]
    private BoostThrusterStats _thrusterStatus = new BoostThrusterStats(0);
    [SerializeField]
    private float _laserOffset = 0.8f;
    [SerializeField]
    private FireRate _fireRate = new FireRate(0);
    [SerializeField]
    private PlayerStats _stats = new PlayerStats(0);
    [SerializeField]
    private float _powerUpDuration = 5f;
    [SerializeField]
    private InstanceCounter _powerUpStatus = new InstanceCounter(0);

    [SerializeField]
    private Registry _managers;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private BoundManager _boundManager;
    private IEnumerator _powerUpTimer;
    private UIManager _uIManager;
    private AudioSource _audioPlayer;
    private Animator _animator;
    private Pausible _pausible;

    [System.Serializable]
    public struct InstanceCounter
    {
        public int tripleShot;
        public int rotateShot;
        public int homingShot;
        public int glitchShot;

        public InstanceCounter(int init)
        {
            this.tripleShot = 0;
            this.rotateShot = 0;
            this.homingShot = 0;
            this.glitchShot = 0;
        }
    }

    [System.Serializable]
    public struct BoostThrusterStats
    {
        public float thrusterFuel;
        public float drainRate;
        public float drainReduction;
        public float rechargeRate;
        public float drainAccumulation;
        public bool overHeat;

        public BoostThrusterStats(int init)
        {
            this.thrusterFuel = 100f;
            this.drainRate = 0.3f;
            this.drainReduction = 0.95f;
            this.rechargeRate = 8f;
            this.drainAccumulation = 0f;
            this.overHeat = false;
        }
    }

    [System.Serializable]
    public struct FireRate
    {
        public float normal;
        public float triple;
        public float rotate;
        public float homing;
        public float glitch;
        public float cooldown;

        public FireRate(int init)
        {
            this.normal = 0.5f;
            this.triple = 0.05f;
            this.rotate = 0.15f;
            this.homing = 0.3f;
            this.glitch = 0.5f;
            this.cooldown = 0f;
        }
    }

    [System.Serializable]
    public struct PlayerStats
    {
        public int health;
        public int shield;
        public int ammo;
        
        public PlayerStats(int init)
        {
            this.health = 3;
            this.shield = 0;
            this.ammo = 15;
        }
    }

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = _managers.spawnManager;
        _uIManager = _managers.uiManager;
        _gameManager = _managers.gameManager;
        _boundManager = _managers.boundManager;
        _pausible = GetComponent<Pausible>();
        _uIManager.updateLives(_stats.health);
        _audioPlayer = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        updateAmmo();
    }

    void Update()
    {
        if (_pausible && _pausible.isPaused()) return;
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
        bool boostMode = Input.GetKey(KeyCode.LeftShift) && !_thrusterStatus.overHeat;
        thrusterControl();
        float speed = boostMode ? _boostedSpeed : _speed;
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * speed * Time.deltaTime);
        _animator.SetBool("turningLeft", horizontalInput < 0);
        _animator.SetBool("turningRight", horizontalInput > 0);
    }

    void thrusterControl()
    {
        bool boostMode = Input.GetKey(KeyCode.LeftShift) && !_thrusterStatus.overHeat;
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
        _uIManager.updateThruster(_thrusterStatus.thrusterFuel);
    }

    void inactiveThruster()
    {
        float drainRate = _thrusterStatus.drainRate;
        float drainReduction = _thrusterStatus.drainReduction;
        float rechargeRate = _thrusterStatus.rechargeRate;
        if (_thrusterStatus.drainAccumulation > 0)
        {
            _thrusterStatus.drainAccumulation -= drainRate * Time.deltaTime * drainReduction;
        }
        else
        {
            _thrusterStatus.drainAccumulation = 0;
        }

        if (_thrusterStatus.thrusterFuel < 100)
        {
            _thrusterStatus.thrusterFuel += rechargeRate * Time.deltaTime;
        }
        else
        {
            _thrusterStatus.overHeat = false;
            _thrusterStatus.thrusterFuel = 100;
        }
    }

    void activeThruster()
    {
        float drainRate = _thrusterStatus.drainRate;
        _thrusterStatus.drainAccumulation += drainRate * Time.deltaTime;
        if (_thrusterStatus.thrusterFuel > 0)
        {
            _thrusterStatus.thrusterFuel -= _thrusterStatus.drainAccumulation;
        }
        else
        {
            _thrusterStatus.overHeat = true;
        }
    }

    void updateAmmo()
    {
        bool tsActive = _powerUpStatus.tripleShot > 0;
        bool rsActive = _powerUpStatus.rotateShot > 0;
        bool hsActive = _powerUpStatus.homingShot > 0;
        bool gsActive = _powerUpStatus.glitchShot > 0;
        _uIManager.updateAmmo(_stats.ammo, gsActive ? shotType.glitch : hsActive ? shotType.homing : rsActive ? shotType.rotate : tsActive ? shotType.triple : shotType.normal);
    }

    void Shoot()
    {
        bool tsActive = _powerUpStatus.tripleShot > 0;
        bool rsActive = _powerUpStatus.rotateShot > 0;
        bool hsActive = _powerUpStatus.homingShot > 0;
        bool gsActive = _powerUpStatus.glitchShot > 0;
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);
        bool canFire = Time.time > _fireRate.cooldown;
        bool superAmmo =  tsActive || rsActive || hsActive || gsActive;
        bool avaliableAmmo = _stats.ammo > 0 || superAmmo;

        if (spacePressed && canFire) 
        {
            if (avaliableAmmo)
            {
                Fire();
                if (!superAmmo)
                {
                    _stats.ammo--;
                }
                updateAmmo();
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
        bool tsActive = _powerUpStatus.tripleShot > 0;
        bool rsActive = _powerUpStatus.rotateShot > 0;
        bool hsActive = _powerUpStatus.homingShot > 0;
        bool gsActive = _powerUpStatus.glitchShot > 0;
        GameObject nmPfb = armory.normalPrefab;
        GameObject tsPfb = armory.tripleShotPrefab;
        GameObject rsPfb = armory.rotateShotPrefab;
        GameObject hsPfb = armory.homingShotPrefab;
        GameObject gsPfb = armory.glitchShotPrefab;
        float fireRate = _fireRate.normal;
        float tripleFR = _fireRate.triple;
        float rotateFR = _fireRate.rotate;
        float homingFR = _fireRate.homing;
        float glitchFR = _fireRate.glitch;

        GameObject readyShot = gsActive ? gsPfb : hsActive ? hsPfb : (rsActive ? rsPfb : (tsActive ? tsPfb : nmPfb));
        GameObject laser = _boundManager.bsInstantiate(readyShot, transform.position, (rsActive && !hsActive) ? rotateAim.transform.rotation : Quaternion.identity);
        float modifierFireRate = gsActive ? glitchFR : (hsActive ? homingFR : (rsActive ? rotateFR : (tsActive ? tripleFR : 0f)));

        if (laser && hsActive || gsActive)
        {
            chooseTarget(laser);
        }

        _audioPlayer.clip = laserFire;
        _audioPlayer.Play();
        _fireRate.cooldown = Time.time + fireRate + modifierFireRate;
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
                _powerUpStatus.tripleShot += instances;
                break;
            case Powerup.type.rotate:
                _powerUpStatus.rotateShot += instances;
                break;
            case Powerup.type.homing:
                _powerUpStatus.homingShot += instances;
                break;
            case Powerup.type.glitch:
                _powerUpStatus.glitchShot += instances;
                break;
            case Powerup.type.repair:
                if (_stats.health < 3 && instances > 0)
                {
                    _stats.health++;
                    _uIManager.updateLives(_stats.health);
                    restoreEngine();
                }
                break;
            case Powerup.type.shield:
                if (instances > 0)
                {
                    _stats.shield = 3;
                    handleShield();
                }
                break;
            case Powerup.type.ammo:
                if (instances > 0)
                {
                    _stats.ammo = 15;
                }
                break;
            case Powerup.type.star:
                _uIManager.toggleStar();
                break;
            default:
                break;
        }
        updateAmmo();
        bool rotateActive = _powerUpStatus.rotateShot > 0;
        bool homingActive = _powerUpStatus.homingShot > 0;
        bool glitchActive = _powerUpStatus.glitchShot > 0;
        rotateAim.SetActive(rotateActive && !(homingActive || glitchActive));
    }

    void handleShield()
    {
        float _shield = _stats.shield;
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

    /*T registerManager<T>(string name)
    {
        GameObject manager = GameObject.Find(name);
        if (manager)
        {
            return manager.GetComponent<T>();
        }
        Debug.LogError(name + " not found");
        return default(T);
    }
    */

    public void Damage()
    {
        if (_immortal) return;
        if (_stats.shield > 0)
        {
            _stats.shield--;
            handleShield();
            return;
        }
        _stats.health--;
        _uIManager.shakeScreen(dmgShake);
        _uIManager.updateLives(_stats.health);
        if (_stats.health <= 0)
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
