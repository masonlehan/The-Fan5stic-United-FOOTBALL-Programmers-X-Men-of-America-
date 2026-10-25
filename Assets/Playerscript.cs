using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerScript : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 direction;
    bool onGround;

    [Header("Movement Settings")]
    public float Speed;
    public float JumpForce;
    public float SuitcaseJumpForce = 16f; // Extra high boost for jumping off the suitcase!

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

    [Header("Suitcase Settings")]
    public Transform suitcaseTransform; // Drag your BCase object here in the inspector
    private bool canSuitcaseDoubleJump = true;



    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
    }

    
    
    void Update()
    {
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
        else if (dashJumpMomentum)
        {
            // Accelerate or decelerate based on player input
            float input = direction.x;
            if (input != 0 && Mathf.Sign(input) != Mathf.Sign(rb.linearVelocity.x))
            {
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x,0f,DashJumpDeceleration * Time.deltaTime),rb.linearVelocity.y);
                
                if (Mathf.Abs(rb.linearVelocity.x) < 0.1f)
                {
                    rb.linearVelocity = new Vector2(0f,rb.linearVelocity.y);
                }
            }
            
            else if (input != 0)
            {
                // Forward input maintains or builds momentum
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x,input * DashJumpMaxSpeed, DashJumpAcceleration * Time.deltaTime), rb.linearVelocity.y);
            }
        }   
    
        else
        {
            // Normal movement
            rb.linearVelocity = new Vector2(direction.x * Speed, rb.linearVelocity.y);
        }
    }



    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = new Vector2(facingDirection * DashSpeed, 0f);
        }
    }

    // Handles your controller/keyboard stick movement
    void OnMove(InputValue movement) 
    { 
        direction = movement.Get<Vector2>();
    }
    
    // Handles jumping using your Input System action
    
    void OnJump()
    {
        if (isDashing && onGround)
        {
            isDashing = false;
            
            if (dashTrail != null)
            {
                dashTrail.emitting = false;
            }
            
            dashJumpMomentum = true;
            
            // Jump without losing horizontal speed
            // moved this here because I felt like it (I needed to lol) - Bill
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,JumpForce);
            onGround = false;
            canSuitcaseDoubleJump = true;
            return;
        }
        if (isDashing) return;
        
        if (onGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,JumpForce);
        }
        
        else if (canSuitcaseDoubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,SuitcaseJumpForce);
            canSuitcaseDoubleJump = false;
            Debug.Log("Suitcase Double Jump Activated!");
        }
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
        if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
            canSuitcaseDoubleJump = true;
            canDash = true;
            dashJumpMomentum = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = false;
        }
    }
}
