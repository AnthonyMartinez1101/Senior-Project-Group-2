using UnityEngine;

public class Caterpillar : MonoBehaviour
{
    public GameObject fakePrefab;
    public FloatingHealth healthBar;
    private bool isSplit = false;
    private GameObject fake;

    void Update()
    {
        if (!isSplit && healthBar.healthPercentage() < 0.5f) Split();
        if (healthBar.healthPercentage() <= 0.0f)
        {
            Destroy(fake);
        }    
    }

    private void Split()
    {
        isSplit = true;
        fake = Instantiate(fakePrefab, transform.position, Quaternion.identity);
    }
}
