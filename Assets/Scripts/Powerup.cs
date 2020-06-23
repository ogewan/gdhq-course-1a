using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum type { ammo, repair, shield, triple, rotate, homing, glitch, star, noammo, breakk, disable };
    public Player player;
    public Transform playerPosition;
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
        Vector3 drift = Vector3.zero;
        if (player && player.absorbActive)
        {
            Vector3 destination = playerPosition.position;
            Vector3 origin = transform.position;
            Vector3 targetVector = destination - origin;
            float distance = Vector3.Distance(destination, origin);
            drift = targetVector / (distance - player.absorbStrength);
            drift.z = 0;
        }
        Vector3 translation = (Vector3.down + drift) * _speed * Time.deltaTime;
        //Debug.Log("Drift: " + drift);
        //Debug.Log("Move: " + translation);
        transform.Translate(translation);
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
        else if (other.tag == "EnemyLaser" && tag != "Star")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
