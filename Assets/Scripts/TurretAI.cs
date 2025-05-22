using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TurretAI : MonoBehaviour, IDamage
{
    [Header("-----Components------")]
    [SerializeField] Renderer model;
    [SerializeField] Transform turretEyePosition;
    [SerializeField] GameObject bullet;

    [Header("-----Stats-----")]
    [Range(1,3)][SerializeField] float HP;
    [Range(1,10)][SerializeField] int faceTargetSpeed;
    [Range(45,180)][SerializeField] int FOVangle;
    [Range(2,10)][SerializeField] int shootRate;
    

    float shootTimer;
    bool PlayerInRange;
    float angleToPlayer;

    Color colorOrig;
    Vector3 playerDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerInRange && canSeePlayer())
        {
            if(shootTimer >= shootRate)
            {
                shoot();
            }
        }
    }

    bool canSeePlayer()
    {
        playerDirection = (GameManager.instance.player.transform.position - turretEyePosition.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z), transform.forward);

        RaycastHit hit;
        if(Physics.Raycast(turretEyePosition.position, playerDirection, out hit))
        {
            if(hit.collider.CompareTag("Player") && angleToPlayer <= FOVangle)
            {
                faceTarget();

                shootTimer += Time.deltaTime;

                return true;
            }
        }

        return false;
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, turretEyePosition.position, transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
        }
    }

    public void TakeDamage(float amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if(HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, playerDirection.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
