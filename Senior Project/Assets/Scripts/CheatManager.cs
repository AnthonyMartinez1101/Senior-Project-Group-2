using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    private ICheat[] cheatScripts;
    private IGoCrazy[] goCrazyScripts;


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
        Debug.Log("CHEATS ACTIVATED");
    }

    public void EnableGoCrazy()
    {
        foreach (IGoCrazy goCrazy in goCrazyScripts)
        {
            goCrazy.GoCrazy();
        }
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
}
