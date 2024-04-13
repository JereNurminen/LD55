using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummoningController : MonoBehaviour
{

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Despawn() {
        animator.SetTrigger("despawn");
    }

    public void OnDespawnAnimationEnd() {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
