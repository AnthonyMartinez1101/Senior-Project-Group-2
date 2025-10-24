using UnityEngine;

public class WellScript : MonoBehaviour
{
    private SpriteRenderer wellSprite; //Reference to parent object's sprite renderer


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wellSprite = GetComponentInParent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MakeSpriteLighter();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MakeSpriteNormal();
        }
    }


    private void MakeSpriteLighter()
    {
        if (wellSprite != null)
        {
            Color lighterColor = wellSprite.color;
            lighterColor.a = 0.8f; //Set alpha to 80% for lighter effect
            wellSprite.color = lighterColor;
        }
    }

    private void MakeSpriteNormal()
    {
        if (wellSprite != null)
        {
            Color normalColor = wellSprite.color;
            normalColor.a = 1.0f; //Set alpha to 100% for normal effect
            wellSprite.color = normalColor;
        }
    }
}
