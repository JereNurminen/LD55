using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrowController : MonoBehaviour
{
    public float speed = 1.0f;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Vector2 movement;
    private Animator animator;
    private Rigidbody2D rigidbody;
    private BoxCollider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Collision");
        if (collision.gameObject.CompareTag("Destructible")) {
            Debug.Log("Crow hit a crate");
            collision.gameObject.GetComponent<CrateController>().Break();
            Despawn();
        }
    }

    private void Despawn() {
        movement = Vector2.zero;
        animator.SetTrigger("despawn");
        collider.enabled = false;   
    }

    public void OnDespawnAnimationEnd() {
        Destroy(gameObject);
    }

    public void Spawn(Vector2 startPosition, Vector2 targetPosition) {
        this.startPosition = startPosition;
        this.targetPosition = targetPosition;
        this.movement = ( targetPosition - startPosition).normalized * speed;
    }

    void FixedUpdate() {
        rigidbody.MovePosition(rigidbody.position + movement * Time.deltaTime);
    }
}
