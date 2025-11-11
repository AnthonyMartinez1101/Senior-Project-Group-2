using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Material material;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    public void FlashOnDamage()
    {
        StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        //material.SetColor("_FlashColor", flashColor);
        //yield return new WaitForSeconds(0.2f);
        material.SetColor("_FlashColor", Color.red);

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while(elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
            material.SetFloat("_FlashAmount", currentFlashAmount);

            yield return null;
        }
    }

    public void FlashOnHeal()
    {
        StartCoroutine(HealFlasher());
    }

    private IEnumerator HealFlasher()
    {
        //material.SetColor("_FlashColor", flashColor);
        //yield return new WaitForSeconds(0.2f);
        material.SetColor("_FlashColor", Color.green);

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;

            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
            material.SetFloat("_FlashAmount", currentFlashAmount);

            yield return null;
        }
    }
}
