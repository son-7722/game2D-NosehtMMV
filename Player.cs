using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Joystick joystick;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SerializeField] private float maxHp = 100f;
    private float currentHp;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameManager gameManager;

    private float baseMoveSpeed;
    private Vector2 moveInput;

    // INPUT SYSTEM
    private InputAction moveAction;
    private InputAction pauseAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        baseMoveSpeed = moveSpeed;

        // MOVE: WASD + Arrow
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        // PAUSE: ESC
        pauseAction = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        pauseAction.Disable();
    }

    void Start()
    {
        currentHp = maxHp;
        UpdateHpBar();
    }

    public void UpdateSpeed(float multiplier)
    {
        moveSpeed = baseMoveSpeed * multiplier;
    }

    void Update()
    {
        // Ưu tiên joystick mobile
        if (joystick != null && joystick.Direction.magnitude > 0.1f)
        {
            moveInput = joystick.Direction;
        }
        else
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }

        if (pauseAction.WasPressedThisFrame())
        {
            gameManager.PauseGameMenu();
        }

        animator.SetBool("isRun", moveInput.magnitude > 0.1f);

        if (moveInput.x < -0.1f)
            spriteRenderer.flipX = true;
        else if (moveInput.x > 0.1f)
            spriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();

        if (currentHp <= 0)
            Die();
    }

    public void Heal(float healValue)
    {
        currentHp = Mathf.Min(currentHp + healValue, maxHp);
        UpdateHpBar();
    }

    public void Die()
    {
        gameManager.GameOverMenu();
    }

    private void UpdateHpBar()
    {
        if (hpBar != null)
            hpBar.fillAmount = currentHp / maxHp;
    }
}
