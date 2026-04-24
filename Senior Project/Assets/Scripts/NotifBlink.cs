using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotifBlink : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private Sprite notif;
    [SerializeField] private float interval = 0.2f;

    private Coroutine blinkCoroutine;
    private bool isBlinking = false;
    private void Awake()
    {
        if (img == null) img = GetComponent<Image>();
    }

    public void EnableNotif()
    {
        if(img == null) return;
        if(notif!= null) 
        {
            img.sprite = notif;
        }
        if (isBlinking) return;

        isBlinking = true;
        blinkCoroutine = StartCoroutine(Blink());
    }

    public void DisableNotif()
    {
        if (img == null) return;
        isBlinking= false;
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        img.enabled = false;
    }

    private IEnumerator Blink()
    {
        while (isBlinking)
        {
            img.enabled = !img.enabled;
            yield return new WaitForSeconds(interval);
            img.enabled = img.enabled;
            yield return new WaitForSeconds(0.4f);
        }
    }
}
