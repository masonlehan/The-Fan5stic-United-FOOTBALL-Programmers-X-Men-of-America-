using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTarget;
    public Transform busTarget;

    void LateUpdate()
    {
        // 1. If the player is active, follow the player
        if (playerTarget != null && playerTarget.gameObject.activeSelf)
        {
            transform.position = new Vector3(playerTarget.position.x, transform.position.y, -10f);
        }
        // 2. If the player is hidden (boarded), follow the bus instead!
        else if (busTarget != null)
        {
            transform.position = new Vector3(busTarget.position.x, transform.position.y, -10f);
        }
    }
}
