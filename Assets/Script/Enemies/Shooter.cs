using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bulletPrefab;

    public void Attack()
    {
        Vector2 targetDirection = Player.Instance.transform.position - transform.position;

        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, 15f);
        GameObject newBullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        newBullet.transform.right = targetDirection;
    }
}
