using UnityEngine;

public class WeaponAiming : MonoBehaviour
{
    public GameObject activeWeapon;
    public GameObject weaponCollider;
    public Player playerController;

    public float autoAimRange = 10f;
    public string enemyTag = "Enemy";

    private Vector3 lastMousePosition;
    private float mouseStillTime = 0f;
    private float idleThreshold = 2f;

    void Update()
    {
        Vector3 currentMousePosition = Input.mousePosition;

        if ((currentMousePosition - lastMousePosition).sqrMagnitude > 0.1f)
        {
            mouseStillTime = 0f;
            MouseFollowWithOffset();
        }
        else
        {
            mouseStillTime += Time.deltaTime;

            if (mouseStillTime >= idleThreshold)
            {
                AutoAimNearestEnemy();
            }
            else
            {
                MouseFollowWithOffset();
            }
        }

        lastMousePosition = currentMousePosition;
    }

    private void MouseFollowWithOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(playerController.transform.position);

        float angle = Mathf.Atan2(mousePos.y - playerScreenPoint.y, mousePos.x - playerScreenPoint.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            activeWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void AutoAimNearestEnemy()
    {
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 currentPosition = playerController.transform.position;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(enemyTag))
        {
            float distance = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distance < shortestDistance && distance <= autoAimRange)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            Vector3 direction = nearestEnemy.transform.position - currentPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (direction.x < 0)
            {
                activeWeapon.transform.rotation = Quaternion.Euler(0, -180, angle);
                weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
            }
            else
            {
                activeWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
                weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
