using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Anything that spawns must clear it with the BoundManager.

public class BoundManager : MonoBehaviour
{
    [SerializeField]
    private float _xMin;
    [SerializeField]
    private float _xMax;
    [SerializeField]
    private float _yMin;
    [SerializeField]
    private float _yMax;
    [SerializeField]

    void Start()
    {
        Collider2D boundingbox = GetComponent<Collider2D>();
        Vector3 _min = boundingbox.bounds.min;
        Vector3 _max = boundingbox.bounds.max;
        _xMin = _min.x;
        _xMax = _max.x;
        _yMin = _min.y;
        _yMax = _max.y;
    }


    void OnTriggerExit2D(Collider2D other)
    {
        float x = other.transform.position.x;
        float y = other.transform.position.y;

        //Debug.Log(other.tag);
        switch (other.tag)
        {
            case "Enemy":
                Enemy(x, y, other);
                break;
            case "Player":
                Player(x, y, other);
                break;
            case "EnemySuperLaser":
                EnemySuperLaser(x, y, other);
                break;
            case "TEST":
                break;
            default:
                //Debug.Log(other.tag);
                //Debug.Log("OUT OF BOUDNDS");
                //Debug.Break();
                //Debug.Log(other.gameObject);
                Destroy(other.gameObject);
                break;
        }
    }

    bool outXBound(float xPos)
    {
        return xPos <= _xMin || xPos >= _xMax;
    }

    bool outYBound(float yPos)
    {
        return yPos <= _yMin || yPos >= _yMax;
    }

    bool oob(Vector3 pos)
    {
        //Debug.Log("BoolTest " + (outXBound(pos.x) || outXBound(pos.y)) + " oobxBOOL " + outXBound(pos.x) + " oobyBOOL " + outXBound(pos.y));
        return (outXBound(pos.x) || outYBound(pos.y));
    }

    public GameObject checkGameObject(GameObject original)
    {
        if (original)
        {
            if (oob(original.transform.position))
            {
                Destroy(original);
                return null;
            }
            Transform[] allTransforms = original.GetComponentsInChildren<Transform>();
            foreach (Transform child in allTransforms)
            {
                //Debug.Log("Position " + child.transform.position + " oob " + oob(child.transform.position) + " oobY " + outYBound(child.transform.position.y) + " oobX " + outXBound(child.transform.position.x));
                if (oob(child.position))
                {
                    Destroy(child.gameObject);
                }
            }
        }
        //Debug.Break();
        return original;
    }

    void Player(float xPos, float yPos, Collider2D player)
    {
        if (outXBound(xPos))
        {
            player.transform.position = new Vector3(xPos <= _xMin ? _xMax : _xMin, yPos, 0);
        }

        if (outYBound(yPos))
        {
            player.transform.position = new Vector3(xPos, yPos <= _yMin ? _yMax : _yMin, 0);
        }
    }

    void Enemy(float xPos, float yPos, Collider2D enemy)
    {
        Debug.Log("OobX " + outXBound(xPos) + " OobY " + outYBound(yPos));
        if (outXBound(xPos))
        {
            float spawnNewYPos = Random.Range(_yMin, _yMax);
            enemy.transform.position = new Vector3(xPos <= _xMin ? _xMax : _xMin, spawnNewYPos, 0);
        }

        if (outYBound(yPos))
        {
            float spawnNewXPos = Random.Range(_xMin, _xMax);
            enemy.transform.position = new Vector3(spawnNewXPos, yPos <= _yMin ? _yMax : _yMin, 0);
            //Debug.Log("yPos " + yPos + " yMin " + _yMin + " yMax " + _yMax);
        }
    }

    void EnemySuperLaser(float xPos, float yPos, Collider2D enemy)
    {
        //Debug.Log("OobX " + outXBound(xPos) + " OobY " + outYBound(yPos));

        if (outXBound(xPos))
        {
            float spawnNewYPos = Random.Range(_yMin, _yMax);
            enemy.transform.position = new Vector3(xPos <= _xMin ? _xMax : _xMin, spawnNewYPos, 0);
        }

        if (outYBound(yPos))
        {
            float spawnNewXPos = Random.Range(_xMin, _xMax);
            enemy.transform.position = new Vector3(spawnNewXPos, yPos <= _yMin ? _yMax : _yMin, 0);
            Debug.Log("yPos " + yPos + " yMin " + _yMin + " yMax " + _yMax);
        }
        SpriteRenderer sprite = enemy.GetComponent<SpriteRenderer>();
        sprite.color = new Color(0, 1, 1);
        enemy.tag = "EnemyLaser";
    }

    public GameObject bsInsantiate(Object original, Vector3 position, Quaternion rotation)
    {
        //Debug.Log("Position " + position + " oob " + oob(position) + " oobY " + outYBound(position.y) + " oobX " + outXBound(position.x));
        //Debug.Break();
        GameObject possible = (!oob(position)) ? (GameObject)Instantiate(original, position, rotation) : null;
        return checkGameObject(possible);
    }
}