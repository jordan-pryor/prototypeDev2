using UnityEngine;
public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    float spawnTimer;
    int spawnCount;
    bool startSpawning;
    [SerializeField] bool spawnOnStart = false;
    void Start()
    {
        GameManager.instance.updateGameGoal(numToSpawn);
        if (spawnOnStart && startSpawning != true)
        {
            startSpawning = true;
        }
    }
    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnCount < numToSpawn && spawnTimer >= timeBetweenSpawns)
            {
                spawn();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && startSpawning != true)
        {
            startSpawning = true;
        }
    }
    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);
        Instantiate(objectToSpawn, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        spawnCount++;
        spawnTimer = 0;
    }
}