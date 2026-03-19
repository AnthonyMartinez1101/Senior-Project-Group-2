using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatBlock
{
    public int numZombiesKilled = 0;
    public int numPestsKilled = 0;
    public int numBossesKilled = 0;
    public int numHarvested = 0;
    public int soilsWatered = 0;

    public float totalDamageTaken = 0;
    public float dmgBuff = 0;
    public float maxHealthBuff = 0;
    public float speedBuff = 0;
}

public class StatManager : MonoBehaviour
{
    public static StatManager Instance;

    private PlayerStatBlock playerStats = new PlayerStatBlock();
    public TMP_Text StatsText;

    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdateStatText()
    {
        StatsText.text = $"Zombies Killed: {playerStats.numZombiesKilled}\nPests Killed: {playerStats.numPestsKilled}\nBosses Killed: {playerStats.numBossesKilled}\nHarvested: {playerStats.numHarvested}\nDamage Taken: {playerStats.totalDamageTaken}";
    }

    public void AddHarvest()
    {
        playerStats.numHarvested++;
        UpdateStatText();
    }

    public void AddDamageTaken(float damage)
    {
        playerStats.totalDamageTaken += damage;
        UpdateStatText();
    }
}