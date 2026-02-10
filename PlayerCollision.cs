using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private float enemyBulletDamage = 10f;

    private Player player;

    private void Awake()
    {
        // Cache Player để không GetComponent liên tục
        player = GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            if (player != null)
            {
                player.TakeDamage(enemyBulletDamage);
            }
        }
        else if (collision.CompareTag("Usb"))
        {
            if (gameManager != null)
            {
                gameManager.AddUsb();
            }
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Energy"))
        {
            if (gameManager != null)
            {
                gameManager.AddEnergy();
            }

            if (audioManager != null)
            {
                audioManager.PlayEnergySound();
            }

            Destroy(collision.gameObject);
        }
    }
}
