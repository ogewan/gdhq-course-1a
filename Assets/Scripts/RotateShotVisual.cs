using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShotVisual : MonoBehaviour
{
    [SerializeField]
    private float _speed = 100f;
    void Update()
    {
        transform.Rotate(Vector3.forward, _speed * Time.deltaTime, Space.World);
    }
}
