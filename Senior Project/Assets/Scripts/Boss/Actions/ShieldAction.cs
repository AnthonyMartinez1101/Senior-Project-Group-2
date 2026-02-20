using System.Collections;
using UnityEngine;

public class ShieldAction : MonoBehaviour
{
    [SerializeField] private float maxHealth = 25f;
    [SerializeField] private float curHealth;

    private BossScript owner;

    void Start()
    {
        if (curHealth <= 0f) curHealth = maxHealth;
    }


    public void Initialize(BossScript boss)
    {
        owner = boss;
        curHealth = maxHealth;
        if (owner != null)
        {
            owner.isShielded = true;
        }
    }


    public void TakeDamage(float dmg = 1f)
    {
        curHealth -= dmg;

        if (curHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.isShielded = false;
        }
    }
}