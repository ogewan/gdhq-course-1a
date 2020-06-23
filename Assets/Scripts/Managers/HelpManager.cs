using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HelpManager : MonoBehaviour
{
    public GameObject[] pages;
    [SerializeField]
    private int index = 0;
    [SerializeField]
    private int current = 0;

    void change()
    {
        pages[current].SetActive(false);
        current = index;
        pages[current].SetActive(true);
    }

    public void next()
    {
        index++;
        if (index >= pages.Length)
        {
            index = 0;
        }
        change();
    }

    public void back()
    {
        index--;
        if (index < 0)
        {
            index = pages.Length - 1;
        }
        change();
    }

    public void exit()
    {
        SceneManager.LoadScene(0);
    }
}
