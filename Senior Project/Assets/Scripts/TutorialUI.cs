using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    //Checkmark sprites
    public Sprite[] sprites;

    public Image checkmark;

    public TMP_Text instructionText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        checkmark.sprite = sprites[0];
        instructionText.text = "Use WASD to move around";
    }

    public void UpdateUI(string text, bool checkMark)
    {
        instructionText.text = text;
        if (checkMark) checkmark.sprite = sprites[1];
        else checkmark.sprite = sprites[0];
    }

    public void Checkmark(bool checkMark)
    {
        if (checkMark) checkmark.sprite = sprites[1];
        else checkmark.sprite = sprites[0];
    }
}
