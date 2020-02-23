using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum type { Normal, Super, Hyper, Ultra, Tohou, Tohou_gun };

    [SerializeField]
    private type enemyType;
    [SerializeField]
    private Transform _rotateAim;
    [SerializeField]
    private float _portalOffset = 4f;
    [SerializeField]
    private int _portalLimit = 30;
    [SerializeField]
    private GameObject _portalContainer;
    [SerializeField]
    private GameObject[] _gunners = new GameObject[3];
    [SerializeField]
    private float _regenerateTime = 4f;
    [SerializeField]
    private bool _activated = true;
    [SerializeField]
    private GameObject _indicator;
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
    private SpawnManager.Randomizer _projectile;
    private BoundManager _boundManager;
    private GameManager _gameManager;
    private StoryManager _storyManager;
    private Pausible _pausible;

    void Start()
    {
        EnemySetup();
        _gameManager = managers.gameManager;
        _boundManager = managers.boundManager;
        _storyManager = managers.storyManager;
        _pausible = GetComponent<Pausible>();
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
            gun.SetActive(true);
        }
    }


    void Update()
    {
        if (_pausible && _pausible.isPaused()) return;
        Movement();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool hitPlayer = other.tag == "Player";
        bool hitLaser = other.tag == "Laser";
        bool hitPortal = other.tag == "Portal";
        bool isTohouGun = enemyType == type.Tohou_gun;

        if (hitPlayer || hitLaser)
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
            if (enemyType == type.Tohou_gun)
            {
                StartCoroutine(regenerate());
                _indicator.SetActive(false);
                _activated = false;
            }
            else
            {
                EnemyDies();
            }
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
        Destroy(gameObject, 2.8f);
    }

    void Movement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
    }

    void EnemySetup()
    {
        SpawnManager.Spawnlet[] pool;
        SpawnManager.Spawnlet normal = new SpawnManager.Spawnlet(armory.normalPrefab, 3);
        SpawnManager.Spawnlet rotate = new SpawnManager.Spawnlet(armory.rotateShotPrefab, 3);
        SpawnManager.Spawnlet homing = new SpawnManager.Spawnlet(armory.homingShotPrefab, 3);
        SpawnManager.Spawnlet super = new SpawnManager.Spawnlet(armory.superShotPrefab, 3);
        SpawnManager.Spawnlet portal = new SpawnManager.Spawnlet(armory.glitchShotPrefab);
        SpawnManager.Spawnlet specialPortal = new SpawnManager.Spawnlet(armory.glitchShotPrefab, 2);

        switch (enemyType)
        {
            case type.Normal:
                pool = new SpawnManager.Spawnlet[1] { normal };
                _projectile = new SpawnManager.Randomizer(pool);
                break;
            case type.Super:
                _scaler = 1.5f;
                pool = new SpawnManager.Spawnlet[3] { normal, rotate, portal };
                _projectile = new SpawnManager.Randomizer(pool);
                break;
            case type.Hyper:
                _scaler = 3f;
                pool = new SpawnManager.Spawnlet[3] { rotate, homing, portal };
                _projectile = new SpawnManager.Randomizer(pool);
                break;
            case type.Ultra:
                _scaler = 5f;
                pool = new SpawnManager.Spawnlet[4] { super, rotate, homing, specialPortal };
                _projectile = new SpawnManager.Randomizer(pool);
                break;
            case type.Tohou:
                _scaler = 6f;
                pool = new SpawnManager.Spawnlet[1] { portal };
                _projectile = new SpawnManager.Randomizer(pool);
                setGunners();
                break;
            case type.Tohou_gun:
                pool = new SpawnManager.Spawnlet[4] { normal, super, rotate, homing };
                _projectile = new SpawnManager.Randomizer(pool);
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
        while (!_destroyed)
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
                Quaternion projectileRotation = (isRotateShot) ? _rotateAim.rotation : transform.rotation;
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
        _indicator.SetActive(true);
        _activated = true;

    }

    T registerComponent<T>(string name)
    {
        GameObject manager = GameObject.Find(name);
        if (manager)
        {
            return manager.GetComponent<T>();
        }
        Debug.LogError(name + " not found");
        return default(T);
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
}