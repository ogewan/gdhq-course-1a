using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum type { Normal, Super, Hyper, Ultra, Tohou, Tohou_gun, White, Bomber, None };

    [SerializeField]
    private type enemyType;
    [SerializeField]
    private bool _helpMenu = false;
    [SerializeField]
    private Transform _rotateAim;
    [SerializeField]
    private float _portalOffset = 4f;
    [SerializeField]
    private int _portalLimit = 30;
    [SerializeField]
    private GameObject _portalContainer;
    [SerializeField]
    private GameObject _powerupContainer;
    [SerializeField]
    private GameObject[] _gunners = new GameObject[3];
    [SerializeField]
    private float _regenerateTime = 4f;
    [SerializeField]
    private bool _activated = true;
    [SerializeField]
    private GameObject _indicator;
    [SerializeField]
    private GameObject _newShields;
    [SerializeField]
    private int _health = 1;
    public LaserArsenal armory;
    public AudioClip laserFire;
    public AudioClip explodeSound;
    [SerializeField]
    private Vector2 fireTimeRange = new Vector2(3f, 5f);
    [SerializeField]
    private float _enemySpeed = 4f;
    [SerializeField]
    private int _scoreValue = 10;
    public Transform player;
    [SerializeField]
    private bool _destroyed = false;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private AudioSource _audioPlayer;
    [SerializeField]
    private Collider2D _selfCollider;
    [SerializeField]
    private SpriteRenderer _sprite;
    [SerializeField]
    private float _scaler = 1f;
    public Registry managers;
    public Spawnable rewards;
    public float rewardChance = 1f;
    private Randomizer _projectile;
    private BoundManager _boundManager;
    private GameManager _gameManager;
    private StoryManager _storyManager;
    private Pausible _pausible;
    private Asteroid _asteroid;

    void Start()
    {
        if (_helpMenu) return;
        _gameManager = managers.gameManager;
        _boundManager = managers.boundManager;
        _storyManager = managers.storyManager;
        _pausible = GetComponent<Pausible>();
        _asteroid = GetComponent<Asteroid>();
        EnemySetup();
        _audioPlayer.clip = laserFire;
        StartCoroutine(fireRoutine());
        _sprite = GetComponent<SpriteRenderer>();
    }

    void setGunners()
    {
        foreach (GameObject gun in _gunners)
        {
            SpawnManager.setPausible(gun, managers);
            gun.GetComponent<Enemy>().managers = managers;
            gun.GetComponent<Enemy>().player = player.transform;
            gun.GetComponent<Enemy>().setPortalContainer(_portalContainer);
            gun.GetComponent<Enemy>().setPowerupContainer(_powerupContainer);
            gun.SetActive(true);
        }
    }

    void Update()
    {
        if (_pausible && _pausible.isPaused() || _helpMenu) return;
        Movement();
        if (_gameManager.getMode() == GameManager.mode.wave && enemyType == Enemy.type.Tohou && !_activated && transform.position.y <= 3.25)
        {
            _activated = true;
            _enemySpeed = 0;
            setGunners();
            _asteroid._rotSpeed = -40f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool hitPlayer = other.tag == "Player";
        bool hitLaser = other.tag == "Laser";
        bool hitGlitch = other.tag == "Glitch";
        bool hitPortal = other.tag == "Portal";
        bool isTohouGun = enemyType == type.Tohou_gun;

        if (hitPlayer || hitLaser || hitGlitch)
        {
            if (hitPlayer)
            {
                Player player = other.transform.GetComponent<Player>();
                if (player)
                {
                    player.Damage();
                }
            }
            else if (hitLaser && !_destroyed)
            {
                Destroy(other.gameObject);
            }
            Damage();
        }

        if (hitPortal && !isTohouGun)
        {
            int sign = randomSign();
            float rotater = (transform.eulerAngles.z == 0) ? 90 * sign : transform.eulerAngles.z * -1;
            transform.Rotate(Vector3.forward, rotater);
            other.gameObject.GetComponent<Portal>().closePortal();
        }
    }

    void Damage()
    {
        _health--;
        if (_health <= 0 && !_destroyed)
        {
            if (enemyType == type.Tohou_gun && _activated)
            {
                StartCoroutine(regenerate());
                _indicator.SetActive(false);
                _activated = false;
                reward();
            }
            else
            {
                EnemyDies();
            }
        }
        else if (_health == 1 && enemyType == type.White)
        {
            StartCoroutine(regenerate());
            _newShields.SetActive(false);
        }
    }

    void EnemyDies()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        _destroyed = true;
        _gameManager.addScore(_scoreValue);
        _gameManager.addKill(enemyType);
        switch (enemyType)
        {
            case Enemy.type.Tohou:
                _gameManager.unlockEndlessMode();
                break;
            case Enemy.type.Ultra:
                _sprite.color = Color.white;
                break;
        }
        _animator.SetTrigger("onEnemyDeath");
        transform.localScale = transform.localScale * _scaler;
        _audioPlayer.clip = explodeSound;
        _audioPlayer.Play();
        _enemySpeed = 0;
        reward();
        Destroy(gameObject, 2.8f);
    }

    void Movement()
    {
        if (enemyType == type.Bomber)
        {
            HomingLaser.homing(transform, player, -50f);
        }
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
    }

    void EnemySetup()
    {
        Spawnable.Spawnlet[] pool;
        Spawnable.Spawnlet normal = new Spawnable.Spawnlet(armory.normalPrefab, 3);
        Spawnable.Spawnlet rotate = new Spawnable.Spawnlet(armory.rotateShotPrefab, 3);
        Spawnable.Spawnlet homing = new Spawnable.Spawnlet(armory.homingShotPrefab, 3);
        Spawnable.Spawnlet super = new Spawnable.Spawnlet(armory.superShotPrefab, 3);
        Spawnable.Spawnlet portal = new Spawnable.Spawnlet(armory.glitchShotPrefab);
        Spawnable.Spawnlet specialPortal = new Spawnable.Spawnlet(armory.glitchShotPrefab, 2);
        Spawnable.Spawnlet rareSuper = new Spawnable.Spawnlet(armory.superShotPrefab, 1);

        switch (enemyType)
        {
            case type.Normal:
                pool = new Spawnable.Spawnlet[1] { normal };
                _projectile = new Randomizer(pool);
                break;
            case type.Super:
                _scaler = 1.1f;
                pool = new Spawnable.Spawnlet[3] { normal, rotate, portal };
                _projectile = new Randomizer(pool);
                break;
            case type.Hyper:
                _scaler = 1.2f;
                pool = new Spawnable.Spawnlet[3] { rotate, homing, portal };
                _projectile = new Randomizer(pool);
                break;
            case type.Ultra:
                _scaler = 1.3f;
                pool = new Spawnable.Spawnlet[4] { super, rotate, homing, specialPortal };
                _projectile = new Randomizer(pool);
                break;
            case type.Tohou:
                _scaler = 3f;
                pool = new Spawnable.Spawnlet[1] { portal };
                _projectile = new Randomizer(pool);
                // Guns aren't set automatically in wave mode
                if (_gameManager.getMode() == GameManager.mode.wave)
                {
                    _activated = false;
                    _enemySpeed = 0.5f;
                    _asteroid._rotSpeed = 0f;
                } else
                {
                    setGunners();
                }
                break;
            case type.Tohou_gun:
                pool = new Spawnable.Spawnlet[4] { normal, super, rotate, homing };
                _projectile = new Randomizer(pool);
                break;
            case type.White:
                pool = new Spawnable.Spawnlet[4] { normal, rareSuper, homing, specialPortal };
                //pool = new Spawnable.Spawnlet[1] { normal };
                _projectile = new Randomizer(pool);
                break;
            case type.Bomber:
                _scaler = 2.5f;
                pool = new Spawnable.Spawnlet[0] { };
                _projectile = new Randomizer(pool);
                break;
        }
    }

    void chooseTarget(GameObject laser)
    {
        HomingLaser laserCPU = laser.GetComponent<HomingLaser>();
        if (laserCPU != null)
        {
            laserCPU.setTarget(player);
        }
    }
    
    IEnumerator fireRoutine()
    {
        while (!_destroyed && !_helpMenu)
        {
            float minTime = Mathf.Max(0, Mathf.Min(fireTimeRange[0], fireTimeRange[1]));
            float maxTime = Mathf.Max(1, Mathf.Max(fireTimeRange[0], fireTimeRange[1]));
            float spawnTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(spawnTime);
            bool isPaused = _pausible && _pausible.isPaused();
            if (!_destroyed && !isPaused && _activated)
            {
                GameObject laser = _projectile.Get();
                chooseTarget(laser);
                bool isRotateShot = armory.rotateShotPrefab.name == laser.name;
                bool isPortal = armory.glitchShotPrefab.name == laser.name;
                Vector3 lastPlayerPosition = player != null ? player.position : transform.position;
                bool isPlayerBehind = (transform.position - lastPlayerPosition).y < 0;
                //Vector3 debugPositioning = transform.position - lastPlayerPosition;
                //Debug.Log(debugPositioning);
                int flipModifier = ((isPlayerBehind && enemyType == type.White) ? 1 : 0);
                //Quaternion stdRotation = Quaternion.Euler(flipModifier * 180 * Vector3.forward);
                Vector3 eulerRot = transform.rotation.eulerAngles;
                eulerRot.z += flipModifier * 180;
                Quaternion stdRotation = Quaternion.Euler(eulerRot);
                Quaternion projectileRotation = (isRotateShot) ? _rotateAim.rotation : stdRotation;
                if (isPortal) {
                    if (_portalContainer.transform.childCount > _portalLimit) continue;
                }
                GameObject firedProjectile = _boundManager.bsInstantiate(laser, transform.position, projectileRotation);
                if (isPortal && firedProjectile)
                {
                    Vector3 eangles = firedProjectile.transform.eulerAngles;
                    firedProjectile.transform.Translate(Vector3.down * _portalOffset);
                    firedProjectile.transform.eulerAngles = new Vector3(eangles.x, eangles.y, eangles.z + 90);
                    firedProjectile.transform.parent = _portalContainer.transform;
                }
                _audioPlayer.Play();
            }
        }
    }

    IEnumerator regenerate()
    {
        yield return new WaitForSeconds(_regenerateTime);
        _health++;
        if (enemyType == type.Tohou_gun)
        {
            _indicator.SetActive(true);
            _activated = true;
        }
        else if (enemyType == type.White && !_destroyed)
        {
            _newShields.SetActive(true);
        }
    }

    public type getType()
    {
        return enemyType;
    }
    
    public int randomSign()
    {
        return Random.Range(0, 2) == 0 ? -1 : 1;
    }

    public void setPortalContainer(GameObject item)
    {
        _portalContainer = item;
    }

    public void setPowerupContainer(GameObject item)
    {
        _powerupContainer = item;
    }

    public void destroyPowerup()
    {
        _boundManager.bsInstantiate(armory.normalPrefab, transform.position, transform.rotation);
    }

    public void dodgeLaser()
    {
        transform.Translate(randomSign() * Vector3.right * 75 * Time.deltaTime);
    }

    public void reward()
    {
        GameObject present = rewards.getObject();
        if (_powerupContainer && present && Random.value <= rewardChance)
        {
            GameObject powerup = _boundManager.bsInstantiate(present, transform.position, Quaternion.identity);
            powerup.transform.parent = _powerupContainer.transform;
        }
    }
}