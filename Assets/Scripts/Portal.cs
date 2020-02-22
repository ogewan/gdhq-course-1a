using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public float duration = 10;
    [SerializeField]
    private float animTime = 0.65f;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(selfDestruct());
    }

    IEnumerator selfDestruct()
    {
        yield return new WaitForSeconds(duration - animTime);
        closePortal();
    }

    public void closePortal()
    {
        _animator.SetBool("deactivated", true);
        Destroy(gameObject, animTime);
    }
}