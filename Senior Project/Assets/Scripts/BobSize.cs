using UnityEngine;
using System.Collections;

public class BobSize : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] private float shrinkMultiplier = 0.8f; // 80%
    [SerializeField] private float growMultiplier = 1.2f;   // 120%

    [Header("Timing")]
    [SerializeField] private float shrinkTime = 0.06f;
    [SerializeField] private float growTime = 0.08f;
    [SerializeField] private float returnTime = 0.08f;

    private Vector3 originalScale;


    private void Awake()
    {
        originalScale = transform.localScale;
    }



    public IEnumerator BobOne()
    {
        Vector3 shrinkScale = originalScale * shrinkMultiplier;
        Vector3 growScale = originalScale * growMultiplier;

        yield return ScaleTo(shrinkScale, shrinkTime);
        yield return ScaleTo(growScale, growTime);
    }

    public IEnumerator BobTwo()
    {
        yield return ScaleTo(originalScale, returnTime);
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
