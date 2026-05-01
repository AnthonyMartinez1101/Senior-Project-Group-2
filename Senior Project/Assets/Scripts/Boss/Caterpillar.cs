using UnityEngine;

public class Caterpillar : MonoBehaviour
{
    public GameObject fakePrefab;
    public FloatingHealth healthBar;
    private bool isSplit = false;
    private FakeCaterpillar fake;

    void Update()
    {
        if (!isSplit && healthBar.healthPercentage() < 0.5f) Split();
    }

    private void Split()
    {
        isSplit = true;
        fake = Instantiate(fakePrefab, transform.position, Quaternion.identity).GetComponent<FakeCaterpillar>();
        if(fake) fake.CreateFake(gameObject);
    }
}
