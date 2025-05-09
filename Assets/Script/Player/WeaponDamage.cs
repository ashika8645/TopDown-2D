using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private int baseDamage = 1;

    public static PlayerDamage Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); 
    }

    public int GetDamage()
    {
        return baseDamage;
    }

    public void SetDamage(int newDamage)
    {
        baseDamage = newDamage;
    }

    public void IncreaseDamage(int amount)
    {
        baseDamage += amount;
    }
}
