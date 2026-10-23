using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject carPrefab;         // Drop your OncomingCar asset here!
    public Transform spawnPoint;         // Where the cars appear on the right
    public float spawnInterval = 3f;     // Shoots a car every 3 seconds

    private bool canSpawn = false;
    private float timer = 0f;

    // The BusController calls this automatically when the player hops inside!
    public void StartSpawning()
    {
        canSpawn = true;
        timer = spawnInterval; // Instantly ready to spawn the first car
        Debug.Log("Obstacle Spawner Activated!");
    }

    void Update()
    {
        if (!canSpawn) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnVehicle();
            timer = 0f;
        }
    }

    void SpawnVehicle()
    {
        if (carPrefab != null && spawnPoint != null)
        {
            // Creates a fresh copy of your car at the off-screen spawn point
            GameObject newCar = Instantiate(carPrefab, spawnPoint.position, Quaternion.identity);
            
            // Make sure the spawned copy has the correct tag
            newCar.tag = "Vehicle";
        }
    }
}
