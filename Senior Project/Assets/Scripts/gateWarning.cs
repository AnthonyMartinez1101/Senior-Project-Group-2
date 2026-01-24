using System.Collections;
using UnityEngine;

public class gateWarning : MonoBehaviour
{

    [SerializeField] private GameObject warningSign;

    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(TextBox());
    }

    IEnumerator TextBox()
    {
        warningSign.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(5f);
        warningSign.GetComponent<SpriteRenderer>().enabled = false;
    }
}
