using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private float lifeTime = 0.3f;

    private bool hasDamaged = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime); // Auto destroy
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasDamaged) return;

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                hasDamaged = true;
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                hasDamaged = true;
            }
        }
    }
}
