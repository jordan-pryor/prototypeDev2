using UnityEngine;

public class playerController : MonoBehaviour
{
	// === System References ===
	[Header("System References")]
	[SerializeField] Rigidbody rb;                        // Our physics-powered body – the meat mech
	[SerializeField] CapsuleCollider capsule;             // The collision jellybean we live in
	[SerializeField] Transform cam;                       // Our eyeballs – aka the camera
	[SerializeField] LayerMask groundMask;                // What counts as "floor" to us
	[SerializeField] float playerHeight = 2f;             // Tall mode – regular standing height
	[SerializeField] float crouchHeight = 1f;             // Short king mode – crouch collider height
	[SerializeField] float crouchTransitionSpeed = 10f;   // How fast we go from tall to small

	// === Player Movement & Control ===
	[Header("Player Movement & Control")]
	[SerializeField] float moveForce;                     // How hard we push ourselves when moving
	[SerializeField] float maxSpeed;                      // Speed cap – no going full Sonic
	[SerializeField] float acceleration;                  // How quick we ramp up to speed
	[SerializeField] float deceleration;                  // How quick we chill back down
	[SerializeField] float airControlMultiplier;          // Air steering power – less rocket, more bird

	[SerializeField] float jumpForce;                     // The yeet power – how high we bounce
	[SerializeField] float coyoteTime = 0.2f;             // Forgiveness window after leaving ground – classic cartoon logic
	[SerializeField] float jumpBufferTime = 0.15f;        // Buffer for jump input – we’re generous like that

	[SerializeField] float walkSpeed;                     // Casual stroll pace – just vibes
	[SerializeField] float runSpeed;                      // Hustle mode – not too fast, not too slow
	[SerializeField] float sprintSpeed;                   // Full send – speed demon time
	[SerializeField] float crouchSpeed;                   // Sneaky sneak pace – quiet but slow
	[SerializeField] float slideSpeed;                    // Initial speed burst when we slide
	[SerializeField] float slideForce;                    // The shove we get when we dive into a slide
	[SerializeField] float slideDuration = 0.75f;         // How long we keep sliding before giving up
	[SerializeField] float slideCooldown = 1f;            // Gotta rest after sliding – no spam zone
	[SerializeField] float slideFriction = 1f;            // How much we slow down mid-slide
	[SerializeField] float slideAngleThreshold = 30f;     // Minimum slope angle for a slide to go turbo

	//[SerializeField] AnimationCurve vaultCurve;         // (Planned) Smooth vault motion – parkour vibes incoming

	// === Gravity & Physics ===
	[Header("Gravity & Physics")]
	[SerializeField] float gravity;                       // The pull of the void – always down
	[SerializeField] float gravityMultiplier;             // How much harder we get yoinked down
	[SerializeField] float dragGround;                    // Sticky shoes – resistance on the ground
	[SerializeField] float dragAir;                       // Floating resistance – like pushing through soup
	[SerializeField] float slopeLimit = 45f;              // Max slope we can climb – beyond this, it’s a slidey slope
	[SerializeField] float slopeForceMultiplier = 2f;     // Extra push when fighting or sliding down slopes

	// === State Check / Flags ===
	[Header("State Check / Flags")]
	[SerializeField] bool isGrounded;                     // Feet on the floor? Let’s find out
	[SerializeField] bool isSprinting;                    // Are we zooming?
	[SerializeField] bool isCrouching;                    // Tiny mode active?
	[SerializeField] bool isSliding;                      // Currently surfing the floor?
	[SerializeField] bool canVault;                       // Ready to hop over something?
	[SerializeField] bool wasGroundedLastFrame;           // Were we grounded a moment ago?
	[SerializeField] bool jumpQueued;                     // Jump was pressed – just waiting for the perfect moment
	[SerializeField] bool isMoving;                       // Are we actually going somewhere?
	[SerializeField] bool hasJumped;                      // Already jumped? Can’t double up unless allowed
	[SerializeField] bool vaulting;                       // Mid-vault animation/action?

	// === Movement Vectors ===
	[Header("Movement Vectors")]
	Vector3 moveInput;                                    // Raw input from player – where we wanna go
	Vector3 moveDirection;                                // Where we’re *actually* headed
	Vector3 desiredVelocity;                              // Our dream velocity
	Vector3 currentVelocity;                              // Current real-world velocity
	Vector3 velocityChange;                               // Delta between desired and current
	Vector3 forceToApply;                                 // Final force we give to physics
	Vector3 momentum;                                     // Built-up speed from past movement
	Vector3 externalForces;                               // Any outside shoves – wind? explosions? ghosts?

	Vector3 groundNormal;                                 // What's “up” from the ground beneath us
	Vector3 slopeDirection;                               // Which way the hill is tilting
	Vector3 slideDirection;                               // Where we go when we start slippin’

	Vector3 vaultTargetPosition;                          // Where we’re headed when vaulting
	Vector3 ledgeCheckOrigin;                             // Where we start looking for a ledge
	Vector3 ledgeHitNormal;                               // The angle of the ledge we found

	Vector3 raycastOrigin;                                // Ray start point – detective work begins here
	Vector3 raycastDirection;                             // Ray direction – where we’re looking

	// === Timers ===
	[Header("Timers")]
	float lastGroundedTime;                               // Last time we touched the ground – for coyote logic
	float lastJumpPressedTime;                            // Last jump input time – for buffering
	float lastVaultTime;                                  // Last time we vaulted – cooldown tracking?
	float slideTimer;                                     // How long we’ve been sliding
	float slideCooldownTimer;                             // Cooldown between slides – chill out

	// === Player State Machine ===
	public enum MovementState { Idle, Walk, Run, Sprint, Crouch, Slide, Vault }  // All the ways we move
	MovementState currentState;                      


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
