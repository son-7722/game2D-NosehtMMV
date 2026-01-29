
using UnityEngine;

public class BossEnemy : Enemy
{
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float speedDanThuong = 20f;
    [SerializeField] private float speedDanVongTron = 10f;
    [SerializeField] private float hpValue = 100f;
    [SerializeField] private GameObject miniEnemy;
    [SerializeField] private float skillCooldown = 2f;
    private float nextSkillTime = 0f;
    [SerializeField] private GameObject usbPrefabs;

    protected override void Start()
    {
        base.Start();
        scoreValue = 20;

        if (gameManager != null)
        {
            float multiplier = gameManager.GetDifficultyMultiplier();
            hpValue *= multiplier;
            speedDanThuong *= multiplier;
            speedDanVongTron *= multiplier;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (Time.time >= nextSkillTime)
        {
            SuDungSkill();
        }
    }
    protected override void Die()
    {
        if (gameManager != null)
        {
            gameManager.OnBossKilled();
        }
        Instantiate(usbPrefabs, transform.position, Quaternion.identity);
        base.Die(); // Handles Score and Destroy
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(enterDamage);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(stayDamage);
        }
    }
    private void BanDanThuong()
    {
        if (player != null)
        {
            Vector2 direction =
                (player.transform.position - firePoint.position).normalized;

            GameObject bullet =
                Instantiate(bulletPrefabs, firePoint.position, Quaternion.identity);

            EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
            if (enemyBullet != null)
            {
                enemyBullet.SetDirection(direction);
                enemyBullet.SetBossBullet(true);
            }
        }
    }



    private void BanDanVongTron()
    {
        const int bulletCount = 12;
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            GameObject bullet =
                Instantiate(bulletPrefabs, transform.position, Quaternion.identity);

            EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
            if (enemyBullet != null)
            {
                enemyBullet.SetDirection(dir);
                enemyBullet.SetBossBullet(true); 
            }
        }
    }


    private void HoiMau(float hpAmount)
    {
        currentHp = Mathf.Min(currentHp + hpAmount, maxHp);
        UpdateHpBar();
    }
    private void SinhMiniEnemy()
    {
        Instantiate(miniEnemy, transform.position, Quaternion.identity);
    }
    private void DichChuyen()
    {
        transform.position = player.transform.position;
    }
    private void ChonSkillNgauNhien()
    {
        int randomSkill = Random.Range(0, 5);
        switch (randomSkill)
        {
            case 0:
                BanDanThuong();
                break;
            case 1:
                BanDanVongTron();
                break;
            case 2:
                HoiMau(hpValue);
                break;
            case 3:
                SinhMiniEnemy();
                break;
            case 4:
                DichChuyen();
                break;
        }
    }
    private void SuDungSkill()
    {
        nextSkillTime = Time.time + skillCooldown;
        ChonSkillNgauNhien();
    }
}
