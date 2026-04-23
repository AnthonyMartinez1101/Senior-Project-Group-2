using UnityEngine;

public class DropPackageSprite : MonoBehaviour
{
    [SerializeField] private GameObject packageSprite;

    [SerializeField] private float startHeight = 10f;      // How high above this object it starts
    [SerializeField] private float gravity = 9.8f;        // Downward acceleration
    [SerializeField] private bool snapToTargetOnFinish = true;

    [SerializeField] private GameObject particles;

    private float verticalVelocity = 0f;
    private bool isDropping = false;
    private Vector3 targetPosition;

    private SpriteRenderer sr;
    private BoxCollider2D col;

    private void Start()
    {
        if (packageSprite == null)
        {
            Debug.LogWarning("DropToTarget2D: No object assigned to drop.");
            return;
        }

        targetPosition = transform.position;

        Vector3 startPosition = targetPosition + Vector3.up * startHeight;
        packageSprite.transform.position = startPosition;

        sr = packageSprite.GetComponent<SpriteRenderer>();
        if (sr) sr.sortingOrder = 5;

        col = GetComponent<BoxCollider2D>();
        if (col) col.enabled = false;

        verticalVelocity = 0f;
        isDropping = true;
    }

    private void Update()
    {
        if (!isDropping || packageSprite == null) return;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 currentPosition = packageSprite.transform.position;
        currentPosition.y -= verticalVelocity * Time.deltaTime;

        if (currentPosition.y <= targetPosition.y)
        {
            currentPosition.y = snapToTargetOnFinish ? targetPosition.y : currentPosition.y;
            packageSprite.transform.position = currentPosition;
            isDropping = false;
            if (sr) sr.sortingOrder = 4;
            if (col) col.enabled = true;
            if (particles) Instantiate(particles, targetPosition, Quaternion.identity);
            return;
        }

        packageSprite.transform.position = currentPosition;
        
    }
}
