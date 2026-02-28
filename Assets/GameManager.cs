using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SaveData save;

    // Runtime upgrade values (computed from save)
    public int bulletDamage = 1;
    public float fireRate = 0.5f;
    public int lightningDamage = 0;
    public int lightningBounces = 0;  // 0 = lightning not bought
    public float poisonDamagePerSec = 0;
    public float poisonDuration = 0;  // 0 = poison not bought
    public float currencyMultiplier = 1f;

    // Base values
    private const float BASE_FIRE_RATE = 0.5f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSave();
        ApplyUpgrades();
    }

    void LoadSave()
    {
        string json = PlayerPrefs.GetString("GameSave", "");
        if (!string.IsNullOrEmpty(json))
        {
            save = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            save = new SaveData();
            foreach (var id in System.Enum.GetValues(typeof(UpgradeId)).Cast<UpgradeId>())
                save.upgradeLevels[id] = 0;
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetString("GameSave", JsonUtility.ToJson(save));
        PlayerPrefs.Save();
    }

    public void ApplyUpgrades()
    {
        int lvl;

        lvl = save.upgradeLevels[UpgradeId.BulletDamage];
        bulletDamage = 1 + lvl;

        lvl = save.upgradeLevels[UpgradeId.FireRate];
        fireRate = BASE_FIRE_RATE / (1f + lvl * 0.2f);

        lvl = save.upgradeLevels[UpgradeId.LightningDamage];
        lightningDamage = lvl; // 0 if not bought

        lvl = save.upgradeLevels[UpgradeId.LightningBounces];
        lightningBounces = lvl; // 0 if not bought at all

        lvl = save.upgradeLevels[UpgradeId.PoisonDamagePerSec];
        poisonDamagePerSec = lvl; // 0 if not bought

        lvl = save.upgradeLevels[UpgradeId.PoisonDuration];
        poisonDuration = lvl > 0 ? 1f + (lvl - 1) * 0.5f : 0f;

        lvl = save.upgradeLevels[UpgradeId.CurrencyMultiplier];
        currencyMultiplier = 1f + lvl * 0.5f;
    }

    public void AddCurrency(double amount)
    {
        save.currency += amount * currencyMultiplier;
        SaveGame();
    }
}