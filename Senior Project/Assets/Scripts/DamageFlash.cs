using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private float flashTime = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void FlashOnDamage()
    {
        StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        spriteRenderer.color = damageColor;

        float elapsedTime = 0f;
        while(elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / flashTime;

            spriteRenderer.color = Color.Lerp(damageColor, originalColor, percentage);

            yield return null;
        }

        spriteRenderer.color = originalColor;
    }

    public void FlashOnHeal()
    {
        StartCoroutine(HealFlasher());
    }

    private IEnumerator HealFlasher()
    {
        spriteRenderer.color = healColor;

        float elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            float percentage = elapsedTime / flashTime;

            spriteRenderer.color = Color.Lerp(healColor, originalColor, percentage);

            yield return null;
        }

        spriteRenderer.color = originalColor;
    }
}
