using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BusController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float driveSpeed = 10f;
    private bool isDriving = false;

    [Header("Audio Settings")]
    public AudioSource mySpeaker;       
    public AudioClip mySynthSound;     

    [Header("Visual Effects")]
    public GameObject headlightVisual; // Toggles your Light 2D headlight beam!

    [Header("UI Canvas Elements")]
    public GameObject driveInstructionsText;

    [Header("Spawner Settings")]
    public ObstacleSpawner levelSpawner; // Fires cars ONLY after you board!

    private bool lightsFlashing = false;
    private bool hornHonking = false;

    public void StartDrivingBus()
    {
        isDriving = true;
        Debug.Log("Bus controls activated!");
    }

    void Update()
    {
        if (!isDriving) return;

        // RIGHT ARROW: Drive Forward
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            transform.Translate(Vector2.right * driveSpeed * Time.deltaTime);
        }

        // LEFT ARROW: Flash Lights (Counters Cars)
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame && !lightsFlashing)
        {
            lightsFlashing = true;
            Debug.Log("LIGHTS FLASHING!");
            StartCoroutine(ResetLights());
        }

        // UP ARROW: Honk Horn (Counters Animals - Plays your Synth asset!)
        if (Keyboard.current.upArrowKey.wasPressedThisFrame && !hornHonking)
        {
            hornHonking = true;
            Debug.Log("HORN HONKING!");
            
            if (mySpeaker != null && mySynthSound != null)
            {
                mySpeaker.PlayOneShot(mySynthSound);
            }
            
            StartCoroutine(ResetHorn());
        }
    }

    // Handles flashing your headlight light beam on and off
    IEnumerator ResetLights()
    {
        if (headlightVisual != null) headlightVisual.SetActive(true);

        yield return new WaitForSeconds(0.6f); // Keep lights on for 0.6 seconds

        if (headlightVisual != null) headlightVisual.SetActive(false);
        lightsFlashing = false;
    }

    IEnumerator ResetHorn()
    {
        yield return new WaitForSeconds(0.6f);
        hornHonking = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Detect if the Player reached the bus to hand over controls
        if (other.CompareTag("Player"))
        {
            StartDrivingBus();
            other.gameObject.SetActive(false); 
            
            if (driveInstructionsText != null) driveInstructionsText.SetActive(true);

            // Wake up our obstacle spawner automatically!
            if (levelSpawner != null) {
                levelSpawner.StartSpawning();
            }

            return; 
        }

        if (!isDriving) return;

        // 2. Obstacle Check: Incoming Vehicles
        if (other.CompareTag("Vehicle"))
        {
            if (lightsFlashing)
            {
                Debug.Log("Success! Avoided vehicle.");
                Destroy(other.gameObject); 
            }
            else
            {
                Debug.LogError("CRASHED INTO VEHICLE!");
            }
        }
    }
}
