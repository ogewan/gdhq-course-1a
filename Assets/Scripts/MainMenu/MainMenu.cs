﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void loadGame()
    {
        SceneManager.LoadScene(1); //main game scene
    }
}
