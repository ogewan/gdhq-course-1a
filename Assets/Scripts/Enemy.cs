using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum type { Normal, Super, Hyper, Ultra, Tohou };

    [SerializeField]
    private type enemyType;
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
    public Player player;
    [SerializeField]
    private bool _destroyed = false;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private AudioSource _audioPlayer;
    [SerializeField]
    private Collider2D _selfCollider;
    //[SerializeField]
    //private GameObject[] _thrusters;
    public Registry managers;
    private BoundManager _boundManager;
    private GameManager _gameManager;
    private StoryManager _storyManager;

    void Start()
    {
        _gameManager = managers.gameManager;
        _boundManager = managers.boundManager;
        _storyManager = managers.storyManager;
        //_animator = GetComponent<Animator>();
        //_audioPlayer = GetComponent<AudioSource>();
        //_selfCollider = GetComponent<Collider2D>();
        _audioPlayer.clip = laserFire;
        StartCoroutine(fireRoutine());
    }

    void Update()
    {
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
        _animator.SetTrigger("onEnemyDeath");
        //Debug.Log("scale " + transform.localScale);
        switch (enemyType)
        {
            case type.Super:
                transform.localScale = transform.localScale * 1.5f;
                break;
            case type.Hyper:
                transform.localScale = transform.localScale * 3f;
                break;
            case type.Ultra:
                transform.localScale = transform.localScale * 6f;
                break;
            case type.Tohou:
                transform.localScale = transform.localScale * 12f;
                break;
        }
        //Debug.Log("scale to " + transform.localScale);
        //Debug.Break();
        _audioPlayer.clip = explodeSound;
        _audioPlayer.Play();
        _enemySpeed = 0;
        Destroy(gameObject, 2.8f);
    }

    void Movement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
    }

    IEnumerator fireRoutine()
    {
        while (!_destroyed)
        {
            float minTime = Mathf.Max(0, Mathf.Min(fireTimeRange[0], fireTimeRange[1]));
            float maxTime = Mathf.Max(1, Mathf.Max(fireTimeRange[0], fireTimeRange[1]));
            float spawnTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(spawnTime);
            if (!_destroyed)
            {
                _boundManager.bsInsantiate(armory.normalPrefab, transform.position, transform.rotation);
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
}