using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float _speed = 8f;
    [SerializeField]
    private Vector2 _yBound = new Vector2(-6f, 7f);

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        float yPos = transform.position.y;
        float yMin = _yBound[0];
        float yMax = _yBound[1];
        if (yPos <= yMin || yPos >= yMax)
        {
            Destroy(gameObject);
        }
    }
}
