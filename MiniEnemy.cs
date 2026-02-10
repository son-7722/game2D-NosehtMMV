using UnityEngine;

public class MiniEnemy : Enemy
{
    [SerializeField] private float damageInterval = 0.5f;
    private float nextDamageTime = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && player != null)
        {
            player.TakeDamage(enterDamage);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && player != null)
        {
            if (Time.time >= nextDamageTime)
            {
                player.TakeDamage(stayDamage);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}
