using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    public float magnetPullSpeed = 5f;
    public float accelerationRate = 0.1f;

    public bool canPullItems = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canPullItems) return;
        if (collision.gameObject.TryGetComponent<ItemDropScript>(out ItemDropScript pullItem))
        {
            pullItem.SetTarget(transform.parent, magnetPullSpeed, accelerationRate);
        }
    }
}
