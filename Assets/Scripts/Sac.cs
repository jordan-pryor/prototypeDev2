using UnityEngine;

public class Sac : MonoBehaviour
{
    [SerializeField] GameObject trap;
    [SerializeField] float timerStart = 3f;
    [SerializeField] float damage = 5f;
    float timer = 3f;
    private void Start()
    {
        timer = timerStart;
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Vector3 spawnPos = transform.position;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
            {
                spawnPos = hit.point;
            }
            Instantiate(trap, spawnPos, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamage dmg)) dmg.TakeDamage(damage);
        Vector3 spawnPos = transform.position;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
        {
            spawnPos = hit.point;
        }
    }
}
