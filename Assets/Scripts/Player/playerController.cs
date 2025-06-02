using NUnit;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamage, ITrap
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;                         // Player Rigidbody
    [SerializeField] private Transform groundCheckPoint;           // Point for checking ground contact
    [SerializeField] private camController camControl;             // Camera script
    public Animator anim;                                          // Player animator
    private Coroutine healRoutine;                                 // Coroutine for healing
    public Inventory inv;                                          // Inventory system
    public TMP_Text goalText;                                      // Text showing goal progress
    public Sound footsteps;                                        // Footstep sound prefab
    public Sound jump;                                             // Jump sound prefab
    public Sound hurt;                                             // Jump sound prefab
    public Image playerHPBar;                                      // HP bar UI image

    [Header("Movement Options")]
    [SerializeField] private float speedCrouch = 2.5f;
    [SerializeField] private float speedWalk = 5f;
    [SerializeField] private float speedSprint = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float drag = 1.1f;
    private Vector2 moveInput;
    public bool isSprinting;
    public bool isMoving;
    [SerializeField] private float moveForce = 40f;

    [Header("Jump Options")]
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float jumpCooldown = 1f;
    private bool canJump = true;
    private bool jumpInput;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundMask;
    public bool isGrounded;

    [Header("Crouch")]
    [SerializeField] private float stealthAmount = 100f;
    private bool isCrouching;
    public float currentStealth;
    public float smell = 0f;

    [Header("Stats")]
    public float maxHP = 100f;
    public float HP = 100f;

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

    [Header("Flags")]
    public bool isTrapped;
    private float trapDecrease = 0f;
    public bool isJumping;

    public bool isCrouch = false;
    public bool lockJump = false;
    internal int ventCount;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawn = GameObject.Find("PlayerSpawn");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
            transform.rotation = spawn.transform.rotation;
        }
    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void UpdateSmell(float amt)
    {
        smell += amt;
    }
    private void Update()
    {
        CheckInput(); // Handle player input
        ventCheck();
    }
    private void FixedUpdate()
    {
        if (!GameManager.instance.isPaused)
        {
            Movement();           // Apply movement force
            CheckAnimation();     // Update animation booleans
            currentStealth = LitCheck() * (isCrouching ? stealthAmount : 50f); // Stealth based on lighting
        }
        GroundCheck();        // Detect if grounded
    }
    private void ventCheck()
    {
        if (ventCount >= 1) 
        {
            lockJump = true;
            isCrouch = true;
            if (isCrouching != true)
            {
                Crouch(true);
            }
        }
        else
        {
            lockJump = false;
            isCrouch = false;
        }
    }
    private void CheckInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        isMoving = moveInput != Vector2.zero;
        jumpInput = Input.GetButton("Jump");
        isSprinting = Input.GetButton("Sprint");

        if (Input.GetKeyDown(KeyCode.LeftControl)) Crouch(true);
        else if (Input.GetKeyUp(KeyCode.LeftControl)) Crouch(false);

        bool fire = Input.GetButtonDown("Fire1");
        anim.SetBool("isWatch", Input.GetButton("Fire2"));

        if (fire || Input.GetButtonDown("Reload"))
        {
            if (inv.equipIndex != -1)
            {
                GameObject equipped = inv.slots[inv.equipIndex];
                if (equipped != null && equipped.TryGetComponent(out IUse item))
                {
                    item.Use(fire);
                }
            }
        }
    }
    public void Step()
    {
        UpdateSmell(1f);
        Instantiate(footsteps, transform.position, transform.rotation); // Play footstep sound
    }
    private void CheckAnimation()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isGrounded", isGrounded);
    }
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundMask);
        rb.linearDamping = isGrounded ? drag : 0f;
        if (isGrounded) isJumping = false;
    }
    private IEnumerator ResetJump(float duration)
    {
        yield return new WaitForSeconds(duration);
        canJump = true;
    }
    private void Movement()
    {
        Vector3 moveDir = Camera.main.transform.forward * moveInput.y + Camera.main.transform.right * moveInput.x;
        moveDir.y = 0f;
        moveDir.Normalize();

        float currentSpeed = isCrouching ? speedCrouch : (isSprinting ? speedSprint : speedWalk);
        float targetSpeed = currentSpeed * (isTrapped ? trapDecrease : 1);
        Vector3 desiredVelocity = moveDir * targetSpeed;

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        Vector3 velocityChange = desiredVelocity - horizontalVelocity;
        velocityChange = Vector3.ClampMagnitude(velocityChange, acceleration);

        rb.AddForce(velocityChange * moveForce, ForceMode.Force);

        float animBlend = horizontalVelocity.magnitude / speedSprint;
        anim.SetFloat("Speed", animBlend);

        if (jumpInput && isGrounded && canJump && !lockJump)
        {
            Jump();
        }
    }
    private float LitCheck()
    {
        float lightLevel = LightManager.instance.GetLightLevel(transform.position);
        return Mathf.Clamp01(1f - lightLevel); // Invert to represent how hidden you are
    }
    public void Crouch(bool state)
    {
        if ( isCrouch )
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = state;
        }
    }
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
    public void SwitchCam(bool newisFPS)
    {
        if(isFPS != newisFPS)
        {
            isFPS = newisFPS;
            camControl.ToggleCam();
        }
    }
    public void trapTrigger(float speedDecrease, float duration)
    {
        // start the coroutine with the arguments
        StartCoroutine(TrapRoutine(speedDecrease, duration));
    }
    IEnumerator TrapRoutine(float speedDecrease, float duration)
    {
        Debug.Log("trapped");
        if (isTrapped) yield break;
        isTrapped = true;

        GameManager.instance.promptTrap.SetActive(true);
        trapDecrease = speedDecrease;

        yield return new WaitForSeconds(duration);

        GameManager.instance.promptTrap.SetActive(false);
        isTrapped = false;
    }
    public void TakeDamage(float amount)
    {
        HP -= amount;
        Instantiate(hurt, transform.position, transform.rotation);
        UpdatePlayerUI();

        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }
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
    public void UpdatePlayerUI()
    {
        playerHPBar.fillAmount = HP / maxHP;
    }
}
