using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float _speed = 8f;

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }
}
