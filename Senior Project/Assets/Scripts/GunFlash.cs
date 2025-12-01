using UnityEngine;

public class GunFlash : MonoBehaviour
{
    public GameObject flashOne;
    public GameObject flashTwo;
    public GameObject flashThree;
    public GameObject flashFour;
    public GameObject flashFive;

    public float flashDuration = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int randNum = Random.Range(1, 6);

        if(randNum == 1) flashOne.SetActive(false);
        else if(randNum == 2) flashTwo.SetActive(false);
        else if(randNum == 3) flashThree.SetActive(false);
        else if(randNum == 4) flashFour.SetActive(false);
        else if(randNum == 5) flashFive.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        flashDuration -= Time.deltaTime;
        if (flashDuration <= 0f) Destroy(gameObject);
    }
}
