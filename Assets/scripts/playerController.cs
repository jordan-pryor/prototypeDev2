using UnityEngine;

public class playerController : MonoBehaviour
{
	[SerializeField] LayerMask ignoreLayer;
	[SerializeField] CharacterController controller;

	[SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpVelocity;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    float shootTimer;
	int jumpCount;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    bool isSprinting;

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

		movement();

		sprint();

	}
    void movement()
    {
	    if (controller.isGrounded)
	    {
		    jumpCount = 0;
		    playerVelocity = Vector3.zero;
	    }

	    //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	    moveDirection = (Input.GetAxis("Horizontal") * transform.right) +
	              (Input.GetAxis("Vertical") * transform.forward);
	    //transform.position += moveDir * speed * Time.deltaTime;


	    controller.Move(moveDirection * speed * Time.deltaTime);

	    jump();

	    playerVelocity.y -= gravity * Time.deltaTime;
	    controller.Move(playerVelocity * speed * Time.deltaTime);

	    shootTimer += Time.deltaTime;

	    if (Input.GetButton("Fire1") && shootTimer >= shootRate)
	    {
		    shoot();
	    }

    }

    void jump()
    {
	    if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
	    {
		    jumpCount++;
		    playerVelocity.y = jumpVelocity;
	    }
    }

    void sprint()
    {
	    if (Input.GetButtonDown("Sprint"))
	    {
		    speed *= sprintMod;
	    }
	    else if (Input.GetButtonUp("Sprint"))
	    {
		    speed /= sprintMod;
	    }
    }

    void shoot()
    {
	    shootTimer = 0;

	    RaycastHit hit;
	    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
	    {
		    Debug.Log(hit.collider.name);

			iDamage dmg = hit.collider.GetComponent<iDamage>();

		    if (dmg != null)
		    {
			    dmg.takeDamage(shootDamage);
		    }
	    }
    }
}
