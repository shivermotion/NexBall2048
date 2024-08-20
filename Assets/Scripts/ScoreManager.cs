using UnityEngine;
using TMPro;
using System.Collections;
using Extension_Methods;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelUpText;  // Reference for the level-up text
    private CanvasGroup levelUpCanvasGroup;

    private int score = 0;
    private int highScore = 0;
    private int comboCount = 0;
    private float comboTimer = 0f;
    public float comboDuration = 2f;

    public GameOverZoneScaler gameOverZoneScaler;

    // Leveling system variables
    private int currentLevel = 1;
    private float a = 1000f;
    private float b = 10000f;
    private float c = 0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the ScoreManager persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (scoreText == null || highScoreText == null || levelText == null || levelUpText == null)
        {
            Debug.LogError("ScoreText, ComboText, LevelText, HighScoreText, or LevelUpText is not assigned in the inspector.");
            enabled = false; // Disable the script to prevent further errors
            return;
        }

        levelUpCanvasGroup = levelUpText.GetComponent<CanvasGroup>();

        // Load the high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        UpdateScoreText();
        UpdateHighScoreText();
        UpdateLevelText();
    }

    void Update()
    {
        if (comboCount > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboDuration)
            {
                Debug.Log("Combo reset due to timer expiration.");
                ResetCombo();
            }
        }

        // Check for restart input
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public int GetComboCount()
    {
        return comboCount;
    }

    public void AddScore(int value, Vector3 position)
    {
        if (value < 0)
        {
            Debug.LogWarning("Attempted to add a negative score value. Ignoring.");
            return;
        }

        try
        {
            int multiplier = comboCount > 0 ? comboCount : 1;
            checked
            {
                score += value * multiplier;
            }
        }
        catch (System.OverflowException)
        {
            score = int.MaxValue;
            Debug.LogError("Score overflow occurred. Setting score to maximum value.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An unexpected error occurred while adding score: " + ex.Message);
            return;
        }

        UpdateScoreText();

        // Check and update the high score if necessary
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreText();
        }

        comboCount++;
        comboTimer = 0f; // Reset the combo timer
        UpdateComboText(position);

        if (gameOverZoneScaler != null)
        {
            try
            {
                gameOverZoneScaler.SetScale(score);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("An unexpected error occurred while setting game over zone scale: " + ex.Message);
            }
        }

        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (score >= GetPointsForLevel(currentLevel + 1))
        {
            currentLevel++;
            OnLevelUp();
        }
    }

    int GetPointsForLevel(int level)
    {
        return Mathf.RoundToInt(a * Mathf.Pow(level, 2) + b * level + c);
    }

    void OnLevelUp()
    {
        Debug.Log("Level Up! New Level: " + currentLevel);
        UpdateLevelText();
        StartCoroutine(PlayLevelUpAnimation());
    }

    void ResetCombo()
    {
        comboCount = 0;
        comboTimer = 0f;
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        else
        {
            Debug.LogWarning("ScoreText is not assigned.");
        }
    }

    void UpdateComboText(Vector3 position)
    {
        ComboCounterText.instance.SpawnComboText(position, comboCount);
    }

    void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
        else
        {
            Debug.LogWarning("HighScoreText is not assigned.");
        }
    }

    void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
        else
        {
            Debug.LogWarning("LevelText is not assigned.");
        }
    }

    public void RestartGame()
    {
        try
        {
            score = 0;
            UpdateScoreText();
            ResetCombo();

            highScore = PlayerPrefs.GetInt("HighScore", 0);
            UpdateHighScoreText();

            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An unexpected error occurred during game restart: " + ex.Message);
        }
    }

    private IEnumerator PlayLevelUpAnimation()
    {
        // Set initial state
        levelUpText.text = "Level " + currentLevel + "!";
        levelUpCanvasGroup.alpha = 0f;
        RectTransform rectTransform = levelUpText.rectTransform;
        Vector3 startPosition = new Vector3(-Screen.width / 2, rectTransform.anchoredPosition.y, 0f);
        Vector3 endPosition = new Vector3(0, rectTransform.anchoredPosition.y, 0f);
        rectTransform.anchoredPosition = startPosition;

        // Fade in and slide in
        float duration = 1f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, t);
            levelUpCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // Pause briefly
        yield return new WaitForSeconds(1f);

        // Fade out
        duration = 0.5f;
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            levelUpCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }
    }
}
