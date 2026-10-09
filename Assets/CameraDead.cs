using UnityEngine;

public class CameraDeadZone : MonoBehaviour
{
    Transform player;
    public float deadZone = 2f;

    void Start()
    {
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.transform;
    }

    void Update()
    {
        Vector3 cameraPosition = transform.position;
        float playerX = player.position.x;

        if (playerX > cameraPosition.x + deadZone)
        {
            cameraPosition.x = playerX - deadZone;
        }
        else if (playerX < cameraPosition.x - deadZone)
        {
            cameraPosition.x = playerX + deadZone;
        }

        transform.position = cameraPosition;
    }
}