using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isKnockbackActive = false;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Transform hitFrom, float force)
    {
        if(!isKnockbackActive) StartCoroutine(KnockbackRoutine(hitFrom, force));
    }

    private IEnumerator KnockbackRoutine(Transform hitFrom, float force)
    {
        isKnockbackActive = true;

        Vector2 direction = (transform.position - hitFrom.position).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        float timer = 0.1f;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;

        isKnockbackActive = false;
    }

    public bool IsKnockbackActive()
    {
        return isKnockbackActive;
    }
}
