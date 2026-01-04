using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
    public float magnetPullSpeed = 5f;
    public float accelerationRate = 0.1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<ItemDropScript>(out ItemDropScript pullItem))
        {
            pullItem.SetTarget(transform.parent, magnetPullSpeed, accelerationRate);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<ItemDropScript>(out ItemDropScript pullItem))
        {
            pullItem.ClearTarget();
        }

    }
}
