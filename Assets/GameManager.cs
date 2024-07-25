using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
  public static GameManager instance;

  public GameObject gameOverPanel; // Game Over panel to display
  public TextMeshProUGUI gameOverText;
  public TextMeshProUGUI restartText;
  public GameObject settingsModal;
  public GameObject dailyChallengesPanel;
  public GameObject rewardsShop;
  public Toggle vibrationToggle;
  public Toggle soundToggle;
  public Toggle musicToggle;
  public TextMeshProUGUI bombCounterText;
  public Button bombButton;
  public GameObject bombPrefab;

  private int bombCounter = 0;
  private Dictionary<int, int> polyhedronCounts; // Dictionary to keep track of polyhedron counts
  public TextMeshProUGUI polyhedronCountsText; // Text component to display the counts
  private bool isPaused = false;
  private Coroutine shakeCoroutine; // Reference to the shake coroutine

  // =======================================
  // Unity Standard Methods
  // =======================================

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  void Start()
  {
    polyhedronCounts = new Dictionary<int, int>(); // Initialize the dictionary

    if (gameOverPanel != null)
    {
      gameOverPanel.SetActive(false); // Ensure the panel is initially hidden
    }
    if (settingsModal != null)
    {
      settingsModal.SetActive(false); // Ensure the modal is initially hidden
    }
    if (bombCounterText != null)
    {
      bombCounterText.text = bombCounter.ToString();
    }
    if (bombButton != null)
    {
      bombButton.onClick.AddListener(UseBomb);
    }
    if (dailyChallengesPanel != null)
    {
      dailyChallengesPanel.SetActive(false); // Ensure the daily reward panel is initially hidden
    }
    if (rewardsShop != null)
    {
      rewardsShop.SetActive(false); // Ensure the rewards shop panel is initially hidden
    }
    //Debug.Log("GameManager Start called");
  }

  void Update()
  {
    // Check if the "R" key is pressed to restart the game when it is paused
    if (Time.timeScale == 0f && Input.GetKeyDown(KeyCode.R))
    {
      RestartGame();
    }

    // Check if the "G" key is pressed to trigger the game over sequence
    if (Input.GetKeyDown(KeyCode.G))
    {
      GameOver();
    }
  }

  // =======================================
  // Game Control Methods
  // =======================================

  public void GameOver()
  {
    if (gameOverPanel != null)
    {
      gameOverPanel.SetActive(true); // Show the game over panel
    }
    if (gameOverText != null)
    {
      gameOverText.gameObject.SetActive(true);
    }
    if (restartText != null)
    {
      restartText.gameObject.SetActive(true);
    }
    if (polyhedronCountsText != null)
    {
      polyhedronCountsText.gameObject.SetActive(true);
      DisplayPolyhedronCounts(); // Display the counts on the game over screen
    }
    Time.timeScale = 0f; // Pause the game
  }

  public void RestartGame()
  {
    Time.timeScale = 1f; // Unpause the game
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  // =======================================
  // Settings Modal Methods
  // =======================================

  public void ToggleSettings()
  {
    isPaused = !isPaused;
    Debug.Log("ToggleSettings called. isPaused: " + isPaused);

    if (settingsModal == null)
    {
      Debug.LogError("settingsModal is not assigned!");
      return;
    }

    settingsModal.SetActive(isPaused);
    //Debug.Log("Settings Modal set to: " + isPaused);

    if (isPaused)
    {
      Time.timeScale = 0f; // Pause the game
    }
    else
    {
      Time.timeScale = 1f; // Resume the game
    }
  }

  public void ToggleVibration()
  {
    // Implement vibration toggle logic here
    Debug.Log("Vibration: " + vibrationToggle.isOn);
  }

  public void ToggleSound()
  {
    // Implement sound toggle logic here
    Debug.Log("Sound: " + soundToggle.isOn);
  }

  public void ToggleMusic()
  {
    // Implement music toggle logic here
    Debug.Log("Music: " + musicToggle.isOn);
  }

  // =======================================
  // Bomb Management Methods
  // =======================================

  public void IncrementBombCounter()
  {
    bombCounter++;
    if (bombCounterText != null)
    {
      bombCounterText.text = bombCounter.ToString();
    }
    //Debug.Log("Bomb counter incremented. Current bomb count: " + bombCounter);
  }

  public void UseBomb()
  {
    if (bombCounter > 0)
    {
      bombCounter--;
      if (bombCounterText != null)
      {
        bombCounterText.text = bombCounter.ToString();
      }
      //Debug.Log("Bomb used. Current bomb count: " + bombCounter);

      // Notify PolyhedronShooter to spawn a bomb
      PolyhedronShooter shooter = FindObjectOfType<PolyhedronShooter>();
      if (shooter != null)
      {
        shooter.SpawnBomb(bombPrefab);
      }
    }
    else
    {
      //Debug.Log("No bombs available.");

      // Start the shake effect if there are no bombs left
      if (shakeCoroutine == null)
      {
        shakeCoroutine = StartCoroutine(ShakeButton(bombButton.transform));
      }
    }
  }

  private IEnumerator ShakeButton(Transform buttonTransform)
  {
    Vector3 originalPosition = buttonTransform.localPosition;
    float shakeDuration = 0.5f; // Duration of the shake
    float shakeMagnitude = 5f; // Magnitude of the shake
    float elapsed = 0.0f;

    while (elapsed < shakeDuration)
    {
      float x = Random.Range(-1f, 1f) * shakeMagnitude;
      float y = Random.Range(-1f, 1f) * shakeMagnitude;

      buttonTransform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

      elapsed += Time.deltaTime;

      yield return null;
    }

    buttonTransform.localPosition = originalPosition; // Reset to original position
    shakeCoroutine = null; // Reset the coroutine reference
  }

  // =======================================
  // Polyhedron Count Methods
  // =======================================

  public void IncrementPolyhedronCount(int value)
  {
    if (polyhedronCounts.ContainsKey(value))
    {
      polyhedronCounts[value]++;
    }
    else
    {
      polyhedronCounts[value] = 1;
    }
    //Debug.Log($"Polyhedron count for value {value} incremented. Current count: {polyhedronCounts[value]}");
  }

  private void DisplayPolyhedronCounts()
  {
    if (polyhedronCountsText == null) return;

    // Clear previous text
    polyhedronCountsText.text = "";

    foreach (var entry in polyhedronCounts)
    {
      polyhedronCountsText.text += $"{entry.Key} : {entry.Value}\n";
    }
  }

  // =======================================
  // Navigation Methods
  // =======================================

  public void BackToGame()
  {
    isPaused = false;
    settingsModal.SetActive(false);
    Time.timeScale = 1f; // Resume the game
    Debug.Log("Back to Game");
  }

  public void OpenDailyChallengesPanel()
  {
    if (dailyChallengesPanel != null)
    {
      dailyChallengesPanel.SetActive(true);
      Time.timeScale = 0f; // Pause the game
    }
    Debug.Log("Daily Challenge Panel Opened");
  }

  public void CloseDailyChallengesPanel()
  {
    if (dailyChallengesPanel != null)
    {
      dailyChallengesPanel.SetActive(false);
      Time.timeScale = 1f; // Resume the game
    }
    Debug.Log("Daily Reward Panel Closed");
  }

  public void OpenRewardsShop()
  {
    if (rewardsShop != null)
    {
      rewardsShop.SetActive(true);
      Time.timeScale = 0f; // Pause the game
    }
    Debug.Log("Rewards Shop Opened");
  }

  public void CloseRewardsShop()
  {
    if (rewardsShop != null)
    {
      rewardsShop.SetActive(false);
      Time.timeScale = 1f; // Resume the game
    }
    Debug.Log("Rewards Shop Closed");
  }
}
