using UnityEngine;
using TMPro;

public class PlayerCollecting : MonoBehaviour
{
    private int coinCount = 0;
    public TextMeshProUGUI coinText;

    private void Start()
    {
        UpdateCoinText();
    }

    public void AddCoin(int value)
    {
        coinCount += value;
        UpdateCoinText();
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    private void UpdateCoinText()
    {
        coinText.text = "Coins: " + coinCount;
    }
}
