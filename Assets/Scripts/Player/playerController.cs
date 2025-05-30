using NUnit;
using System.Collections;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamage, ITrap
{
    // --- 🧩 CORE REFERENCES ---
    [Header("References")]
    [SerializeField] private Rigidbody rb;                         // Our physics meat suit
    [SerializeField] private Transform groundCheckPoint;           // Is the floor... lava?
    [SerializeField] private camController camControl;             // Camera overlord
    public Animator anim;                                          // Jiggly meat puppet animator
    private Coroutine healRoutine;                                 // Medic!
    public Inventory inv;                                          // Backpack of holding
    public TMP_Text goalText;                                      // "Do the Thing" text
    public Sound footsteps;                                        // Clap stomp walk sounds
    public Sound jump;                                             // Dramatic whoosh
    public Image playerHPBar;                                      // UI bar of life

    [Header("UI")]
    [SerializeField] private CrosshairUI crosshairUI;              // Fancy cross lines

    // --- 🏃 MOVEMENT VARIABLES ---
    [Header("Movement Options")]
    [SerializeField] private float speedCrouch = 2.5f;
    [SerializeField] private float speedWalk = 5f;
    [SerializeField] private float speedSprint = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float drag = 1.1f;
    [SerializeField] private float moveForce = 40f;
    private Vector2 moveInput;
    public bool isSprinting;
    public bool isMoving;

    // --- 🚀 JUMPING ---
    [Header("Jump Options")]
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float jumpCooldown = 1f;
    private bool canJump = true;
    private bool jumpInput;

    // --- 🧱 GROUND DETECTION ---
    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundMask;
    public bool isGrounded;
    public bool isJumping;

    // --- 🐱‍👤 CROUCHING / STEALTH ---
    [Header("Crouch")]
    [SerializeField] private float stealthAmount = 100f;
    private bool isCrouching;
    public float currentStealth;
    public float smell = 0f; // Yes, you stink. It’s stealth-related.

    // --- ❤️ PLAYER STATS ---
    [Header("Stats")]
    public float maxHP = 100f;
    public float HP = 100f;

    // --- 🎥 CAMERA SETTINGS ---
    [Header("Camera Options")]
    public bool isFPS = false;
    public int sensX = 75;
    public int sensY = 75;
    public int pitchClamp = 60;
    public int yawClamp = 80;
    public float turnThreshold = 0.45f;
    public float stationaryThreshold = 0.9f;
    public float turnSpeed = 90f;
    public float sprintTurnMod = 45f;

    // --- 🚨 GAMEPLAY FLAGS ---
    [Header("Flags")]
    public bool isTrapped;
    private float trapDecrease = 0f;
    public bool crosshairOn;

    // --- 🎮 MONO METHODS ---
    private void Start()
    {
        // Is the crosshair a lie? Let's check saved preferences.
        if (crosshairUI != null)
        {
            crosshairOn = PlayerPrefs.GetInt("crosshairOn", 1) == 1;
            crosshairUI.SetVisible(crosshairOn);
        }
    }

    private void Update()
    {
        CheckInput(); // Because pressing buttons is important
    }

    private void FixedUpdate()
    {
        GroundCheck();                         // Are we airborne or not?
        Movement();                            // Walking, sprinting, skating
        CheckAnimation();                      // Make the puppet dance
        currentStealth = LitCheck() * (isCrouching ? stealthAmount : 50f); // If hiding in shadows, stealth++
    }

    // --- 🧠 INPUT HANDLER ---
    private void CheckInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        isMoving = moveInput != Vector2.zero;
        jumpInput = Input.GetButton("Jump");
        isSprinting = Input.GetButton("Sprint");

        if (Input.GetKeyDown(KeyCode.LeftControl)) Crouch(true);
        else if (Input.GetKeyUp(KeyCode.LeftControl)) Crouch(false);

        anim.SetBool("isWatch", Input.GetButton("Fire2")); // For dramatic aim-down-watch scenes

        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Reload"))
        {
            if (inv.equipIndex != -1)
            {
                GameObject equipped = inv.slots[inv.equipIndex];
                if (equipped != null && equipped.TryGetComponent(out IUse item))
                {
                    item.Use(Input.GetButtonDown("Fire1"));
                }
            }
        }
    }

    // --- 🐾 MOVE + ANIMATION ---
    private void Movement()
    {
        Vector3 moveDir = Camera.main.transform.forward * moveInput.y + Camera.main.transform.right * moveInput.x;
        moveDir.y = 0f; moveDir.Normalize();

        float currentSpeed = isCrouching ? speedCrouch : (isSprinting ? speedSprint : speedWalk);
        float targetSpeed = currentSpeed * (isTrapped ? trapDecrease : 1f);
        Vector3 desiredVelocity = moveDir * targetSpeed;

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        Vector3 velocityChange = desiredVelocity - horizontalVelocity;
        velocityChange = Vector3.ClampMagnitude(velocityChange, acceleration);

        rb.AddForce(velocityChange * moveForce, ForceMode.Force);

        float animBlend = horizontalVelocity.magnitude / speedSprint;
        anim.SetFloat("Speed", animBlend);

        if (jumpInput && isGrounded && canJump) Jump();
    }

    // --- 👁️ LIGHT + STEALTH ---
    private float LitCheck()
    {
        float lightLevel = LightLevelManager.instance.GetLightLevel(transform.position);
        return Mathf.Clamp01(1f - lightLevel); // Darkness is your friend
    }

    // --- 🧍 ANIMATOR LOGIC ---
    private void CheckAnimation()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isGrounded", isGrounded);
    }

    // --- 🧼 CROUCHING ---
    private void Crouch(bool state)
    {
        isCrouching = state;
    }

    // --- 🐸 JUMPING ---
    private void Jump()
    {
        isJumping = true;
        anim.SetTrigger("isJumping");
        canJump = false;
        UpdateSmell(5f);
        Instantiate(jump, transform.position, transform.rotation);

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        StartCoroutine(ResetJump(jumpCooldown));
    }

    private IEnumerator ResetJump(float duration)
    {
        yield return new WaitForSeconds(duration);
        canJump = true;
    }

    // --- 🦶 STEP + SMELL ---
    public void Step()
    {
        UpdateSmell(1f);
        Instantiate(footsteps, transform.position, transform.rotation);
    }

    public void UpdateSmell(float amt)
    {
        smell += amt;
    }

    // --- 🌍 GROUND DETECTION ---
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundMask);
        rb.linearDamping = isGrounded ? drag : 0f;
        if (isGrounded) isJumping = false;
    }

    // --- 📸 CAM SWITCHER ---
    public void SwitchCam(bool newisFPS)
    {
        isFPS = newisFPS;
        camControl.ToggleCam(); // Toggle between selfie mode and cinematic
    }

    // --- 🪤 TRAP INTERFACE ---
    IEnumerator ITrap.trap(float speedDecrease, float duration)
    {
        if (isTrapped) yield break;

        isTrapped = true;
        GameManager.instance.promptTrap.SetActive(true);
        trapDecrease = speedDecrease;

        yield return new WaitForSeconds(duration);

        GameManager.instance.promptTrap.SetActive(false);
        isTrapped = false;
    }

    // --- 💔 DAMAGE SYSTEM ---
    public void TakeDamage(float amount)
    {
        HP -= amount;
        UpdatePlayerUI();

        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }

    // --- 🩹 HEALING ---
    public void StartHealing(float healAmt, int healCount, float healInterval)
    {
        if (healRoutine != null)
            StopCoroutine(healRoutine);

        healRoutine = StartCoroutine(HealOverTime(healAmt, healCount, healInterval));
    }

    private IEnumerator HealOverTime(float healAmt, int healCount, float healInterval)
    {
        while (healCount > 0 && HP < maxHP)
        {
            while (GameManager.instance.isPaused)
                yield return null;

            yield return new WaitForSeconds(healInterval);

            float prevHP = HP;
            HP = Mathf.Min(HP + healAmt, maxHP);
            healCount--;

            if (HP != prevHP)
                UpdatePlayerUI();
        }

        healRoutine = null;
    }

    // --- 🎯 UI UPDATE ---
    public void UpdatePlayerUI()
    {
        playerHPBar.fillAmount = HP / maxHP;
    }
}
