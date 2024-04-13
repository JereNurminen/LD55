using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 velocity = Vector2.zero;
    private Vector2 acceleration = Vector2.zero;
    public float gravity = 9.8f;
    public float runSpeed = 10.0f;

    public float groundRaycastDistance = 1 / 8f;
    public int groundRaycastCount = 2;
    public LayerMask groundLayerMask;

    private bool isGrounded = false;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;


    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /* Handle Inputs */
    void HandleInputs() {
        if (Input.GetKey(KeyCode.A)) {
            Debug.Log("A key is pressed");
            velocity.x = -runSpeed;
        } else if (Input.GetKey(KeyCode.D)) {
            Debug.Log("D key is pressed");
            velocity.x = runSpeed;
        } else {
            velocity.x = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            velocity.y = 10;
        }
    }

    void HandleGravity() {
        if (!isGrounded) {
            velocity.y = -gravity * Time.deltaTime;
        } else {
            velocity.y = 0;
        }
    }

    void CheckForGround() {
        Vector2 colliderBottomLeft = new Vector2(boxCollider.bounds.min.x, boxCollider.bounds.center.y);
        float raylength = boxCollider.bounds.size.y / 2 + groundRaycastDistance;
        float colliderWidth = boxCollider.bounds.size.x;

        // This actually draws one more ray than defined in groundRaycastCount, don't care about fixing it rn
        for (int i = 0; i <= groundRaycastCount; i++) {
            Vector2 origin = new Vector2(colliderBottomLeft.x + (i * ( colliderWidth / groundRaycastCount )), colliderBottomLeft.y);
            Debug.DrawRay(origin, Vector2.down * raylength, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raylength, groundLayerMask);
            if (hit.collider != null) {
                isGrounded = true;
                return;
            }
            isGrounded = false;
        }
    }

    void ApplyVelocity() {
        // Apply the velocity to the player
        transform.Translate(velocity * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInputs();
    }

    void FixedUpdate() {
        CheckForGround();
        ApplyVelocity();
        HandleGravity();
    }
}
