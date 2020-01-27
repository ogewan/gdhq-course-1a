using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private string _type = "tripleShot";
    [SerializeField]
    private float _bound = -6f;

    void Update()
    {
        if (transform.position.y <= _bound)
        {
            Destroy(gameObject);
        }
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
