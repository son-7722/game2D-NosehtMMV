using System;

[Serializable]
public class SaveData
{
    public int wave;
    public int score;
    public int usbCount;
    public float difficultyMultiplier;

    // PLAYER
    public float maxHp;
    public float currentHp;
    public float moveSpeed;

    // GUN
    public float damage;
    public int ammo;

    public SaveData(
        int wave,
        int score,
        int usbCount,
        float difficultyMultiplier,
        float maxHp,
        float currentHp,
        float moveSpeed,
        float damage,
        int ammo
    )
    {
        this.wave = wave;
        this.score = score;
        this.usbCount = usbCount;
        this.difficultyMultiplier = difficultyMultiplier;

        this.maxHp = maxHp;
        this.currentHp = currentHp;
        this.moveSpeed = moveSpeed;

        this.damage = damage;
        this.ammo = ammo;
    }
}
