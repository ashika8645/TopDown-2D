using UnityEngine;
using System.Collections;

public class ShopTrigger : MonoBehaviour
{
    private GameObject updateCardUI;
    private bool playerInRange = false;

    private void Start()
    {
        StartCoroutine(FindUICanvasRoutine());
    }

    private IEnumerator FindUICanvasRoutine()
    {
        GameObject canvas = null;

        while (canvas == null)
        {
            canvas = GameObject.FindWithTag("UICanvas");
            yield return null; 
        }

        Transform updateCardTransform = canvas.transform.Find("UpdateCard");

        if (updateCardTransform != null)
        {
            updateCardUI = updateCardTransform.gameObject;
            updateCardUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy UpdateCard trong Canvas.");
        }
    }


    private void Update()
    {
        if (playerInRange && Input.GetButtonDown("Attack"))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0 && updateCardUI != null)
            {
                updateCardUI.SetActive(true);
                Time.timeScale = 0f; 
            }
        }

        if (updateCardUI != null && updateCardUI.activeSelf && Input.GetButtonDown("Escape"))
        {
            updateCardUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (updateCardUI != null)
            {
                updateCardUI.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
}
