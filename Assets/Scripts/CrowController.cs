using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrowController : MonoBehaviour
{
    public float speed = 1.0f;
    public int damage = 1;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Vector2 movement;
    private Animator animator;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log(collision.gameObject.tag);
        var health = collision.gameObject.GetComponent<Health>();
        OnHit();

        if (health != null) {
            health.TakeDamage(1, collision.GetContact(0).point);
        }
    }

    private void OnHit() {
        animator.SetTrigger("despawn");
        movement = Vector2.zero;
        boxCollider.enabled = false;   
    }

    public void OnDespawnAnimationEnd() {
        Destroy(gameObject);
    }

    public void Spawn(Vector2 startPosition, Vector2 targetPosition) {
        this.startPosition = startPosition;
        this.targetPosition = targetPosition;
        movement = ( targetPosition - startPosition).normalized * speed;
    }

    void FixedUpdate() {
        rb.MovePosition(rb.position + movement * Time.deltaTime);
    }
}
