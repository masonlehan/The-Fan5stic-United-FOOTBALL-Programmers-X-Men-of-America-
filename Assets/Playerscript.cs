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
        // I'm in hell (Dashing) - Bill
        if (direction.x > 0)
        {
            facingDirection = 1f;
        }
        else if (direction.x < 0)
        {
            facingDirection = -1f;
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
        else
        {
            // Keeps your exact movement running smoothly (Is this ai? - Bill)
            // moved this here because I felt like it (I had to to get dashing working lmao) - Billy <3 
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
        if (onGround == true)
        {
            // Normal Jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
        }
        else if (canSuitcaseDoubleJump == true)
        {
            // Suitcase Double Jump Power!
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, SuitcaseJumpForce);
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
