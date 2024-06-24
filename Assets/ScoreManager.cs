using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    private int score = 0;
    private int comboCount = 0;
    private float comboTimer = 0f;
    public float comboDuration = 2f; // Duration to keep the combo active

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (scoreText == null || comboText == null)
        {
            Debug.LogError("ScoreText or ComboText is not assigned in the inspector.");
            enabled = false; // Disable the script to prevent further errors
            return;
        }
        
        UpdateScoreText();
        UpdateComboText();
    }

    void Update()
    {
        if (comboCount > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboDuration)
            {
                ResetCombo();
            }
        }
    }

    public void AddScore(int value)
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

        UpdateScoreText();

        comboCount++;
        comboTimer = 0f; // Reset the combo timer
        UpdateComboText();
    }

    void ResetCombo()
    {
        comboCount = 0;
        comboTimer = 0f;
        UpdateComboText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void UpdateComboText()
    {
        if (comboText != null)
        {
            comboText.text = comboCount > 1 ? "Combo: x" + comboCount : "";
        }
    }
}
