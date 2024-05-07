using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool HasKeycard { get; set; }


    public bool isHacking;
    public int detectCount;
    private float alertLevel;
    private Image fadeImage;

    [SerializeField] private float fadeSpeed = 5f;

    [SerializeField] private int detectCountMax = 3;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelEndScreen;
    [SerializeField] private TMP_Text alertLevelText;
    [SerializeField] private GameObject fadeScreen;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
        {
            enemy.IncreaseAlert(detectCount);
        }
        UpdateUI();
        detectCount = 0;
        fadeImage = fadeScreen.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartHacking()
    {
        isHacking = true;
        // Optionally disable player movement
        PlayerMovement.canMove = false;
    }

    public void StopHacking()
    {
        isHacking = false;
        // Optionally enable player movement
        PlayerMovement.canMove = true;
    }

    public void AlertEnemies(Vector3 noisePosition)
    {
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
        {
            enemy.InvestigateNoise(noisePosition);
        }
    }

    public void IncreaseDetect()
    {
        detectCount++;
        Debug.Log("Detection Couint: " + detectCount);
        if (detectCount >= detectCountMax)
        {
            GameOver();
        }
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
        {
            enemy.IncreaseAlert(detectCount);
        }
        UpdateUI();
    }

    public void CollectKeycard()
    {
        HasKeycard = true;
    }

    public void CompleteLevel()
    {
        PlayerMovement.canMove = false;
        levelEndScreen.SetActive(true);
        Time.timeScale = 0.1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GameOver()
    {
        PlayerMovement.canMove = false;
        gameOverScreen.SetActive(true);
        Time.timeScale = 0.1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnNextLevelButton()
    {
        detectCount = 0;
        Time.timeScale = 0.1f;
        levelEndScreen.SetActive(false);
        StartCoroutine(FadeOutAndLoadNext());
        //StartCoroutine(FadeIn());
        HasKeycard = false;
        foreach (var enemy in FindObjectsOfType<EnemyAI>())
        {
            enemy.IncreaseAlert(1);
        }
        PlayerMovement.canMove = true;
        UpdateUI();
    }

    private IEnumerator FadeOutAndLoadNext()
    {
        yield return StartCoroutine(FadeOut());  
        SwitchScene();  
    }

    private void SwitchScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene < 3)
        {
            SceneManager.LoadScene(currentScene + 1);
        }
        if (currentScene == 3)
        {
            SceneManager.LoadScene(0);
        }
        StartCoroutine(FadeIn());

    }

    public void UpdateUI()
    {
        alertLevel = detectCount + 1;
        alertLevelText.text = ("Alert Level: " + alertLevel);
    }

    public void OnRetryPress()
    {
        Debug.Log("RetryButtonPress");
        detectCount = 0;

        Time.timeScale = 1f;
        gameOverScreen.SetActive(false);

        PlayerMovement.canMove = true;

        UpdateUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public IEnumerator FadeIn()
    {
        fadeImage.enabled = true;
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        float targetAlpha = 0.0f;
        while (Mathf.Abs(fadeImage.color.a - targetAlpha) > 0.01f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Lerp(fadeImage.color.a, targetAlpha, fadeSpeed * Time.deltaTime));
            yield return null;
        }
        fadeImage.enabled = false;
    }

    public IEnumerator FadeOut()
    {
        fadeImage.enabled = true;
        float targetAlpha = 1.0f;
        while (Mathf.Abs(fadeImage.color.a - targetAlpha) > 0.01f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Lerp(fadeImage.color.a, targetAlpha, fadeSpeed * Time.deltaTime));
            yield return null;
        }
    }
}
