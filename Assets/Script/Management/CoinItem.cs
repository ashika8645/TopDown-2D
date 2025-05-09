using UnityEngine;

public class CoinItem : MonoBehaviour
{
    [SerializeField] private int coinAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.UpdateCurrentGold(coinAmount);
                GameManager.Instance.AddCoin();
            }

            Destroy(gameObject);
        }
    }
}
