using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    public GameObject puddlePrefab;
    private Vector2 direction;
    private float speed;
    private float travelDistance;
    private Vector2 startPos;
    private bool hasLanded = false;

    public void Initialize(Vector2 targetPos, float throwSpeed, float angleOffset)
    {
        startPos = transform.position;
        direction = (targetPos - startPos).normalized;
        if (angleOffset != 0)
        {
            direction = Quaternion.Euler(0, 0, angleOffset) * direction;
        }
        speed = throwSpeed;
        travelDistance = Vector2.Distance(startPos, targetPos);
    }

    void Update()
    {
        if (hasLanded) return;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (Vector2.Distance(startPos, transform.position) >= travelDistance)
        {
            Land();
        }
    }

    private void Land()
    {
        hasLanded = true;
        Instantiate(puddlePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
