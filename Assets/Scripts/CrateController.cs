using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrateController : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D collider;
    private Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();   
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBreakAnimationEnd() {
        Destroy(gameObject);
    }

    public void Break() {
        animator.SetTrigger("break");
        rigidbody.isKinematic = true;
    }
}
