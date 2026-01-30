using System.Collections;
using UnityEngine;

public class gateWarning : MonoBehaviour
{

    [SerializeField] private GameObject warningSign;

    void Start()
    {
        warningSign.GetComponent<SpriteRenderer>().enabled = false;     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision entered");
        if (collision.collider.CompareTag("Player")) StartCoroutine(TextBox());
    }

    IEnumerator TextBox()
    {
        warningSign.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(5f);
        warningSign.GetComponent<SpriteRenderer>().enabled = false;
    }
}
