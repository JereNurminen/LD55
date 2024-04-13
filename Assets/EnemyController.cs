using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        collisionDetection = GetComponent<CollisionDetection>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    RaycastHit2D? CheckForGround()
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
        return hit;
    }

    RaycastHit2D? CheckForCeiling()
    {
        var hit = collisionDetection.CheckForCeiling();
        if (hit != null)
        {
            velocity.y = 0;
        }
        return hit;
    }

    RaycastHit2D? CheckForWalls()
    {
        var hit = collisionDetection.CheckForWalls(velocity.x);
        if (hit != null)
        {
            velocity.x = 0;
            var hitHealth = hit.Value.collider.gameObject.GetComponent<Health>();
            if (isCharging && hitHealth != null) {
                hitHealth.TakeDamage(1);
            }
            StopCharging();
        }
        return hit;
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
        var playerBottom = player.GetComponent<BoxCollider2D>().bounds.min;
        var playerTop = player.GetComponent<BoxCollider2D>().bounds.max;
        RaycastHit2D bottomHit = Physics2D.Raycast(
            transform.position,
            playerBottom - transform.position,
            visionRange,
            visionBlockingLayerMask
        );
        RaycastHit2D topHit = Physics2D.Raycast(
            transform.position,
            playerTop - transform.position,
            visionRange,
            visionBlockingLayerMask
        );

        if (topHit.collider != null || bottomHit.collider != null) {
            if (topHit.collider.gameObject.CompareTag("Player"))
            {
                Debug.DrawLine(transform.position, playerTop, Color.red);
                return true;
            }
            if (bottomHit.collider.gameObject.CompareTag("Player"))
            {
                Debug.DrawLine(transform.position, playerBottom, Color.red);
                return true;
            }
            Debug.DrawLine(transform.position, topHit.point, Color.green);
            Debug.DrawLine(transform.position, bottomHit.point, Color.green);
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    // Update is called once per frame
    void Update()
    {
        var groundHits = CheckForGround();
        if (isAlive)
        {
            var wallHits = CheckForWalls();
            var ceilingHits = CheckForCeiling();
            CanSeePlayer();

            if (CanSeePlayer() && playerController.isAlive)
            {
                ChargePlayer();
            }

            new List<RaycastHit2D?> { groundHits, wallHits, ceilingHits }.ForEach(hit =>
            {
                if (hit != null && hit.Value.collider.gameObject.GetComponent<Health>() != null) 
                {
                    hit.Value.collider.gameObject.GetComponent<Health>().TakeDamage(1);
                }
            });
        }
        animator.SetBool("is_charging", isCharging);
        timeSinceLastChargeEnd += Time.deltaTime;
    }

    void FixedUpdate()
    {
        ApplyGravity();
        ApplyVelocity();
    }
}
