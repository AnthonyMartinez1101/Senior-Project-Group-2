using UnityEngine;

public class Caterpillar : MonoBehaviour
{
    public GameObject fakePrefab;
    public FloatingHealth healthBar;
    private bool isSplit = false;

    void Update()
    {
        if (!isSplit && healthBar.isHalf()) Split();
    }

    private void Split()
    {
        isSplit = true;
        GameObject fake = Instantiate(fakePrefab, transform.position, Quaternion.identity);
    }
}
