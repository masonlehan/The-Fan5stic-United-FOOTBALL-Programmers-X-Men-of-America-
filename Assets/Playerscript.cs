using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{

Rigidbody2D rb;

Vector2 direction;

public float Speed;

public float JumpForce;
bool onGround;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(direction.x * Speed, rb.linearVelocity.y);
    }

void OnMove(InputValue movement)
{
direction = movement.Get<Vector2>();
}

void OnJump()
{
    if(onGround == true)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
    }
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