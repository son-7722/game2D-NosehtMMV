using UnityEngine;

public class EnergyEnemy : Enemy
{
    [SerializeField] private GameObject energyObject;
    [SerializeField] private float energyDropChance = 0.5f; // 50%

    [Header("Damage Settings")]
    [SerializeField] private float stayDamageInterval = 1f;
    private float nextDamageTime = 0f;

    protected override void Start()
    {
        base.Start();
        scoreValue = 5;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && player != null)
        {
            player.TakeDamage(enterDamage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || player == null) return;

        if (Time.time >= nextDamageTime)
        {
            player.TakeDamage(stayDamage);
            nextDamageTime = Time.time + stayDamageInterval;
        }
    }

    protected override void Die()
    {
        DropEnergy();
        base.Die();
    }

    private void DropEnergy()
    {
        if (energyObject != null && Random.value <= energyDropChance)
        {
            GameObject energy = Instantiate(energyObject, transform.position, Quaternion.identity);
            Destroy(energy, 5f);
        }
    }
}
