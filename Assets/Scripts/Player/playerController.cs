using NUnit;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, ITrap
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private camController camControl;
    public Animator anim;
    private Coroutine healRoutine;
    public Inventory inv;
    public TMP_Text goalText;

    [Header("Movement Options")]
    [SerializeField] private float speedCrouch = 2.5f;
    [SerializeField] private float speedWalk = 5f;
    [SerializeField] private float speedSprint = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float drag = 1.1f;
    //[SerializeField] private bool toggleSprint = false;
    private Vector2 moveInput;
    public bool isSprinting;
    public bool isMoving;
    [SerializeField] private float moveForce = 40f;

    [Header("Jump Options")]
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float jumpCooldown = 1f;
    //[SerializeField] private int jumpCount = 1;
    private bool canJump = true;
    private bool jumpInput;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;

    [Header("Crouch")]
    [SerializeField] private float stealthAmount = 100f;
    private bool isCrouching;
    public float currentStealth;

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

    // Update is called once per frame
    private void Update()
    {
        CheckInput();
    }
    private void FixedUpdate()
    {
        GroundCheck();
        Movement();
        CheckAnimation();
        currentStealth = LitCheck() * (isCrouching ? stealthAmount : 50f);
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
        if ( fire || Input.GetButtonDown("Reload"))
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
        if (isGrounded)
        {
            isJumping = false;
        }
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
        if (jumpInput && isGrounded && canJump)
        {
            Jump();
        }
    }
    private float LitCheck()
    {
        float lightLevel = LightLevelManager.instance.GetLightLevel(transform.position);
        return Mathf.Clamp01(1f - lightLevel);
    }
    private void Crouch(bool state)
    {
        isCrouching = state;
    }
    private void Jump()
    {
        isJumping = true;
        anim.SetTrigger("isJumping");
        canJump = false;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        StartCoroutine(ResetJump(jumpCooldown));
    }
    public void SwitchCam( bool newisFPS)
    {
        isFPS = newisFPS;
        camControl.ToggleCam();
    }
    IEnumerator ITrap.trap(float speedDecrease, int duration)
    {
        if (isTrapped) yield break; // prevent stacking traps
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
        // Here for now
        //GameManager.instance.playerHPBar.fillAmount = (float)HP / maxHP;
    }
}
