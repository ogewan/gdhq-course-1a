using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingLaser : MonoBehaviour
{
    public Transform target;
    public float acceleration = 2;
    public float spinSpeed = 5f;
    public bool glitched = false;
    Laser self;
    private Pausible _pausible;

    void Start()
    {
        self = GetComponent<Laser>();
        _pausible = GetComponent<Pausible>();
    }

    void Update()
    {
        if (_pausible && _pausible.isPaused()) return;
        if (target)
        {
            if (!glitched)
            {
                Target();
            }
            else
            {
                Glitched();
            }
        }
        self.speed += acceleration * Time.deltaTime;
    }

    void Target()
    {
        Vector3 destination = target.position;
        Vector3 origin = transform.position;
        Vector3 targetVector = destination - origin;
        float angle = Vector3.Cross(transform.up, targetVector).z;
        transform.Rotate(Vector3.forward, angle * spinSpeed * Time.deltaTime, Space.World);
    }

    void Glitched()
    {
        Vector3 destination = target.position;
        Vector3 origin = transform.position;
        float angle = Vector3.Angle(origin, destination);
        transform.Rotate(Vector3.forward, angle, Space.World);
    }

    public void setTarget(Transform targetEnemy)
    {
        target = targetEnemy;
    }
}
