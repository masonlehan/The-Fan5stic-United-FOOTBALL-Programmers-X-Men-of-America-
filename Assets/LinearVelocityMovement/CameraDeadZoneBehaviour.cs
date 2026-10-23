using UnityEngine;

public class CameraDeadZoneBehaviour : MonoBehaviour
{
    // The object that the camera will track.
    public GameObject TrackingObject;

    // The dead zone for the camera.
    // The dead zone is the area around the tracking object where the camera will not move.
    public float DeadZone;

    // Update is called once per frame
    void Update()
    {
        // Get the x position of the tracking object.
        float trackingObjectPositionX = TrackingObject.transform.position.x;

        // Get the position of the camera.
        Vector3 cameraPosition = gameObject.transform.position;

        // If the distance is greater than the dead zone, move the camera towards the tracking object.
        if (trackingObjectPositionX > cameraPosition.x + DeadZone)
        {
            // Adjust the camera position to follow the tracking object within the dead zone.
            cameraPosition.x = trackingObjectPositionX - DeadZone;

            // Set the new camera position.
            gameObject.transform.position = cameraPosition;
        }

        // If the distance is less than the dead zone, move the camera towards the tracking object.
        if (trackingObjectPositionX < cameraPosition.x - DeadZone)
        {
            // Adjust the camera position to follow the tracking object within the dead zone.
            cameraPosition.x = trackingObjectPositionX + DeadZone;

            // Set the new camera position.
            gameObject.transform.position = cameraPosition;
        }
    }
}

 