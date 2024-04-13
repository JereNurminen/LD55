using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 velocity = Vector2.zero;
    private Vector2 acceleration = Vector2.zero;
    public float gravity = 9.8f;
    public float runSpeed = 10.0f;

    public float groundRaycastDistance = 1 / 16f;
    public int groundRaycastCount = 2;
    public LayerMask groundLayerMask;

    private bool isGrounded = false;

    private BoxCollider2D boxCollider;


    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    /* Handle Inputs */
    void HandleInputs() {
        if (Input.GetKey(KeyCode.W)) {
            
            // Move forward
        } else if (Input.GetKey(KeyCode.S)) {
            // Move backward
        }
        if (Input.GetKey(KeyCode.A)) {
            Debug.Log("A key is pressed");
            velocity.x = -runSpeed;
        } else if (Input.GetKey(KeyCode.D)) {
            Debug.Log("D key is pressed");
            velocity.x = runSpeed;
        } else {
            velocity.x = 0;
        }
    }

    void HandleGravity() {
        velocity.y = -gravity * Time.deltaTime;
    }

    void CheckForGround() {
        Vector2 colliderBottomLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.min.y);
        float colliderWidth = boxCollider.bounds.size.x;

        // This actually draws one more ray than defined in groundRaycastCount, don't care about fixing it rn
        for (int i = 0; i <= groundRaycastCount; i++) {
            Vector2 origin = new Vector2(colliderBottomLeft.x + (i * ( colliderWidth / groundRaycastCount )), colliderBottomLeft.y);
            Debug.DrawRay(origin, Vector2.down * groundRaycastDistance, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundRaycastDistance, groundLayerMask);
            if (hit.collider != null) {
                isGrounded = true;
                return;
            }
        }
    }

    void ApplyVelocity() {
        // Apply the velocity to the player
        transform.Translate(velocity * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGround();
        HandleInputs();
        ApplyVelocity();
        HandleGravity();
    }
}
