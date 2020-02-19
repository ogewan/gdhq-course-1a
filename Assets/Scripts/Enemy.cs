using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum type { Normal, Super, Hyper, Ultra, Tohou };

    [SerializeField]
    private type enemyType;
    [SerializeField]
    private Transform _rotateAim;
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
        //_animator = GetComponent<Animator>();
        //_audioPlayer = GetComponent<AudioSource>();
        //_selfCollider = GetComponent<Collider2D>();
        _audioPlayer.clip = laserFire;
        StartCoroutine(fireRoutine());
        _sprite = GetComponent<SpriteRenderer>();
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
    }

    void Damage()
    {
        _health--;
        if (_health <= 0 && !_destroyed)
        {
            EnemyDies();
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
                pool = new SpawnManager.Spawnlet[4] { super, rotate, homing, portal };
                _projectile = new SpawnManager.Randomizer(pool);
                break;
            case type.Tohou:
                _scaler = 6f;
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

    void rotateLaser(GameObject laser)
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
            if (!_destroyed && !isPaused)
            {
                GameObject laser = _projectile.Get();
                chooseTarget(laser);
                rotateLaser(laser);
                _boundManager.bsInstantiate(laser, transform.position, (armory.rotateShotPrefab.name == laser.name) ? _rotateAim.rotation : transform.rotation);
                _audioPlayer.Play();
            }
        }
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
}