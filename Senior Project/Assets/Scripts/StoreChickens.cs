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
        while(chickens.Count > 0 && clock.IsDay())
        {
            GameObject chicken = chickens[0];
            chickens.RemoveAt(0);

            var wander = chicken.GetComponent<ChickenWander>();

            if(wander)
            {
                wander.StopHiding();
                chicken.SetActive(true);
            }

            yield return new WaitForSeconds(0.5f);
        }

        releasingChickens = null;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var chicken = other.GetComponent<ChickenWander>();
        if (!chicken) return;

        if (chicken.IsHiding())
        {
            if(!chickens.Contains(other.gameObject)) chickens.Add(other.gameObject);
            other.gameObject.SetActive(false);
        }
    }
}
