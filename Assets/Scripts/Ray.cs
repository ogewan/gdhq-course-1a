using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ray : MonoBehaviour
{
    public enum type { shootPowerup, dodgeLaser };

    public type rayType;
    public bool active = true;
    public Enemy parent;
    public float timer = 4f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (active && gameObject.activeInHierarchy)
        {
            if (other.tag == "Powerup" && rayType == type.shootPowerup)
            {
                active = false;
                StartCoroutine(activateCooldown());
                parent.destroyPowerup();
            }

            if (other.tag == "Laser" && rayType == type.dodgeLaser)
            {
                active = false;
                StartCoroutine(activateCooldown());
                parent.dodgeLaser();
            }
        }
    }

    IEnumerator activateCooldown()
    {
        yield return new WaitForSeconds(timer);
        active = true;
    }
}
