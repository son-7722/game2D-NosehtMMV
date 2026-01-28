using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float lifeTime = 0.5f;
    [SerializeField] private float damage;
    [SerializeField] private GameObject bloodPrefabs;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Di chuyển bằng physics
    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (bloodPrefabs != null)
                {
                    GameObject blood = Instantiate(
                        bloodPrefabs,
                        transform.position,
                        Quaternion.identity
                    );
                    Destroy(blood, 1f);
                }
            }

            Destroy(gameObject);
        }
    }
    public void SetDamage(float value)
    {
        damage = value;
    }
}
