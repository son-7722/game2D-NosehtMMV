using UnityEngine;

public class EnergyEnemy : Enemy
{
    [SerializeField] private GameObject energyObject;

    protected override void Start()
    {
        base.Start();
        scoreValue = 5;
    }

    private void OnTriggerEnter2D(Collider2D conllision)
    {
        if (conllision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(enterDamage);
            }
        }
    }
    private void OnTriggerStay2D (Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.TakeDamage(stayDamage);
            }
        }
    }
    protected override void Die()
    {
        if (energyObject != null)
        {
            GameObject energy = Instantiate(energyObject, transform.position, Quaternion.identity);
            Destroy(energy, 5f);
        }
        base.Die();
    }
}
