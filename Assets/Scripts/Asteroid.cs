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

    void Start()
    {
        _spawnManager = registerManager<SpawnManager>("SpawnManager");
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * _rotSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool hitLaser = other.tag == "Laser";
        if (hitLaser)
        {
            Destroy(other.gameObject);
            _spawnManager.startSpawning();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.5f);
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
