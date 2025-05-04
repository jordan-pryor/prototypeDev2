using UnityEngine;
public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;      // Object prefab to spawn
    [SerializeField] int numToSpawn;                // Total number of spawns
    [SerializeField] int timeBetweenSpawns;         // Delay between each spawn
    [SerializeField] Transform[] spawnPos;          // Array of possible spawn positions

    float spawnTimer;
    int spawnCount;
    bool startSpawning;

    [SerializeField] bool spawnOnStart = false;     // Option to auto-start spawning at scene start

    void Start()
    {
        GameManager.instance.updateGameGoal(numToSpawn);  // Notify game of spawn goal
        if (spawnOnStart && !startSpawning)
        {
            startSpawning = true;  // Begin spawning if enabled
        }
    }

    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;

            // Spawn next object if delay met and limit not reached
            if (spawnCount < numToSpawn && spawnTimer >= timeBetweenSpawns)
            {
                spawn();
            }
        }
    }

    // Triggered by player entering the spawner area
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !startSpawning)
        {
            startSpawning = true;  // Start spawning sequence
        }
    }

    // Spawn logic: pick random position and instantiate object
    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);
        Instantiate(objectToSpawn, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        spawnCount++;
        spawnTimer = 0f;
    }
}