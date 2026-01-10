using UnityEngine;
using TMPro;
using System.Collections;


public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private int coinCount = 0;
    [SerializeField] private TMP_Text coinCountText;

    [Header("Coin Transition Time (sec)")]
    [SerializeField] private float timeToUpdate = 1f;

    private int displayedCoinCount = 0;

    private Coroutine currentCoroutine;



    void Start()
    {
        UpdateText();
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        UpdateText();
    }

    public void RemoveCoins(int amount)
    {
        coinCount -= amount;
        UpdateText();
    }

    private void UpdateText()
    {
        if(currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ChangeCoinCount());
    }



    IEnumerator ChangeCoinCount()
    {
        int changeAmount = Mathf.Abs(coinCount - displayedCoinCount);
        int direction;
        if ((coinCount - displayedCoinCount) >= 0) direction = 1;
        else direction = -1;

        if (changeAmount == 0)
        {
            coinCountText.text = "x" + displayedCoinCount.ToString();
            yield break;
        }
        

        int stepAmount;
        float updateTime;
        if (changeAmount >= 100)
        {
            stepAmount = 3;
            updateTime = timeToUpdate * 2f;
        }
        else if(changeAmount >= 50)
        {
            stepAmount = 2;
            updateTime = timeToUpdate * 1.5f;
        }
        else
        {
            stepAmount = 1;
            updateTime = timeToUpdate;
        }

        float stepTime = updateTime / changeAmount;

        while (displayedCoinCount != coinCount)
        {
            displayedCoinCount += stepAmount * direction;

            if((direction == 1 && displayedCoinCount > coinCount) || (direction == -1 && displayedCoinCount < coinCount))
            {
                displayedCoinCount = coinCount;
            }

            coinCountText.text = "x" + displayedCoinCount.ToString();

            yield return new WaitForSeconds(stepTime);
        }
    }
}
