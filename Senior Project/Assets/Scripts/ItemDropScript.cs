using System.Collections;
using UnityEngine;

public class ItemDropScript : MonoBehaviour
{
    //References to children sprite renderers
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;

    private Rigidbody2D rb;

    public Item itemInfo;
    private Item item;

    private bool canPull = false;

    private bool hasTarget = false;
    private Transform targetPos;

    private float pullSpeed;
    private float accelerationRate;

    public bool canDespawn = true;
    public bool canBeCollected = true;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (canDespawn)
        {
            StartCoroutine(ItemTimer());
        }

        // Initialize item if itemInfo is set in inspector
        if (itemInfo != null)
        {
            CreateItemDrop(itemInfo);
        }
    }

    IEnumerator ItemTimer()
    {
        yield return new WaitForSeconds(20f);

        for (int i = 0; i < 10; i++)
        {
            yield return BlinkFor(0.5f);
            yield return new WaitForSeconds(0.5f);
        }
        for (int i = 0; i < 5; i++)
        {
            yield return BlinkFor(0.25f);
            yield return new WaitForSeconds(0.25f);
        }
        for (int i = 0; i < 12; i++)
        {
            yield return BlinkFor(0.125f);
            yield return new WaitForSeconds(0.125f);
        }
        Destroy(gameObject);
    }

    IEnumerator BlinkFor(float time)
    {
        spriteRenderer.enabled = false;
        shadowSpriteRenderer.enabled = false;
        yield return new WaitForSeconds(time);
        spriteRenderer.enabled = true;
        shadowSpriteRenderer.enabled = true;
    }

    public void CreateItemDrop(Item itemInfo)
    {
        item = itemInfo;

        if (item.icon != null) spriteRenderer.sprite = item.icon;
        else spriteRenderer.sprite = GameManager.Instance.NoSprite();

        DropBounce dropBounce = GetComponent<DropBounce>();
        if (dropBounce != null)
        {
            dropBounce.StartBounce();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeCollected) return;
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.AddToInventory(item);
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!canPull)
        {
            rb.AddForce(-rb.linearVelocity * 3f, ForceMode2D.Force);
            if(rb.linearVelocity.magnitude < 0.1f)
            {
                rb.linearVelocity = Vector2.zero;
                canPull = true;
            }
        }
        else
        {
            if (hasTarget)
            {
                Vector2 targetDir = ((Vector2)targetPos.position - rb.position).normalized;
                rb.linearVelocity = targetDir * pullSpeed;
                pullSpeed += pullSpeed * accelerationRate; // Accelerate pull speed
            }
        }
    }

    public void SetTarget(Transform target, float speed, float acc)
    {   
        if(!hasTarget && canPull)
        {
            targetPos = target;
            pullSpeed = speed;
            accelerationRate = acc;
            hasTarget = true;
        }
    }
}
