using TMPro;
using UnityEngine;

public class GameoverUI : MonoBehaviour
{
    [Header("UI References")]
    private TMP_Text goldText;
    private TMP_Text enemyText;
    private TMP_Text playerTimeText;

    const string COIN_AMOUNT_TEXT = "Gold Amount Text";
    const string ENEMY_AMOUNT_TEXT = "Enemy Amount Text";
    const string PLAYER_TIME_TEXT = "Player Life Time";

    void Start()
    {
        if (GameManager.Instance != null)
        {
            goldText = GameObject.Find(COIN_AMOUNT_TEXT).GetComponent<TMP_Text>();
            enemyText = GameObject.Find(ENEMY_AMOUNT_TEXT).GetComponent<TMP_Text>();
            playerTimeText = GameObject.Find(PLAYER_TIME_TEXT).GetComponent<TMP_Text>();

            goldText.text = GameManager.Instance.coinsCollected.ToString("D4");
            enemyText.text = GameManager.Instance.enemiesKilled.ToString("D4");

            float elapsed = GameManager.Instance.ElapsedTime;
            int minutes = Mathf.FloorToInt(elapsed / 60f);
            int seconds = Mathf.FloorToInt(elapsed % 60f);
            playerTimeText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}
