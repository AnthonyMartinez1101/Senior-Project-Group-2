using UnityEngine;

public class FakeCaterpillar : MonoBehaviour
{
    private GameObject mainCaterpillar;

    private bool setReference = false;

    private BossScript bossScript;

    public GameObject deathParticles;

    public void CreateFake(GameObject pillah)
    {
        mainCaterpillar = pillah;
        setReference = true;
        bossScript = GetComponent<BossScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!setReference) return;

        if (mainCaterpillar == null) 
        {
            if (bossScript) bossScript.StopAllCoroutines();
            if(deathParticles) Instantiate(deathParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
