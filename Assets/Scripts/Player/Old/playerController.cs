using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class playerController : MonoBehaviour//, IDamage, ITrap
{
    //OLD CODE

    /*
    // === System References ===
    [Header("System References")]
    [SerializeField] Rigidbody rb;                         // Our physics-powered body — the meat mech
    [SerializeField] CapsuleCollider capsule;              // The collision jellybean we live in
    [SerializeField] Transform cam;                        // Our eyeballs — aka the camera
    [SerializeField] LayerMask groundMask;                 // What counts as "floor" to us
    [SerializeField] float playerHeight = 2f;              // Tall mode — regular standing height
    //[SerializeField] float crouchHeight = 1f;            // Short king mode — crouch collider height
    //[SerializeField] float crouchTransitionSpeed = 10f;  // How fast we go from tall to small
    [SerializeField] public int HP = 100;                  // How much life we got left — don’t let it hit 0!
    int origHP;

    // === Player Movement & Control ===
    [Header("Player Movement & Control")]
    [SerializeField] float moveForce = 40f;                // Strong enough for responsive force-based movement
    [SerializeField] float maxSpeed = 7f;                  // General cap – tweak if you want sprint to go over this
    [SerializeField] float acceleration = 10f;             // How fast we ramp to speed on ground
    [SerializeField] float deceleration = 12f;             // Slightly stronger than accel to help stop cleanly
    [SerializeField] float airControlMultiplier = 0.4f;    // Less control mid-air, but still maneuverable
    [SerializeField] float walkSpeed = 3f;                 // Slow pace, immersive movement
    [SerializeField] float runSpeed = 5f;                  // Standard movement speed
    [SerializeField] float sprintSpeed = 8f;               // Fast and flowy
    //[SerializeField] float crouchSpeed = 2.5f;           // Slow but not crawling
    //[SerializeField] float slideSpeed = 10f;             // Initial burst when initiating a slide
    //[SerializeField] float slideForce = 20f;             // Push into the slide – lower = stickier, higher = zippier
    //[SerializeField] float slideDuration = 0.75f;        // Long enough for fun movement, short enough to not abuse
    //[SerializeField] float slideCooldown = 1.2f;         // Prevents chaining slides unrealistically
    //[SerializeField] float slideFriction = 3f;           // Higher value = quicker slowdown during slide
    //[SerializeField] float slideAngleThreshold = 30f;    // Starts auto-sliding down steeper slopes

    // Cache for trap
    float origWalkSpeed;
    float origRunSpeed;
    float origSprintSpeed;
    [SerializeField] bool toggleSprint = false;            // Players choose whether they hold to sprint or toggle
    public int sensX = 75;
    public int sensY = 75;
    public int pitchClamp = 55;                            // Max head turn up/down
    public int yawClamp = 80;
    public float turnThreshold = 0.45f;
    public float stationaryThreshold = 0.9f;
    public int turnSpeed = 90;
    public bool isFPS = false;
    //[SerializeField] AnimationCurve vaultCurve;          // (Planned) Smooth vault motion — parkour vibes incoming

    // === Gravity & Physics ===
    [Header("Gravity & Physics")]
    //[SerializeField] float gravity;                      // The pull of the void — always down
    [SerializeField] float gravityMultiplier;             // How much harder we get yoinked down
    [SerializeField] float dragGround;                    // Sticky shoes — resistance on the ground
    [SerializeField] float dragAir;                       // Floating resistance — like pushing through soup
    //[SerializeField] float slopeLimit = 45f;             // Max slope we can climb — beyond this, it’s a slidey slope
    //[SerializeField] float slopeForceMultiplier = 2f;    // Extra push when fighting or sliding down slopes

    // === State Check / Flags ===
    [Header("State Check / Flags")]
    bool isGrounded;                                      // Feet on the floor? Let’s find out
    public bool isSprinting;                              // Are we zooming?
    bool isCrouching;                                     // Tiny mode active?
    [SerializeField] bool isSliding;                      // Currently surfing the floor?
    [SerializeField] bool canVault;                       // Ready to hop over something?
    bool wasGroundedLastFrame;                            // Were we grounded a moment ago?
    [SerializeField] bool jumpQueued;                     // Jump was pressed — just waiting for the perfect moment
    public bool isMoving;                                 // Are we actually going somewhere?
    bool hasJumped;                                       // Already jumped? Can’t double up unless allowed
    [SerializeField] bool vaulting;                       // Mid-vault animation/action?
    [SerializeField] bool sprintToggled;                  // If sprinting has been toggled
    bool isTrapped = false;                               // Are we slowed by a trap?

    // === Movement Vectors ===
    [Header("Movement Vectors")]
    Vector3 moveInput;              // Raw input from player — where we wanna go
    Vector3 moveDirection;          // Where we’re *actually* headed
    Vector3 desiredVelocity;        // Our dream velocity
    Vector3 currentVelocity;        // Current real-world velocity
    Vector3 velocityChange;         // Delta between desired and current
    Vector3 forceToApply;           // Final force we give to physics
    Vector3 momentum;               // Built-up speed from past movement
    Vector3 externalForces;         // Any outside shoves — wind? explosions? ghosts?
    Vector3 groundNormal;           // What's “up” from the ground beneath us
    Vector3 slopeDirection;         // Which way the hill is tilting
    Vector3 slideDirection;         // Where we go when we start slippin’
    Vector3 vaultTargetPosition;    // Where we’re headed when vaulting
    Vector3 ledgeCheckOrigin;       // Where we start looking for a ledge
    Vector3 ledgeHitNormal;         // The angle of the ledge we found
    Vector3 raycastOrigin;          // Ray start point — detective work begins here
    Vector3 raycastDirection;       // Ray direction — where we’re looking

    // === Player Interaction (Weapons) ===
    [Header("Player Interaction")]
    [SerializeField] private GameObject startingWeaponPrefab;
    [SerializeField] private Transform itemHolder;
    [SerializeField] GameObject equippedItem;
    public Weapon equippedWeapon;

    // === Jumping ===
    [Header("Jumping")]
    [SerializeField] float jumpForce = 8f;
    [SerializeField] int maxJumpCount = 1;
    int currentJumpCount;
    bool jumpRequested;

    // === Timers ===
    //[Header("Timers")]
    //float lastGroundedTime;       // Last time we touched the ground — for coyote logic
    //float lastJumpPressedTime;    // Last jump input time — for buffering
    //float lastVaultTime;          // Last time we vaulted — cooldown tracking?
    //float slideTimer;             // How long we’ve been sliding
    //float slideCooldownTimer;     // Cooldown between slides — chill out
    // === Player State Machine ===
    public enum MovementState { Idle, Walk, Run, Sprint, Crouch, Slide, Vault }  // All the ways we move
	MovementState currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origRunSpeed = runSpeed;
        origWalkSpeed = walkSpeed;
        origSprintSpeed = sprintSpeed;
        oldupdatePlayerUI();
    }
    public void PickUpWeapon(Weapon item)
    {
        equippedWeapon = item;
        item.transform.SetParent(itemHolder);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        Collider col = item.GetComponent<Collider>();
        col.enabled = false;
    }
    void Update()
    {
        if(equippedWeapon != null)
        {
            isFPS = true;
            if (Input.GetButtonDown("Fire1"))
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

        // Check for jump input
        if (Input.GetButtonDown("Jump") && currentJumpCount < maxJumpCount)
        {
            hasJumped = true;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset Y velocity for clean jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            currentJumpCount++;
        }
    }

    void FixedUpdate()
    {
        Vector3 bottomCenter = capsule.bounds.center - new Vector3(0, capsule.bounds.extents.y, 0) + Vector3.up * 0.05f;
        float checkRadius = capsule.radius * 0.95f;

        isGrounded = Physics.CheckSphere(bottomCenter, checkRadius, groundMask);

        if (isGrounded && !wasGroundedLastFrame)
        {
            currentJumpCount = 0; // Reset jump count when touching ground
            hasJumped = false;
        }

        wasGroundedLastFrame = isGrounded;

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
        //if (isCrouching) targetSpeed = crouchSpeed;

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

    public void TakeDamage(int amount)
    {
        HP -= amount;
        oldupdatePlayerUI();

        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }

    public IEnumerator trap(float speedMult, int duration)
    {
        if (isTrapped) yield break; // prevent stacking traps
        isTrapped = true;
        GameManager.instance.promptTrap.SetActive(true);
        float originalWalk = walkSpeed;
        float originalSprint = sprintSpeed;
        walkSpeed *= speedMult;
        sprintSpeed *= speedMult;
        // crouchSpeed *= speedMult;
        yield return new WaitForSeconds(duration);
        walkSpeed = originalWalk;
        sprintSpeed = originalSprint;
        // crouchSpeed = origCrouchSpeed;
        GameManager.instance.promptTrap.SetActive(false);
        isTrapped = false;
    }

    public void oldupdatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / origHP;
    }
*/
}
