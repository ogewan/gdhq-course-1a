using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundManager : MonoBehaviour
{
    private float _xMin;
    private float _xMax;
    private float _yMin;
    private float _yMax;

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

        Debug.Log(other.tag);
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
            default:
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
            Debug.Log("yPos " + yPos + " yMin " + _yMin + " yMax " + _yMax);
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
        enemy.tag = "EnemyLaser";
    }
}