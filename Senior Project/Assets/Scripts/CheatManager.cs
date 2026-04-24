using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    private ICheat[] cheatScripts;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cheatScripts = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICheat>().ToArray();
        StartCoroutine(CheckForCheats());
    }

    public void EnableCheats()
    {
        foreach(ICheat cheat in cheatScripts)
        {
            cheat.SetCheats();
        }
        Debug.Log("CHEATS ACTIVATED");
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
}
