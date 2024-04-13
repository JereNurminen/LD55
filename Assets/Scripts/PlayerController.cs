using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SelectedSummon
{
    Crow,
    Rat
}

public class PlayerController : MonoBehaviour
{
    private Vector2 velocity = Vector2.zero;
    private Vector2 velocityChange = Vector2.zero;
    public float gravity = 9.8f;
    public float runSpeed = 10.0f;
    public float jumpStrength = 100.0f;

    public float groundRaycastDistance = 1 / 8f;
    public float wallRaycastDistance = 1 / 8f;
    public int groundRaycastCount = 2;
    public int wallRaycastCount = 3;
    public LayerMask groundLayerMask;
    public float gravityImmunityAfterJump = 0.1f;

    public GameObject crowPrefab;
    public GameObject ratPrefab;

    private bool isGrounded = false;

    private BoxCollider2D boxCollider;
    private Animator animator;
    private MouseController mouseController;
    private CollisionDetection collisionDetection;
    private float timeSinceJump = 0.0f;
    public bool isAlive = true;

    private SelectedSummon selectedSummon = SelectedSummon.Crow;
    private HudController hudController;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        mouseController = GetComponent<MouseController>();
        collisionDetection = GetComponent<CollisionDetection>();
        hudController = FindAnyObjectByType<HudController>();
    }

    /* Handle Inputs */
    void HandleInputs()
    {
        if (!mouseController.isHeld)
        {
            if (Input.GetKey(KeyCode.A))
            {
                velocity.x = -runSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                velocity.x = runSpeed;
            }
            else
            {
                velocity.x = 0;
            }
        }
        else
        {
            velocity.x = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            selectedSummon =
                selectedSummon == SelectedSummon.Rat ? SelectedSummon.Crow : SelectedSummon.Rat;
            hudController.ChangeIcon(selectedSummon);
        }
    }

    void Jump()
    {
        if (isGrounded)
        {
            velocity.y = jumpStrength;
            timeSinceJump = 0.0f;
            animator.SetTrigger("jump");
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded && timeSinceJump > gravityImmunityAfterJump)
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
        }
    }

    void CheckForCeiling()
    {
        var hit = collisionDetection.CheckForCeiling();
        if (hit != null)
        {
            velocity.y = 0;
            timeSinceJump = gravityImmunityAfterJump + 0.1f;
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

    public void SummonCrow(Vector2 Start, Vector2 Target)
    {
        GameObject crow = Instantiate(crowPrefab, Start, Quaternion.identity);
        crow.GetComponent<CrowController>().Spawn(Start, Target);
    }

    public void SummonRat(Vector2 Start, Vector2 Target)
    {
        GameObject rat = Instantiate(ratPrefab, Start, Quaternion.identity);
        rat.GetComponent<RatController>().Spawn(Start, Target);
    }

    public void Summon(Vector2 Start, Vector2 Target)
    {
        switch (selectedSummon)
        {
            case SelectedSummon.Crow:
                SummonCrow(Start, Target);
                break;
            case SelectedSummon.Rat:
                SummonRat(Start, Target);
                break;
        }
    }

    public void onDeath()
    {
        isAlive = false;
        animator.SetTrigger("death");
        velocity.x = 0;
        velocity.y = Mathf.Min(0, velocity.y);
        StartCoroutine(hudController.Die());
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            HandleInputs();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload current Scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        CheckForGround();
        CheckForWalls();
        CheckForCeiling();
        animator.SetBool("running", !Mathf.Approximately(velocity.x, 0f));
        animator.SetFloat("vertical_speed", velocity.y);
        animator.SetBool("grounded", isGrounded);
        animator.SetBool("summoning", mouseController.isHeld);
    }

    void FixedUpdate()
    {
        ApplyGravity();
        ApplyVelocity();
        timeSinceJump += Time.deltaTime;
    }
}
