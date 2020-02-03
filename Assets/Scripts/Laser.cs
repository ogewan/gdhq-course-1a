using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 8f;

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
