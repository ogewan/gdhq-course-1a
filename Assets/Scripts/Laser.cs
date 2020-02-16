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
}
