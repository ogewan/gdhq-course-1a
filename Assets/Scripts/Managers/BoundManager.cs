using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Anything that spawns must clear it with the BoundManager.

public class BoundManager : MonoBehaviour
{
    [SerializeField]
    private boundingBox _bbox;

    [System.Serializable]
    public struct boundingBox
    {
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;

        public boundingBox(Vector3 min, Vector3 max)
        {
            this.xMin = min.x;
            this.xMax = max.x;
            this.yMin = min.y;
            this.yMax = max.y;
        }
    }

    void Start()
    {
        Collider2D boundingbox = GetComponent<Collider2D>();
        Vector3 _min = boundingbox.bounds.min;
        Vector3 _max = boundingbox.bounds.max;
        _bbox = new boundingBox(_min, _max);
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
        return xPos <= _bbox.xMin || xPos >= _bbox.xMax;
    }

    bool outYBound(float yPos)
    {
        return yPos <= _bbox.yMin || yPos >= _bbox.yMax;
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
            player.transform.position = new Vector3(xPos <= _bbox.xMin ? _bbox.xMax : _bbox.xMin, yPos, 0);
        }

        if (outYBound(yPos))
        {
            player.transform.position = new Vector3(xPos, yPos <= _bbox.yMin ? _bbox.yMax : _bbox.yMin, 0);
        }
    }

    void Enemy(float xPos, float yPos, Collider2D enemy)
    {
        //Debug.Log("OobX " + outXBound(xPos) + " OobY " + outYBound(yPos));
        if (outXBound(xPos))
        {
            float spawnNewYPos = Random.Range(_bbox.yMin, _bbox.yMax);
            enemy.transform.position = new Vector3(xPos <= _bbox.xMin ? _bbox.xMax : _bbox.xMin, spawnNewYPos, 0);
        }

        if (outYBound(yPos))
        {
            float spawnNewXPos = Random.Range(_bbox.xMin, _bbox.xMax);
            enemy.transform.position = new Vector3(spawnNewXPos, yPos <= _bbox.yMin ? _bbox.yMax : _bbox.yMin, 0);
            //Debug.Log("yPos " + yPos + " yMin " + _yMin + " yMax " + _yMax);
        }
    }

    void EnemySuperLaser(float xPos, float yPos, Collider2D enemy)
    {
        //Debug.Log("OobX " + outXBound(xPos) + " OobY " + outYBound(yPos));

        if (outXBound(xPos))
        {
            float spawnNewYPos = Random.Range(_bbox.yMin, _bbox.yMax);
            enemy.transform.position = new Vector3(xPos <= _bbox.xMin ? _bbox.xMax : _bbox.xMin, spawnNewYPos, 0);
        }

        if (outYBound(yPos))
        {
            float spawnNewXPos = Random.Range(_bbox.xMin, _bbox.xMax);
            enemy.transform.position = new Vector3(spawnNewXPos, yPos <= _bbox.yMin ? _bbox.yMax : _bbox.yMin, 0);
            Debug.Log("yPos " + yPos + " yMin " + _bbox.yMin + " yMax " + _bbox.yMax);
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

    public boundingBox bbox()
    {
        return _bbox;
    }
}