using UnityEngine;

public class BossSpawnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform spawnPoint;

    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasSpawned || !other.CompareTag("Player")) return;

        hasSpawned = true;

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Instantiate(bossPrefab, pos, Quaternion.identity);
    }
}
