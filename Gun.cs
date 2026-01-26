using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    private float rotateOffset = 180f;
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private float shotDelay = 0.15f;
    private float nextShot;
    [SerializeField] private int maxAmmo = 24;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private AudioManager audioManager;
    
    [Header("Mobile Controls")]
    [SerializeField] private Button fireButton;
    [SerializeField] private Button reloadButton;

    public int currenAmmo;
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
        // DISABLE PC CONTROLS FOR TESTING
        // if (!Application.isMobilePlatform)
        // {
        //     RotateGun();
        //     Shoot();
        //     Reload();
        // }
    }
    void RotateGun()
    {
        // Disable mouse rotation if using touch/mobile might conflict, 
        // but for now keeping it simple. Only rotate if mouse usage is detected or on PC.
        if(Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            return;
        }
        Vector3 displacement = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(displacement.y,displacement.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle + rotateOffset);
        HandleFlip(angle);
    }

    void HandleFlip(float angle)
    {
        if(angle < -90 || angle >90)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.localScale = new Vector3(1,-1,1);
        }
    }

    void Shoot()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FireBullet();
        }
    }

    // Called by Mobile Input
    public void TryShoot()
    {
        if (currenAmmo > 0 && Time.time > nextShot)
        {
            // Auto-aim logic
            Enemy nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                Vector3 direction = transform.position - nearestEnemy.transform.position; // Direction from Enemy to Gun? Wait. 
                // Original logic: transform.position - Camera... (displacement)
                // We want gun to look AT enemy.
                // Original: displacement = transform.position - Mouse.
                // angle = Atan2(y, x).
                // transform.rotation = angle + 180 (rotateOffset).
                // So gun faces AWAY from the vector? If displacement is (Gun - Target), vector points to Gun.
                // 180 flip makes it point to Target. Correct.

                Vector3 displacement = transform.position - nearestEnemy.transform.position;
                float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);
                HandleFlip(angle);
            }
            
            FireBullet();
        }
    }

    private void FireBullet()
    {
        if (currenAmmo > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + shotDelay;
            Instantiate(bulletPrefabs, firePos.position, firePos.rotation);
            currenAmmo --;
            UpdateAmmoText();
            audioManager.PlayShootSound();
        }
    }

    Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (Enemy t in enemies)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDistance)
            {
                nearest = t;
                minDistance = dist;
            }
        }
        return nearest;
    }

    void Reload()
    {
        if(Input.GetMouseButtonDown(1))
        {
            ReloadGun();
        }
    }

    public void ReloadGun()
    {
        if(currenAmmo < maxAmmo)
        {
            currenAmmo = maxAmmo;
            UpdateAmmoText();
            audioManager.PlayReloadSound();
        }
    }
    private void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            if (currenAmmo > 0)
            {
                ammoText.text = currenAmmo.ToString();
            }
            else
            {
                ammoText.text = "EMPTY";
            }
        }
    }
}
