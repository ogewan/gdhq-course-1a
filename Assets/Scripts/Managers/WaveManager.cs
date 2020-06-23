using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public Registry managers;
    public Transform eContainer;
    public GameObject pContainer;
    public GameObject puContainer;
    public Transform playerPos;
    public GameObject green;
    public GameObject yellow;
    public GameObject red;
    public GameObject white;
    public GameObject black;
    public GameObject bomb;
    public GameObject boss;
    //public wave[] waves;

    private SpawnManager _spawnManager;
    private Dictionary<int, SpawnManager.spawnIngredient[]>[] waveList;

    void Start()
    {
        _spawnManager = managers.spawnManager;

        waveList = waveListing(8);
        //WAVE 1
        {
            waveList[0][0] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(green, eContainer)
            };
            waveList[0][1] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(green, eContainer, 2)
            };
            waveList[0][3] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(green, eContainer, 3)
            };
            waveList[0][6] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(green, eContainer, 4)
            };
            waveList[0][10] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(green, eContainer, 5)
            };
            waveList[0][15] = new SpawnManager.spawnIngredient[] { };
        }
        //WAVE 2
        {
            waveList[1][0] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(yellow, eContainer)
            };
            waveList[1][1] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(yellow, eContainer),
                new SpawnManager.spawnIngredient(green, eContainer, 2)
            };
            waveList[1][4] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(yellow, eContainer, 2)
            };
            waveList[1][6] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(yellow, eContainer, 2),
                new SpawnManager.spawnIngredient(green, eContainer, 3)
            };
            waveList[1][11] = new SpawnManager.spawnIngredient[] { };
        }
        //WAVE 3
        {
            waveList[2][0] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(red, eContainer)
            };
            waveList[2][1] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(yellow, eContainer),
                new SpawnManager.spawnIngredient(red, eContainer)
            };
            waveList[2][3] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(yellow, eContainer, 3)
            };
            waveList[2][6] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(yellow, eContainer, 2),
                new SpawnManager.spawnIngredient(red, eContainer, 1)
            };
            waveList[2][9] = new SpawnManager.spawnIngredient[] { };
        }
        //WAVE 4
        {
            waveList[3][0] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(bomb, eContainer)
            };
            waveList[3][1] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(bomb, eContainer, 2)
            };
            waveList[3][3] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(bomb, eContainer, 2),
                new SpawnManager.spawnIngredient(white, eContainer, 1)
            };
            waveList[3][6] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(bomb, eContainer, 2),
                new SpawnManager.spawnIngredient(white, eContainer, 2)
            };
            waveList[3][10] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(bomb, eContainer, 2),
                new SpawnManager.spawnIngredient(white, eContainer, 3)
            };
            waveList[3][15] = new SpawnManager.spawnIngredient[] { };
        }
        //WAVE 5
        {
            waveList[4][0] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(black, eContainer)
            };
            waveList[4][1] = new SpawnManager.spawnIngredient[] { };
        }
        //WAVE 6
        {
            waveList[5][0] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(white, eContainer),
                new SpawnManager.spawnIngredient(green, eContainer, 2)
            };
            waveList[5][3] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(yellow, eContainer),
                new SpawnManager.spawnIngredient(green, eContainer, 2)
            };
            waveList[5][6] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(red, eContainer),
                new SpawnManager.spawnIngredient(green, eContainer, 2)
            };
            waveList[5][9] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(white, eContainer, 2),
                new SpawnManager.spawnIngredient(green, eContainer, 4)
            };
            waveList[5][15] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(bomb, eContainer, 10)
            };
            waveList[5][25] = new SpawnManager.spawnIngredient[] { };
        }
        //WAVE 7
        {
            waveList[6][0] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(white, eContainer)
            };
            waveList[6][1] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(white, eContainer, 2)
            };
            waveList[6][3] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(white, eContainer, 3)
            };
            waveList[6][6] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(white, eContainer, 4)
            };
            waveList[6][10] = new SpawnManager.spawnIngredient[] {
                new SpawnManager.spawnIngredient(white, eContainer, 4),
                new SpawnManager.spawnIngredient(black, eContainer)
            };
            waveList[6][15] = new SpawnManager.spawnIngredient[] { };
        }
        //WAVE 8
        {
            waveList[7][100] = new SpawnManager.spawnIngredient[] { };
        }
    }

    public Dictionary<int, SpawnManager.spawnIngredient[]>[] waveListing(int waves)
    {
        Dictionary<int, SpawnManager.spawnIngredient[]>[] waveList = new Dictionary<int, SpawnManager.spawnIngredient[]>[waves];
        for (int i = 0; i < waveList.Length; i++)
        {
            waveList[i] = new Dictionary<int, SpawnManager.spawnIngredient[]>();
        }
        return waveList;
    }

    public bool waveStory(int wave, int killed)
    {
        if (wave == 0 || waveList.Length <= wave - 1)
        {
            return false;
        }
        Dictionary<int, SpawnManager.spawnIngredient[]> level = waveList[wave - 1];
        SpawnManager.spawnIngredient[] recipe;
        if (level.TryGetValue(killed, out recipe))
        {
            if (recipe.Length == 0)
            {
                // If target exist but there is no recipe, go to next wave
                return true;
            }
            _spawnManager.spawnRecipe(recipe);
        }

        // FINAL WAVE
        if (wave == 8 && killed == 0)
        {
            GameObject tohou = Instantiate(boss, new Vector3(0, 10), Quaternion.identity);

            SpawnManager.setPausible(tohou, managers);
            tohou.GetComponent<Enemy>().managers = managers;
            tohou.GetComponent<Enemy>().player = playerPos;
            tohou.transform.parent = eContainer;
            tohou.GetComponent<Enemy>().setPortalContainer(pContainer);
            tohou.GetComponent<Enemy>().setPowerupContainer(puContainer);
        }
        return false;
    }
}
