using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject mobileControls;

    [SerializeField] private Player player;
    [SerializeField] private Gun gun;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI usbText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI hpText;

    private int cost = 1;

    private void OnEnable()
    {
        Time.timeScale = 0f;

        if (mobileControls != null)
            mobileControls.SetActive(false);

        UpdateUI(); // ✅ mở shop là update liền
    }

    void UpdateUI()
    {
        if (gameManager != null)
            usbText.text = "USB: " + gameManager.GetUsb();

        if (gun != null)
            damageText.text = "D: " + gun.GetDamage().ToString("0");

        if (player != null)
        {
            speedText.text = "S: " + player.GetBaseMoveSpeed().ToString("0.0");
            hpText.text = $"HP: {player.GetCurrentHp():0} / {player.GetMaxHp():0}";
        }
    }

    // ================= UPGRADE =================

    public void UpgradeDamage()
    {
        if (!gameManager.SpendUsb(cost)) return;

        gun.IncreaseDamage(4f);
        UpdateUI(); // ✅ cập nhật ngay
    }

    public void UpgradeSpeed()
    {
        if (!gameManager.SpendUsb(cost)) return;

        player.IncreaseMoveSpeed(0.3f);
        UpdateUI();
    }

    public void UpgradeHp()
    {
        if (!gameManager.SpendUsb(cost)) return;

        player.IncreaseMaxHp(100f);
        UpdateUI();
    }

    public void CloseShop()
    {
        gameManager.SaveGame(); // ✅ save khi đóng shop
        gameObject.SetActive(false);

        if (mobileControls != null)
            mobileControls.SetActive(true);

        Time.timeScale = 1f;
    }
}
