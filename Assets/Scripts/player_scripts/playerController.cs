using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
	// === System References ===
	[Header("System References")]
	[SerializeField] Rigidbody rb;                        // Our physics-powered body � the meat mech
	[SerializeField] CapsuleCollider capsule;             // The collision jellybean we live in
	[SerializeField] Transform cam;                       // Our eyeballs � aka the camera
	[SerializeField] LayerMask groundMask;                // What counts as "floor" to us
	[SerializeField] float playerHeight = 2f;             // Tall mode � regular standing height
	[SerializeField] float crouchHeight = 1f;             // Short king mode � crouch collider height
	[SerializeField] float crouchTransitionSpeed = 10f;   // How fast we go from tall to small
	[SerializeField] int HP = 100;             // How much life we got left � don�t let it hit 0!
	int origHP;

	// === Player Movement & Control ===
	[Header("Player Movement & Control")]
	[SerializeField] float moveForce = 40f;                   // Strong enough for responsive force-based movement
	[SerializeField] float maxSpeed = 7f;                     // General cap – tweak if you want sprint to go over this

	[SerializeField] float acceleration = 10f;                // How fast we ramp to speed on ground
	[SerializeField] float deceleration = 12f;                // Slightly stronger than accel to help stop cleanly
	[SerializeField] float airControlMultiplier = 0.4f;       // Less control mid-air, but still maneuverable

	[SerializeField] float jumpForce = 8f;                    // Height of jump – feels snappy but not floaty
	[SerializeField] float coyoteTime = 0.2f;                 // Common value, very forgiving
	[SerializeField] float jumpBufferTime = 0.15f;            // Helps you land consistent jumps

	[SerializeField] float walkSpeed = 3f;                    // Slow pace, immersive movement
	[SerializeField] float runSpeed = 5f;                     // Standard movement speed
	[SerializeField] float sprintSpeed = 8f;                  // Fast and flowy
	[SerializeField] float crouchSpeed = 2.5f;                // Slow but not crawling
	[SerializeField] float slideSpeed = 10f;                  // Initial burst when initiating a slide
	[SerializeField] float slideForce = 20f;                  // Push into the slide – lower = stickier, higher = zippier
	[SerializeField] float slideDuration = 0.75f;             // Long enough for fun movement, short enough to not abuse
	[SerializeField] float slideCooldown = 1.2f;              // Prevents chaining slides unrealistically
	[SerializeField] float slideFriction = 3f;                // Higher value = quicker slowdown during slide
	[SerializeField] float slideAngleThreshold = 30f;         // Starts auto-sliding down steeper slopes

    [SerializeField] bool toggleSprint = false;               // Players choose whether they hold to sprint or toggle
    public int sensX = 75;
    public int sensY = 75;
    public int pitchClamp = 55;                           // Max head turn up/down
    public int yawClamp = 80;
	public float turnThreshold = 0.9f;
	public int turnSpeed = 90;
	public bool isFPS = false;

    //[SerializeField] AnimationCurve vaultCurve;         // (Planned) Smooth vault motion � parkour vibes incoming

    // === Gravity & Physics ===
    [Header("Gravity & Physics")]
	[SerializeField] float gravity;                       // The pull of the void � always down
	[SerializeField] float gravityMultiplier;             // How much harder we get yoinked down
	[SerializeField] float dragGround;                    // Sticky shoes � resistance on the ground
	[SerializeField] float dragAir;                       // Floating resistance � like pushing through soup
	[SerializeField] float slopeLimit = 45f;              // Max slope we can climb � beyond this, it�s a slidey slope
	[SerializeField] float slopeForceMultiplier = 2f;     // Extra push when fighting or sliding down slopes

	// === State Check / Flags ===
	[Header("State Check / Flags")]
	[SerializeField] bool isGrounded;                     // Feet on the floor? Let�s find out
	[SerializeField] bool isSprinting;                    // Are we zooming?
	[SerializeField] bool isCrouching;                    // Tiny mode active?
	[SerializeField] bool isSliding;                      // Currently surfing the floor?
	[SerializeField] bool canVault;                       // Ready to hop over something?
	[SerializeField] bool wasGroundedLastFrame;           // Were we grounded a moment ago?
	[SerializeField] bool jumpQueued;                     // Jump was pressed � just waiting for the perfect moment
	[SerializeField] bool isMoving;                       // Are we actually going somewhere?
	[SerializeField] bool hasJumped;                      // Already jumped? Can�t double up unless allowed
	[SerializeField] bool vaulting;                       // Mid-vault animation/action?
	[SerializeField] bool sprintToggled;				  // If sprinting has been toggled

	// === Movement Vectors ===
	[Header("Movement Vectors")]
	Vector3 moveInput;                                    // Raw input from player � where we wanna go
	Vector3 moveDirection;                                // Where we�re *actually* headed
	Vector3 desiredVelocity;                              // Our dream velocity
	Vector3 currentVelocity;                              // Current real-world velocity
	Vector3 velocityChange;                               // Delta between desired and current
	Vector3 forceToApply;                                 // Final force we give to physics
	Vector3 momentum;                                     // Built-up speed from past movement
	Vector3 externalForces;                               // Any outside shoves � wind? explosions? ghosts?

	Vector3 groundNormal;                                 // What's �up� from the ground beneath us
	Vector3 slopeDirection;                               // Which way the hill is tilting
	Vector3 slideDirection;                               // Where we go when we start slippin�

	Vector3 vaultTargetPosition;                          // Where we�re headed when vaulting
	Vector3 ledgeCheckOrigin;                             // Where we start looking for a ledge
	Vector3 ledgeHitNormal;                               // The angle of the ledge we found

	Vector3 raycastOrigin;                                // Ray start point � detective work begins here
	Vector3 raycastDirection;                             // Ray direction � where we�re looking

	// === Player Interaction(Weapons) ===
	[SerializeField] private GameObject startingWeaponPrefab;
	[SerializeField] private Transform itemHolder;
	[SerializeField] GameObject equippedItem;
    [SerializeField] Weapon equippedWeapon;
	

	// === Timers ===
	[Header("Timers")]
	float lastGroundedTime;                               // Last time we touched the ground � for coyote logic
	float lastJumpPressedTime;                            // Last jump input time � for buffering
	float lastVaultTime;                                  // Last time we vaulted � cooldown tracking?
	float slideTimer;                                     // How long we�ve been sliding
	float slideCooldownTimer;                             // Cooldown between slides � chill out

	// === Player State Machine ===
	public enum MovementState { Idle, Walk, Run, Sprint, Crouch, Slide, Vault }  // All the ways we move
	MovementState currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origHP = HP;
    }

    void Update()
    {
        if(equippedWeapon != null)
        {
            isFPS = true;
            if (Input.GetButton("Fire1"))
            {
                equippedWeapon.Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                equippedWeapon.Reload();
            }
        }
        else
        {
            isFPS = false;
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.2f, groundMask);
        movement();
        sprint();
        crouch();
    }

    void movement()
    {
        // === 1. Get Input ===
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(horizontal, 0f, vertical).normalized;

        // === 2. Translate Input to World Space ===
        moveDirection = cam.forward * moveInput.z + cam.right * moveInput.x;
        moveDirection.y = 0f;
        moveDirection.Normalize();

        // === 3. Choose Target Speed Based on State ===
        float targetSpeed = walkSpeed;
        if (isSprinting) targetSpeed = sprintSpeed;
        if (isCrouching) targetSpeed = crouchSpeed;

        // === 4. Calculate Desired Velocity ===
        desiredVelocity = moveDirection * targetSpeed;

        // === 5. Get Current Horizontal Velocity ===
        currentVelocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        // === 6. Figure Out the Difference ===
        velocityChange = desiredVelocity - horizontalVelocity;

        // === 7. Clamp Velocity Change Based on Acceleration ===
        float controlFactor = isGrounded ? 1f : airControlMultiplier;
        velocityChange = Vector3.ClampMagnitude(velocityChange, acceleration * controlFactor);

        // === 8. Apply That Force to the Meat Mech ===
        forceToApply = velocityChange * moveForce;
        rb.AddForce(forceToApply, ForceMode.Force);

        // === 9. Update Movement Flag ===
        isMoving = moveInput.magnitude > 0f;
        if (!isMoving && toggleSprint) { sprintToggled = false; }
    }

    void sprint()
    {
        if (toggleSprint)
        {
            if (Input.GetButtonDown("Sprint"))
            {
                sprintToggled = !sprintToggled;
            }
            if (sprintToggled && isGrounded && isMoving && !isCrouching && !isSliding)
            {
                isSprinting = true;
                currentState = MovementState.Sprint;
            }
            else
            {
                isSprinting = false;
                currentState = isMoving ? MovementState.Run : MovementState.Idle;
            }
        }
        else
        {
            if (Input.GetButton("Sprint") && isGrounded && isMoving && !isCrouching && !isSliding)
            {
                isSprinting = true;
                currentState = MovementState.Sprint;
            }
            else
            {
                isSprinting = false;
                currentState = isMoving ? MovementState.Run : MovementState.Idle;
            }
        }
    }

    void crouch()
    {

    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }
}