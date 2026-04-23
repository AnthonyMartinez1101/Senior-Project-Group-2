using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class StoreChickens : MonoBehaviour
{
    public WorldClock clock;

    private List<GameObject> chickens = new List<GameObject>();

    private Coroutine releasingChickens;

    // Update is called once per frame
    void Update()
    {
        if(!clock || chickens.Count <= 0 || releasingChickens != null) return;

        if (clock.IsDay())
        {
            releasingChickens = StartCoroutine(ReleaseChickens());
        }
    }

    IEnumerator ReleaseChickens()
    {
        foreach (var chicken in chickens)
        {
            var wander = chicken.GetComponent<ChickenWander>();
            if (wander) wander.StopHiding();
            chicken.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        chickens.Clear();
        releasingChickens = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var chicken = other.GetComponent<ChickenWander>();
        if (!chicken) return;

        if (chicken.IsHiding())
        {
            chickens.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
}
