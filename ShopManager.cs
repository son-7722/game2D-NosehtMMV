using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject mobileControls;

    [SerializeField] private Player player;
    [SerializeField] private Gun gun;

    [SerializeField] private TextMeshProUGUI usbText;

    private int cost = 1;

    private void OnEnable()
    {
        Time.timeScale = 0f;

        if (mobileControls != null)
            mobileControls.SetActive(false);

        UpdateUI();
    }


    void UpdateUI()
    {
        usbText.text = "USB: " + gameManager.GetUsb();
    }

    public void UpgradeDamage()
    {
        if (!gameManager.SpendUsb(cost)) return;

        gun.IncreaseDamage(4f);
        UpdateUI();
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
        gameManager.SaveGame();
        gameObject.SetActive(false);

        if (mobileControls != null)
            mobileControls.SetActive(true);

        Time.timeScale = 1f;
    }

}
