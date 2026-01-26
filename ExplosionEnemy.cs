using UnityEngine;

public class ExplosionEnemy : Enemy
{
    [SerializeField] private GameObject explosionPrefabs;
    private bool hasExploded = false;

    protected override void Start()
    {
        base.Start();
        scoreValue = 7;
    }

    private void CreateExplosion()
    {
        if (hasExploded) return;

        hasExploded = true;

        if (explosionPrefabs != null)
        {
            Instantiate(explosionPrefabs, transform.position, Quaternion.identity);
        }
    }

    protected override void Die()
    {
        CreateExplosion();
        base.Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasExploded)
        {
            player?.TakeDamage(enterDamage);
            Die(); // Nổ + destroy enemy
        }
    }
}
