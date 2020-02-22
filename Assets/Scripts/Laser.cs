using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 8f;
    private Pausible _pausible;

    void Start()
    {
        _pausible = GetComponent<Pausible>();
    }

    void Update()
    {
        if (_pausible && _pausible.isPaused()) return;
        Movement();
    }

    void Movement()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool hitPortal = other.tag == "Portal";
        if (hitPortal && tag == "Laser")
        {
            speed *= -1;
            gameObject.tag = "EnemyLaser";
            GetComponent<SpriteRenderer>().color = new Color32(0x00, 0xB4, 0xFF, 0xFF);
            other.gameObject.GetComponent<Portal>().closePortal();
        }
    }
}
