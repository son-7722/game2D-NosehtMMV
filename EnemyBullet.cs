using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float damage = 10f;

    private Vector2 moveDir;
    private bool isBossBullet = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }

    public void SetDirection(Vector2 direction)
    {
        moveDir = direction.normalized;
    }

    public void SetBossBullet(bool value)
    {
        isBossBullet = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            if (!isBossBullet)
            {
                Destroy(gameObject);
            }
        }
    }
}
