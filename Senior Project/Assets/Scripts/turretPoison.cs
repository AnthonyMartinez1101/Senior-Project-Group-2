using System.Collections;
using UnityEngine;

public class turretPoison : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(WaitToDestroy());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss") || collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.ApplyPoison(5); 
            }

            var boss = collision.GetComponent<BossScript>();
            if(boss)
            {
                boss.ApplyPoison(5);
            }
        }
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(0.25f); 
        Destroy(gameObject);
    }
}
