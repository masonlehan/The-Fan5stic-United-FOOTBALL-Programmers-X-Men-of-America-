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

    [Header("Suitcase Settings")]
    public Transform suitcaseTransform; // Drag your BCase object here in the inspector
    private bool canSuitcaseDoubleJump = true;

    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Keeps your exact movement running smoothly
        rb.linearVelocity = new Vector2(direction.x * Speed, rb.linearVelocity.y);
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
            canSuitcaseDoubleJump = true; // Resets your double jump whenever you touch down
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
