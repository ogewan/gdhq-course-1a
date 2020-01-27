using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject laserPrefab;
    public AudioClip laserFire;
    public AudioClip explodeSound;
    [SerializeField]
    private Vector2 fireTimeRange = new Vector2(3f, 5f);
    [SerializeField]
    private float _enemySpeed = 4f;
    [SerializeField]
    private Vector2 _bounds = new Vector2(-6f, 7f);
    [SerializeField]
    private Vector2 _spawnBounds = new Vector2(-11.5f, 10.5f);
    [SerializeField]
    private int _scoreValue = 10;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private bool _destroyed = false;
    private Animator _animator;
    private AudioSource _audioPlayer;

    void Start()
    {
        _player =  registerManager<Player>("Player");
        _animator = GetComponent<Animator>();
        _audioPlayer = GetComponent<AudioSource>();
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
            _destroyed = true;
            _player.addScore(_scoreValue);
            _animator.SetTrigger("onEnemyDeath");
            _audioPlayer.clip = explodeSound;
            _audioPlayer.Play();
            _enemySpeed = 0;
            Destroy(gameObject, 2.8f);
        }
    }

    void Movement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        float yPos = transform.position.y;
        float xMin = _spawnBounds[0];
        float xMax = _spawnBounds[1];
        float yMin = _bounds[0];
        float yMax = _bounds[1];
        if (yPos <= yMin)
        {
            float spawnPos = Random.Range(xMin, xMax);
            transform.position = new Vector3(spawnPos, yMax, 0);
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
            if (!_destroyed)
            {
                Instantiate(laserPrefab, transform.position, Quaternion.identity);
                _audioPlayer.Play();
            }
        }
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
}