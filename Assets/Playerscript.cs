using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 direction;
    bool onGround;
    private HashSet<Collider2D> groundContacts = new HashSet<Collider2D>();
    private Collider2D playerCollider;

    [Header("Movement Settings")]
    public float Speed;
    public float JumpForce;

    [Header("Jump Timing Settings")]
    public float CoyoteTime = 0.15f;
    public float JumpBufferTime = 0.15f;
    private float coyoteTimeCounter = 0f;
    private float jumpBufferCounter = 0f;

    [Header("Dash Settings")]
    public float DashSpeed = 20f;
    public float DashDuration = 0.15f;
    public TrailRenderer dashTrail;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float facingDirection = 1f;
    private bool canDash = true;
    private bool dashJumpMomentum = false;

    [Header("Dash Jump Settings")]
    public float DashJumpAcceleration = 20f;
    public float DashJumpDeceleration = 30f;
    public float DashJumpMaxSpeed = 20f;
    

    [Header ("Bounce Settings")]
    public float SlamSpeed = 25f;
    public float BounceHeight = 22f;
    public float SlamDeceleration = 2f;
    [Range(0f, 1f)]
    public float BounceDeceleration = 0.2f;
    public TrailRenderer slamTrail;

    private bool isSlamming = false;
    private bool canSlam = true;


    [Header("Wall Slide Settings")]
    public float wallCheckDistance = 0.6f;
    public LayerMask groundLayer;
    public float wallSlideSpeed = 3f;
    public float wallSlideFallThreshold = -0.5f;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private int wallDirection = 0;

    [Header("Wall Jump Settings")]
    public float WallJumpHorizontalForce = 12f;
    public float WallJumpVerticalForce = 16f;
    public float WallJumpControlLock = 0.2f;
    private bool isWallJumping = false;
    private float wallJumpTimer = 0f;
    private int wallJumpAwayDirection = 0;
    public float WallJumpBufferTime = 0.15f;
    private float wallJumpBufferCounter = 0f;

    [Header("Suitcase Settings")]
    public Transform suitcaseTransform; // Drag your BCase object here in the inspector
    //private bool canSuitcaseDoubleJump = true;



    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       playerCollider = GetComponent<Collider2D>();
    }

    
    
    void Update()
    {
        CheckWall();

        if (wallJumpBufferCounter > 0f)
        {
            wallJumpBufferCounter -= Time.deltaTime;
            if (isTouchingWall && !onGround && !isDashing && !isSlamming && !isWallJumping)
            {
                WallJump();
                wallJumpBufferCounter = 0f;
            }
        }
        
        if (onGround && rb.linearVelocity.y <= 0.01f)
        {
            coyoteTimeCounter = CoyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        if (jumpBufferCounter > 0f)
        {
            if (onGround && !isDashing && !isSlamming)
            {
                PerformGroundJump();
            }
            
            else if (!onGround && coyoteTimeCounter > 0f && !isSlamming)
            {
                if (isDashing)
                {
                    isDashing = false;
                    
                    if (dashTrail != null) dashTrail.emitting = false;
                    dashJumpMomentum = true;
                }
                PerformGroundJump();
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }
        }
        
        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
        
        else
        {
            isWallJumping = false;
        }
        
        if (direction.x != 0 && !dashJumpMomentum)
        {
            facingDirection = Mathf.Sign(direction.x);
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            
            if (dashTimer <= 0f)
            {
                isDashing = false;

                if (dashTrail != null)
                {
                    dashTrail.emitting = false;
                }
            }
        }
        else if (isSlamming)
        {
            float newX = Mathf.MoveTowards(rb.linearVelocity.x, 0f, SlamDeceleration * Time.deltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }
        
        else if (dashJumpMomentum)
        {
            float input = direction.x;
            if (input != 0 && Mathf.Sign(input) != Mathf.Sign(rb.linearVelocity.x))
            {
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0f, DashJumpDeceleration * Time.deltaTime),rb.linearVelocity.y);
                
                if (Mathf.Abs(rb.linearVelocity.x) < 0.1f)
                {
                    rb.linearVelocity = new Vector2(0f,rb.linearVelocity.y);
                }
            }
            
            else if (input != 0)
            {
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x,input * DashJumpMaxSpeed, DashJumpAcceleration * Time.deltaTime), rb.linearVelocity.y);
            }
        }   
    
        else if (isWallJumping)
        {
            rb.linearVelocity = new Vector2( wallJumpAwayDirection * WallJumpHorizontalForce, rb.linearVelocity.y);
        }
        
        else
        {
            rb.linearVelocity = new Vector2(direction.x * Speed, rb.linearVelocity.y);
        }
        
        
        isWallSliding = isTouchingWall && !onGround && rb.linearVelocity.y < wallSlideFallThreshold && !isDashing && !isSlamming && !isWallJumping;
        
        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
    }



    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(facingDirection * DashSpeed, 0f);
        }
        
        if (isSlamming)
        {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x,-SlamSpeed);
        }
    }


    void OnMove(InputValue movement) 
    { 
        direction = movement.Get<Vector2>();
    }
    
    
    void OnJump()
    {
        // WALL JUMP BUFFER
        if (!isTouchingWall || onGround)
        {
            wallJumpBufferCounter = WallJumpBufferTime;
        }

        if (isTouchingWall && !onGround && rb.linearVelocity.y < -0.1f && !isDashing && !isSlamming && !isWallJumping)
        {
            WallJump();
            jumpBufferCounter = 0f;
            return;
        }
        
        if (isDashing && onGround)
        {
            isDashing = false;
            
            if (dashTrail != null) dashTrail.emitting = false;
            dashJumpMomentum = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
            
            onGround = false;
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
            return;
        }
        
        if (isSlamming) return;

        if (isDashing && !onGround && coyoteTimeCounter > 0f)
        {
            isDashing = false;
            if (dashTrail != null) dashTrail.emitting = false;
            
            dashJumpMomentum = true;
            PerformGroundJump();
            return;
        }
        
        if (!isDashing && (onGround || coyoteTimeCounter > 0f))
        {
            PerformGroundJump();
        }
        
        else if (!isDashing)
        {
            jumpBufferCounter = JumpBufferTime;
        }
    }

    void PerformGroundJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
        onGround = false;

        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
    }



    // LEFT SHIFT DASH DASH DASH AWAY HO HO HO - Bill
    void OnDash()
    {
        if (isDashing || (!onGround && !canDash))        
        {
            return;
        }
        
        isDashing = true;
        dashTimer = DashDuration;
        
        if (!onGround)
        {
            canDash = false;
        }
        
        if (direction.x != 0)
        {
            facingDirection = Mathf.Sign(direction.x);
        }
        
        rb.linearVelocity = new Vector2(facingDirection * DashSpeed,0f);
        
        if (dashTrail != null)
        {
            dashTrail.emitting = true;
        }
    }
    
    //I'M NOT MAKING A BOUNCE JOKE... DOWN KEY TO BOUNCE - Bill
    void OnBounce()
    {
        if (onGround || isSlamming || !canSlam) return;
        isDashing = false;

        if (dashTrail != null) dashTrail.emitting = false;
    
        dashJumpMomentum = false;
        isSlamming = true;
        canSlam = false;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x,-SlamSpeed);

        if (slamTrail != null)
        {
            slamTrail.Clear();
            slamTrail.emitting = true;
        }
    }


    void CheckWall()
    
    {
        Bounds bounds = playerCollider.bounds;
        float rightX = bounds.max.x + 0.02f;
        float leftX = bounds.min.x - 0.02f;
        float[] heights =
        {
            bounds.min.y + bounds.size.y * 0.25f, bounds.center.y, bounds.min.y + bounds.size.y * 0.75f
        };
        
        bool hitRight = false;
        bool hitLeft = false;
        
        foreach (float y in heights)
        {
            RaycastHit2D rightHit = Physics2D.Raycast(new Vector2(rightX, y), Vector2.right, wallCheckDistance, groundLayer);
            RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(leftX, y), Vector2.left, wallCheckDistance, groundLayer);
            
            if (rightHit.collider != null && rightHit.normal.x < -0.5f)
            {
                hitRight = true;
            }
            
            if (leftHit.collider != null && leftHit.normal.x > 0.5f)
            {
                hitLeft = true;
            }
        }
        
        if (hitRight && !hitLeft)
        {
            isTouchingWall = true;
            wallDirection = 1;
        }

        else if (hitLeft && !hitRight)
        {
            isTouchingWall = true;
            wallDirection = -1;
        }

        else if (hitRight && hitLeft)
        {
            isTouchingWall = true;
            wallDirection = direction.x >= 0 ? 1 : -1;
        }
        else
        {
            isTouchingWall = false;
            wallDirection = 0;
        }
    }


    void WallJump()
    {
        if (!isTouchingWall || onGround || isDashing || isSlamming)
        return;
        
        isWallSliding = false;
        isWallJumping = true;
        
        wallJumpTimer = WallJumpControlLock;
        wallJumpAwayDirection = -wallDirection;

        rb.linearVelocity = new Vector2(wallJumpAwayDirection * WallJumpHorizontalForce, WallJumpVerticalForce);
    }


    bool HasGroundContact(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);
            
            if (contact.normal.y > 0.5f)
            {
                return true;
            }
        }
        
        return false;
    }


    // Left-click or attack button triggers this automatically in the New Input System
    void OnFire()
    {
        SwingSuitcase();
    }

    void SwingSuitcase()
    {
        if (suitcaseTransform != null)
        {
            suitcaseTransform.localRotation = Quaternion.Euler(0, 0, -45f);
            Invoke("ResetSuitcaseRotation", 0.15f);
        }
    }

    void ResetSuitcaseRotation()
    {
        if (suitcaseTransform != null) 
        {
            suitcaseTransform.localRotation = Quaternion.identity;
        }
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        
        if (!HasGroundContact(collision)) return;
        
        groundContacts.Add(collision.collider);
        onGround = groundContacts.Count > 0;
        canDash = true;
        dashJumpMomentum = false;
        
        if (isSlamming)
        {
            isSlamming = false;
            
            if (slamTrail != null)
            slamTrail.emitting = false;
            
            float bouncedX = rb.linearVelocity.x * (1f - BounceDeceleration);
            
            rb.linearVelocity = new Vector2(bouncedX, BounceHeight);
            canSlam = true;
        }
        
        else
        {
            canSlam = true;
        }
    }
    

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        
        if (HasGroundContact(collision))
        {
            groundContacts.Add(collision.collider);
        }
        
        else
        {
            groundContacts.Remove(collision.collider);
        }
        
        onGround = groundContacts.Count > 0;
        }
        
    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        groundContacts.Remove(collision.collider);
        onGround = groundContacts.Count > 0;
    }
}