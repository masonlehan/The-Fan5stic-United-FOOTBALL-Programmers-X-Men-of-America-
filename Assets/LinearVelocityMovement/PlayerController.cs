using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // We set this in Unity.
    public float Speed;
    public float jumpSpeed;

    Rigidbody2D rb;
    bool onGround;

    // Private property to store a reference to the PlayerInput component.
    PlayerInput pi;

    Animator anim;

    SpriteRenderer sr;

    Vector2 direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pi = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Sent by PlayerInput for the "Move" action (Value / Vector2).
    void OnMove(InputValue inputMovement)
    {
        direction = inputMovement.Get<Vector2>();
    }

    void OnJump()
    {
        // Set the up (Y) velocity to the jump speed.
        // Keep the current left/right (X) velocity so we don't stop horizontal movement.
        if (onGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
            onGround = false;
        }
    }

    // Update is called once per frame.
    void Update()
    {
        InputAction moveAction = pi.actions["Move"];

        // Read how much left/right movement is being requested.
        Vector2 movementAmount = moveAction.ReadValue<Vector2>();

        // Are we moving positively or negatively along the X axis and about to flip the character's facing direction?
        if (movementAmount.x > 0)
        {
            sr.flipX = false;
        }
        else if (movementAmount.x < 0)
        {
            sr.flipX = true;
        }

        // Set the left/right (X) velocity, but keep the current up/down (Y) velocity
        // so gravity and jumping still work.
        rb.linearVelocity = new Vector2(movementAmount.x * Speed, rb.linearVelocity.y);
        
        // Set the "Speed" parameter in the Animator to the horizontal movement amount.
        // We use the absolute value so the speed is always positive, regardless of direction.
        anim.SetFloat("Speed", Mathf.Abs(movementAmount.x));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Access the GameObject the collider is attached to.
        GameObject go = collision.gameObject;

        // Does the GameObject we collided with have the tag "Ground"?
        if (go.tag == "Ground")
        {
            // Yes, we are on the ground.
            onGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Access the GameObject the collider is attached to.
        GameObject go = collision.gameObject;

        // Does the GameObject we collided with have the tag "Ground"?
        if (go.tag == "Ground")
        {
            // Yes, we were on the ground.
            onGround = false;
        }
    }

}
