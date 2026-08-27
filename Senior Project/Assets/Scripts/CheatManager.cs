using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class CheatManager : MonoBehaviour
{
    private ICheat[] cheatScripts;
    private IGoCrazy[] goCrazyScripts;

    [SerializeField] TMP_Text cheatText;

    private Coroutine flashTextCoroutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cheatScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICheat>().ToArray();
        goCrazyScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGoCrazy>().ToArray();
        StartCoroutine(CheckForCheats());
        StartCoroutine(CheckForCrazy());
        StartCoroutine(SetEndlessMode());
    }

    public void EnableCheats()
    {
        foreach (ICheat cheat in cheatScripts)
        {
            cheat.SetCheats();
        }
        FlashAndAddText("iCheat Activated", Color.green);
        Debug.Log("CHEATS ACTIVATED");
    }

    public void EnableGoCrazy()
    {
        foreach (IGoCrazy goCrazy in goCrazyScripts)
        {
            goCrazy.GoCrazy();
        }
        FlashAndAddText("GoCrazy Activated", Color.red);
        Debug.Log("GO CRAZY ACTIVATED");
    }

    IEnumerator SetEndlessMode()
    {
        yield return new WaitUntil(() => Keyboard.current.eKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.nKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.dKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.lKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.eKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.sKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.sKey.wasPressedThisFrame);
        GameManager.Instance.SetEndless();
        FlashAndAddText("Endless Activated", Color.red);
        Debug.Log("Endless Mode Activated");
    }

    IEnumerator CheckForCheats()
    {
        yield return new WaitUntil(() => Keyboard.current.iKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.cKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.hKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.eKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.aKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.tKey.wasPressedThisFrame);
        EnableCheats();
    }

    IEnumerator CheckForCrazy()
    {
        yield return new WaitUntil(() => Keyboard.current.gKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.oKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.cKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.rKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.aKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.zKey.wasPressedThisFrame);
        yield return new WaitUntil(() => Keyboard.current.yKey.wasPressedThisFrame);
        EnableGoCrazy();
    }


    private void FlashAndAddText(string text, Color clr)
    {
        if (cheatText)
        {
            cheatText.text += text + "\n";
            if (flashTextCoroutine != null)
            {
                StopCoroutine(flashTextCoroutine);
            }
            flashTextCoroutine = StartCoroutine(FlashText(clr));
        }
    }

    private IEnumerator FlashText(Color clr)
    {
        cheatText.color = clr;

        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            cheatText.color = Color.Lerp(clr, Color.white, elapsedTime / duration);

            yield return null;
        }

        cheatText.color = Color.white;
        flashTextCoroutine = null;
    }
}
