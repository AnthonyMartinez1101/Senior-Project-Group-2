using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public Image block1;
    public Image block2;
    public Image block3;

    public Image background;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(block1 == null || block2 == null || block3 == null || background == null)
        {
            Debug.LogWarning("Images are not assigned in the inspector.");
        }
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(1.5f);

        // Fade in block1
        if(block1) yield return StartCoroutine(FadeOut(block1, 2f));

        yield return new WaitForSeconds(2.5f);

        // Fade in block2
        if(block2) yield return StartCoroutine(FadeOut(block2, 2f));

        yield return new WaitForSeconds(3f);

        // Fade in block3
        if(block3) yield return StartCoroutine(FadeOut(block3, 2f));

        yield return new WaitForSeconds(2.5f);

        if(background) yield return StartCoroutine(FadeOut(background, 4f));

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
}
