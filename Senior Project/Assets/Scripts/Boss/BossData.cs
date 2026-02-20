using UnityEngine;
using System.Collections.Generic;

public abstract class BossAction : ScriptableObject
{
    public string actionName;
    public float actionDuration;
    public abstract void ExecuteAction(BossScript boss);
}

[CreateAssetMenu(fileName = "BossData", menuName = "Boss/BossData")]
public class BossData : ScriptableObject
{
    public string bossName;
    public Sprite bossSprite;
    public Sprite altSprite;

    [Header("Stats")]
    public float maxHealth = 300f;
    public float attackRating = 5f;
    public float knockbackForce = 15f;
    public float hitCooldown = 2.0f;
    public bool isInvincible = false;

    [Header("Phases")]
    public float phaseTwoThreshold = 0.5f;
    public float phaseThreeThreshold = 0.0f;
    public float rageMultiplier = 1.5f;
    public float cooldownTime = 0.5f; // break between actions
    public float idleTime = 2.0f;

    [Header("Moveset")]
    public List<BossAction> actions;
}
