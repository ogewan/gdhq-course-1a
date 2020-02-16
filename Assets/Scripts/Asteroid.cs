using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotSpeed = 2f;
    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private SpawnManager _spawnManager;
    private Pausible _pausible;

    void Start()
    {
        _pausible = GetComponent<Pausible>();
    }

    void Update()
    {
        if (_pausible && _pausible.isPaused()) return;
        transform.Rotate(Vector3.forward * Time.deltaTime * _rotSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool hitLaser = other.tag == "Laser";
        if (hitLaser && _spawnManager)
        {
            Destroy(other.gameObject);
            _spawnManager.startSpawning();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.5f);
        }
    }
}
