using UnityEngine;

public class plantGORWTH : MonoBehaviour
{
    private int stage = 0;
    public int growthTimer;
    public int maxStage = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("Growth", growthTimer, growthTimer);
    }

    public void Growth()
    {
        if(stage  != maxStage)
        {
            gameObject.transform.GetChild(stage).gameObject.SetActive(true);
            
        }
        if(stage > 0 && stage < maxStage)
        {
            gameObject.transform.GetChild(stage - 1).gameObject.SetActive(false);
        }
        if(stage < maxStage)
        {
            stage++;
        }
    }
}
