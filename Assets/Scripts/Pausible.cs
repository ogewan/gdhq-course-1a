using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausible : MonoBehaviour
{
    public GameManager _gameManager;
    public bool isPaused()
    {
        return _gameManager.isPaused();
    }
}
