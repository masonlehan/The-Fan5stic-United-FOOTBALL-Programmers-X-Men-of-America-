using UnityEngine;

public class WaterPush : MonoBehaviour
{
    [Tooltip("How strong the water blast pushes the player")]
    public float pushForce = 15f; 

    // This triggers automatically whenever a particle hits a collider
    private void OnParticleCollision(GameObject other)
    {
        // Check if the object the water hit has a Rigidbody2D component
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            // Calculate a horizontal push direction (away from the hydrant source)
            Vector2 pushDirection = new Vector2(1, 0.2f).normalized;
            
            // Shove the player back!
            rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
        }
    }
}