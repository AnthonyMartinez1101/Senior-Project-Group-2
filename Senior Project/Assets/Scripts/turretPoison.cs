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
        var poisonable = collision.GetComponent<IPoisonable>();
        if (poisonable != null)
        {
            poisonable.ApplyPoison(5);
        }
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(0.25f); 
        Destroy(gameObject);
    }
}
