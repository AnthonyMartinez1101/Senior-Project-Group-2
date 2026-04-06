using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public Image block1;
    public Image block2;
    public Image block3;

    public Image blackImage;

    private bool cutsceneSkipped = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(block1 == null || block2 == null || block3 == null || blackImage == null)
        {
            Debug.LogWarning("Images are not assigned in the inspector.");
        }
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(1f);

        // Fade in block1
        if(block1) yield return StartCoroutine(FadeOut(block1, 2f));

        yield return new WaitForSeconds(2.5f);

        // Fade in block2
        if(block2) yield return StartCoroutine(FadeOut(block2, 2f));

        yield return new WaitForSeconds(3f);

        // Fade in block3
        if(block3) yield return StartCoroutine(FadeOut(block3, 2f));

        yield return new WaitForSeconds(2.5f);

        if(blackImage) yield return StartCoroutine(FadeIn(blackImage, 4f));

        SceneManager.LoadScene("Main Scene");
    }

    public void SkipCutsceneButton()
    {
        if (cutsceneSkipped) return;
        cutsceneSkipped = true;
        StartCoroutine(SkipCutscene());
    }

    IEnumerator SkipCutscene()
    {
        if(blackImage) yield return StartCoroutine(FadeIn(blackImage, 1f));
        SceneManager.LoadScene("Main Scene");
    }

    IEnumerator FadeOut(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color originalColor = image.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Ensure it's fully transparent at the end
    }

    IEnumerator FadeIn(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color originalColor = image.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Ensure it's fully opaque at the end
    }
}
