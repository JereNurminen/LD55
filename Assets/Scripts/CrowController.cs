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
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Destructible")) {
            collision.gameObject.GetComponent<CrateController>().Break();
            OnHit();
        } else if (collision.gameObject.CompareTag("Level")) {
            OnHit();
        } else if (collision.gameObject.CompareTag("Enemies")) {
            collision.gameObject.GetComponent<Health>().TakeDamage(1);
            OnHit();
        }
    }

    private void OnHit() {
        animator.SetTrigger("despawn");
        movement = Vector2.zero;
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
