using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    private float rotateOffset = 180f;

    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private float shotDelay = 0.15f;
    [SerializeField] private float damage = 30f;
    private float nextShot;

    [SerializeField] private int maxAmmo = 24;
    public int currenAmmo;

    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private AudioManager audioManager;

    [Header("Mobile Controls")]
    [SerializeField] private Button fireButton;
    [SerializeField] private Button reloadButton;

    void Start()
    {
        currenAmmo = maxAmmo;
        UpdateAmmoText();

        if (fireButton != null)
            fireButton.onClick.AddListener(TryShoot);

        if (reloadButton != null)
            reloadButton.onClick.AddListener(ReloadGun);
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        RotateGun_PC();
        Shoot_PC();
        Reload_PC();
#endif
    }

    // ================= PC INPUT (NEW INPUT SYSTEM) =================

    void RotateGun_PC()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldMouse =
            Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));

        Vector3 displacement = transform.position - worldMouse;
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);
        HandleFlip(angle);
    }

    void Shoot_PC()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            FireBullet();
        }
    }

    void Reload_PC()
    {
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            ReloadGun();
        }
    }

    // ================= MOBILE INPUT =================

    public void TryShoot()
    {
        if (currenAmmo <= 0 || Time.time < nextShot) return;

        Enemy nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector3 displacement = transform.position - nearestEnemy.transform.position;
            float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);
            HandleFlip(angle);
        }

        FireBullet();
    }

    // ================= COMMON =================

    void FireBullet()
    {
        if (currenAmmo <= 0 || Time.time < nextShot) return;

        nextShot = Time.time + shotDelay;

        GameObject bullet = Instantiate(bulletPrefabs, firePos.position, firePos.rotation);

        // 🔥 TRUYỀN DAMAGE TỪ GUN SANG BULLET
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        if (playerBullet != null)
        {
            playerBullet.SetDamage(damage);
        }

        currenAmmo--;

        UpdateAmmoText();
        audioManager?.PlayShootSound();
    }


    void ReloadGun()
    {
        if (currenAmmo >= maxAmmo) return;

        currenAmmo = maxAmmo;
        UpdateAmmoText();
        audioManager?.PlayReloadSound();
    }

    void HandleFlip(float angle)
    {
        if (angle < -90 || angle > 90)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(1, -1, 1);
    }

    Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Enemy e in enemies)
        {
            float dist = Vector3.Distance(e.transform.position, transform.position);
            if (dist < minDistance)
            {
                nearest = e;
                minDistance = dist;
            }
        }
        return nearest;
    }

    void UpdateAmmoText()
    {
        if (ammoText == null) return;
        ammoText.text = currenAmmo > 0 ? currenAmmo.ToString() : "EMPTY";
    }
    public float GetDamage()
    {
        return damage;
    }
    public void SetDamage(float value)
    {
        damage = value;
    }
    public void IncreaseDamage(float amount)
    {
        damage += amount;
    }
}
