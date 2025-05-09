using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coinsCollected = 0;
    public int enemiesKilled = 0;

    private float startTime;
    public float ElapsedTime => Time.time - startTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            startTime = Time.time;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoin(int amount = 1)
    {
        coinsCollected += amount;
    }

    public void AddEnemyKill(int amount = 1)
    {
        enemiesKilled += amount;
    }

    public void ResetStats()
    {
        coinsCollected = 0;
        enemiesKilled = 0;
        startTime = Time.time;
    }
}
