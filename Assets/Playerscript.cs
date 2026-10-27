using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 direction;
    bool onGround;
    private HashSet<Collider2D> groundContacts = new HashSet<Collider2D>();

    [Header("Movement Settings")]
    public float Speed;
    public float JumpForce;
    //public float SuitcaseJumpForce = 16f; // Extra high boost for jumping off the suitcase!

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
    public Transform wallCheck;
    public float wallCheckDistance = 0.6f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 3f;
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

    [Header("Suitcase Settings")]
    public Transform suitcaseTransform; // Drag your BCase object here in the inspector
    //private bool canSuitcaseDoubleJump = true;



    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
    }

    
    
    void Update()
    {
        CheckWall();
        
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
        
        isWallSliding = isTouchingWall && !onGround && rb.linearVelocity.y <= 0 && !isDashing && !isSlamming && !isWallJumping;
        
        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
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
        if (isWallSliding)
        {
            WallJump();
            return;
        }



        if (isDashing && onGround)
        {
            isDashing = false;
            
            if (dashTrail != null)
            {
                dashTrail.emitting = false;
            }
            
            dashJumpMomentum = true;
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,JumpForce);
            onGround = false;
            //canSuitcaseDoubleJump = true;
            return;
        }
        if (isDashing) return;
        
        if (onGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,JumpForce);
        }
        
        //else if (canSuitcaseDoubleJump)
        //{
            //rb.linearVelocity = new Vector2(rb.linearVelocity.x,SuitcaseJumpForce);
            //canSuitcaseDoubleJump = false;
            //Debug.Log("Suitcase Double Jump Activated!");
        //}
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
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);
        
        if (hitRight.collider != null && direction.x > 0)
        {
            isTouchingWall = true;
            wallDirection = 1;
        }
        
        else if (hitLeft.collider != null && direction.x < 0)
        {
            isTouchingWall = true;
            wallDirection = -1;
        }
        
        else
        {
            isTouchingWall = false;
            wallDirection = 0;
        }
    }


    void WallJump()
    {
        if (!isWallSliding || isDashing || isSlamming) return;
        
        isWallSliding = false;
        isWallJumping = true;
        wallJumpTimer = WallJumpControlLock;
        wallJumpAwayDirection = -wallDirection;
        
        rb.linearVelocity = new Vector2 (wallJumpAwayDirection * WallJumpHorizontalForce, WallJumpVerticalForce);
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
        
        groundContacts.Add(collision.collider);
        onGround = groundContacts.Count > 0;
        canDash = true;
        dashJumpMomentum = false;
        
        if (isSlamming)
        {
            isSlamming = false;
            
            if (slamTrail != null) slamTrail.emitting = false;

        float bouncedX = rb.linearVelocity.x * (1f - BounceDeceleration);
        rb.linearVelocity = new Vector2(bouncedX, BounceHeight);
        canSlam = true;
        }
        
        else
        {
        canSlam = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        groundContacts.Remove(collision.collider);
        onGround = groundContacts.Count > 0;
    }

}
