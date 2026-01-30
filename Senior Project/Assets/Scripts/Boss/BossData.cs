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

    [Header("Stats")]
    public float maxHealth;
    public float attackRating;
    public float knockbackForce;
    public float hitCooldown = 2.0f;

    [Header("Phases")]
    public float phaseTwoThreshold = 0.5f;
    public float phaseThreeThreshold = 0.0f;
    public float rageMultiplier = 1.5f;
    public float cooldownTime = 0.5f; // break between actions
    public float idleTime = 2.0f;

    [Header("Moveset")]
    public List<BossAction> actions;
}
