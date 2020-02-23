using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausible : MonoBehaviour
{
    public GameManager gameManager;

    public bool isPaused()
    {
        return !gameManager ? false : gameManager.isPaused();
    }
}
