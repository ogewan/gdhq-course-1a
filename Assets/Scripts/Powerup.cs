using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum type { ammo, repair, shield, triple, rotate, homing, glitch, star };
    [SerializeField]
    private float _speed = 0f;
    [SerializeField]
    private type _type;
    private Pausible _pausible;

    void Start()
    {
        _pausible = GetComponent<Pausible>();
    }

    void Update()
    {
        if (_pausible && _pausible.isPaused()) return;
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player)
            {
                player.activatePowerUp(_type);
                Destroy(gameObject);
            }
        }
    }
}
