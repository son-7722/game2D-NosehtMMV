using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Joystick joystick;

    private Rigidbody2D  rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField] private float maxHp = 100f;
    private float currentHp;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameManager gameManager;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        currentHp = maxHp;
        UpdateHpBar();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.PauseGameMenu();
        }
    }
    void MovePlayer()
    {
        Vector2 playerInput;
        if (joystick != null && joystick.Direction.magnitude > 0.1f)
        {
            playerInput = joystick.Direction;
        }
        else
        {
            playerInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
        }

        rb.linearVelocity = playerInput.normalized * moveSpeed;

        if (playerInput.x < -0.1f)
        {
           spriteRenderer.flipX = true;
        }
        else if (playerInput.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        animator.SetBool("isRun", playerInput.magnitude > 0.1f);
        // Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // rb.linearVelocity = playerInput.normalized * moveSpeed;
        // if(playerInput.x < 0)
        // {
        //     spriteRenderer.flipX = true;
        // }else if (playerInput.x > 0)
        // {
        //     spriteRenderer.flipX = false;
        // }
        // if (playerInput != Vector2.zero)
        // {
        //     animator.SetBool("isRun", true);
        // }else
        // {
        //     animator.SetBool("isRun", false);
        // }
    }
     public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        if (currentHp <= 0)
        {
            Die();
        }
    }
    public void Heal(float healValue)
    {
        if (currentHp < maxHp)
        {
            currentHp += healValue;
            currentHp = Mathf.Min(currentHp, maxHp);
            UpdateHpBar();
        }
    }
    public virtual void Die()
    {
        gameManager.GameOverMenu();
    }
    public void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = currentHp / maxHp;
        }
    }
}
