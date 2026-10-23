using UnityEngine;

public class IncomingVehicle : MonoBehaviour
{
    public float approachSpeed = 12f;

    void Update()
    {
        // Oncoming cars drive leftward directly into the path of your bus!
        transform.Translate(Vector2.left * approachSpeed * Time.deltaTime);
    }
}
