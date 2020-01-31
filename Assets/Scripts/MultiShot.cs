using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShot : MonoBehaviour
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
