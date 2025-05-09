using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    public GameObject canvasPrefab;
    public GameObject managerPrefab;
    public GameObject audioPrefab;
    public GameObject[] altarRunes;

    private static bool hasSpawned = false;
    private bool allEnemiesDefeated = false;
    private bool playerInRange = false;
    private bool altarActivated = false;

    private GameObject loadingScreen;
    private GameObject existingCanvas;

    void Start()
    {
        if (!hasSpawned)
        {
            GameObject existingPlayer = GameObject.FindWithTag("Player");

            if (existingPlayer == null)
            {
                if (canvasPrefab != null)
                {
                    existingCanvas = Instantiate(canvasPrefab);
                    DontDestroyOnLoad(existingCanvas);
                }

                GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                DontDestroyOnLoad(player);

                GameObject cam = Instantiate(cameraPrefab, transform.position, Quaternion.identity);
                DontDestroyOnLoad(cam);

                GameObject man = Instantiate(managerPrefab, transform.position, Quaternion.identity);
                DontDestroyOnLoad(man);

                GameObject audio = Instantiate(audioPrefab, transform.position, Quaternion.identity);
                DontDestroyOnLoad(audio);
            }

            hasSpawned = true;
        }

        foreach (GameObject rune in altarRunes)
        {
            rune.SetActive(false);
        }

        existingCanvas = GameObject.FindWithTag("UICanvas");
        if (existingCanvas != null)
        {
            loadingScreen = existingCanvas.transform.Find("Loading Screen")?.gameObject;

            if (loadingScreen != null && loadingScreen.activeSelf)
            {
                StartCoroutine(DisableLoadingScreenAfterDelay());
            }
        }

        StartCoroutine(CheckEnemiesRoutine());
    }

    private IEnumerator CheckEnemiesRoutine()
    {
        while (!allEnemiesDefeated)
        {
            yield return new WaitForSeconds(1f);

            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                allEnemiesDefeated = true;
                StartCoroutine(ActivateRunesSequentially());
            }
        }
    }

    private IEnumerator ActivateRunesSequentially()
    {
        foreach (GameObject rune in altarRunes)
        {
            rune.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        altarActivated = true;
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
        }
    }

    void Update()
    {
        if (playerInRange && altarActivated && Input.GetButtonDown("Attack"))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;
            int lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;

            if (nextSceneIndex == lastSceneIndex && existingCanvas != null)
            {
                Destroy(existingCanvas);
                Debug.Log("UICanvas đã bị hủy vì chuyển sang scene cuối.");
            }

            if (loadingScreen != null && nextSceneIndex < lastSceneIndex)
            {
                loadingScreen.SetActive(true);
            }

            StartCoroutine(LoadNextSceneWithDelay(nextSceneIndex));
        }
    }

    private IEnumerator LoadNextSceneWithDelay(int nextSceneIndex)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextSceneIndex);
    }

    private IEnumerator DisableLoadingScreenAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        loadingScreen.SetActive(false);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = Vector3.zero;
        }
    }

    public static void ResetSpawnState()
    {
        hasSpawned = false;
    }
}
