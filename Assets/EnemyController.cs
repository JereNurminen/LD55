using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public float speed = 1.0f;
    public float gravity = 9.8f;
    public float groundRaycastDistance = 1 / 16f;
    public LayerMask visionBlockingLayerMask;
    public float visionRange = 10.0f;

    private Vector2 velocity = Vector2.zero;
    private bool isGrounded = false;
    private bool isAlive = true;
    private bool isCharging = false;
    public float chargeCooldown = 1.0f;
    private float timeSinceLastChargeEnd = 0.0f;
    private int chargeDirection = 1;
    private BoxCollider2D boxCollider;
    private CollisionDetection collisionDetection;
    private Animator animator;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        collisionDetection = GetComponent<CollisionDetection>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    void CheckForGround()
    {
        var hit = collisionDetection.CheckForGround();
        if (hit != null)
        {
            isGrounded = true;
            velocity.y = Mathf.Max(0, velocity.y);
            transform.position = new Vector2(
                transform.position.x,
                hit.Value.point.y + boxCollider.bounds.size.y / 2 + groundRaycastDistance
            );
        }
        else
        {
            isGrounded = false;
        }
    }

    void CheckForWalls()
    {
        var hit = collisionDetection.CheckForWalls(velocity.x);
        if (hit != null)
        {
            velocity.x = 0;
            StopCharging();
        }
    }

    void ApplyVelocity()
    {
        // Apply the velocity to the player
        transform.Translate(velocity * Time.deltaTime);
        if (velocity.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (velocity.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

    private bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            player.transform.position - transform.position,
            visionRange,
            visionBlockingLayerMask
        );
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.DrawLine(transform.position, player.transform.position, Color.red);
                return true;
            }
            Debug.DrawLine(transform.position, hit.point, Color.green);
            return false;
        }
        else
        {
            Debug.DrawRay(
                transform.position,
                (player.transform.position - transform.position).normalized * visionRange,
                Color.green
            );
            return false;
        }
    }

    public void onDeath()
    {
        animator.SetTrigger("die");
        isAlive = false;
    }

    private void ChargePlayer()
    {
        if (!isCharging && timeSinceLastChargeEnd > chargeCooldown)
        {
            Debug.Log("Charging");
            var direction = player.transform.position.x - transform.position.x > 0 ? 1 : -1;
            chargeDirection = direction;
            isCharging = true;
        }
        velocity.x = chargeDirection * speed;
    }

    private void StopCharging()
    {
        isCharging = false;
        chargeDirection = 0;
        velocity.x = 0;
        timeSinceLastChargeEnd = 0.0f;
    }

    public void onDamage()
    {
        StopCharging();
        timeSinceLastChargeEnd = chargeCooldown / 2;
        //animator.SetTrigger("damage");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGround();
        if (isAlive)
        {
            CheckForWalls();
            CanSeePlayer();

            if (CanSeePlayer())
            {
                ChargePlayer();
            }
        }
        timeSinceLastChargeEnd += Time.deltaTime;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        ApplyVelocity();
    }
}
