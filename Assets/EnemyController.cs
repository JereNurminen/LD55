using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public float speed = 1.0f;
    public float gravity = 9.8f;
    public float groundRaycastDistance = 1 / 16f;
    private Vector2 velocity = Vector2.zero;
    private bool isGrounded = false;
    private BoxCollider2D boxCollider;
    private CollisionDetection collisionDetection; 

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        collisionDetection = GetComponent<CollisionDetection>();
    }

    void ApplyGravity() {
        if (!isGrounded) {
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    void CheckForGround() {
        var hit = collisionDetection.CheckForGround();
        if (hit != null) {
            Debug.Log(hit.Value.collider.gameObject.name);
            isGrounded = true;
            velocity.y = Mathf.Max(0, velocity.y);
            transform.position = new Vector2(transform.position.x, hit.Value.point.y + boxCollider.bounds.size.y / 2 + groundRaycastDistance);
        } else {
            isGrounded = false;
        }
    }

    void CheckForWalls() {
        var hit = collisionDetection.CheckForWalls(velocity.x);
        if (hit != null) {
            velocity.x = 0;
        }
    }

    void ApplyVelocity() {
        // Apply the velocity to the player
        transform.Translate(velocity * Time.deltaTime);
        if (velocity.x < 0) {
            transform.localScale = new Vector2(-1, 1);
        } else if (velocity.x > 0) {
            transform.localScale = new Vector2(1, 1);
        }
    }


    // Update is called once per frame
    void Update()
    {
        CheckForGround();
        CheckForWalls();
    }

    void FixedUpdate() {
        ApplyGravity();
        ApplyVelocity();
    }
}