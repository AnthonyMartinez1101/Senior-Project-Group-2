using UnityEngine;
using TMPro;


public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private int coinCount = 0;
    [SerializeField] private TMP_Text coinCountText;


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
        if (coinCountText != null)
        {
            coinCountText.text = "x" + coinCount;
        }
    }
}
