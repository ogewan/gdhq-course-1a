using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShot : MonoBehaviour
{
    private Pausible _pausible;
    [SerializeField]
    private GameObject[] lasers = new GameObject[3];

    void Start()
    {
        _pausible = GetComponent<Pausible>();
        foreach (GameObject laser in lasers)
        {
            SpawnManager.setPausible(laser, _pausible.gameManager);
        }
    }

    void Update()
    {
        bool empty = gameObject.transform.childCount == 0;
        if (empty)
        {
            Destroy(gameObject);
        }
    }
}
