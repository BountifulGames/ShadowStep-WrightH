using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool HasKeycard {  get;  set; }


    public bool isHacking;
    public int detectCount;
    private float alertLevel;

    [SerializeField] private int detectCountMax = 3;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelEndScreen;
    [SerializeField] private TMP_Text alertLevelText;

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
        detectCount = 0;
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
            enemy.IncreaseAlert(detectCount + 1);
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
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GameOver()
    {
        PlayerMovement.canMove = false;
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnNextLevelButton()
    {
        detectCount = 0;
        levelEndScreen.SetActive(false);
        HasKeycard = false;

    }

    public void UpdateUI()
    {
        alertLevel = 100 / detectCountMax * detectCount;
        alertLevelText.text = ("Alert Level: " + alertLevel) + "%";
    }

    public void OnRetryPress()
    {
        gameOverScreen.SetActive(false);
        detectCount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
        PlayerMovement.canMove = true;
        UpdateUI();
    }
}
