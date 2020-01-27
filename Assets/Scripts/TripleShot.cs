using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : MonoBehaviour
{
    void Update()
    {
        bool empty = gameObject.transform.childCount == 0;
        if (empty)
        {
            Destroy(gameObject);
        }
    }
}
