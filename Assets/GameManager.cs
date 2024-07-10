using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI restartText;
    public GameObject settingsModal;

    public Toggle vibrationToggle;
    public Toggle soundToggle;
    public Toggle musicToggle;

    public TextMeshProUGUI bombCounterText; 
     public Button bombButton; 
    public GameObject bombPrefab; 
    private int bombCounter = 0; 

    private bool isPaused = false;

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
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
        if (restartText != null)
        {
            restartText.gameObject.SetActive(false);
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
        Debug.Log("GameManager Start called");
    }

    void Update()
    {
        if (Time.timeScale == 0f && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void GameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }
        if (restartText != null)
        {
            restartText.gameObject.SetActive(true);
        }
        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Unpause the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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
        Debug.Log("Settings Modal set to: " + isPaused); 

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
     public void IncrementBombCounter()
    {
        bombCounter++;
        if (bombCounterText != null)
        {
            bombCounterText.text = bombCounter.ToString();
        }
        Debug.Log("Bomb counter incremented. Current bomb count: " + bombCounter);
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
            Debug.Log("Bomb used. Current bomb count: " + bombCounter);

            // Notify PolyhedronShooter to spawn a bomb
            PolyhedronShooter shooter = FindObjectOfType<PolyhedronShooter>();
            if (shooter != null)
            {
                shooter.SpawnBomb(bombPrefab);
            }
        }
        else
        {
            Debug.Log("No bombs available.");
        }
    }

    // Method to close the settings menu and return to the game
    public void BackToGame()
    {
        isPaused = false;
        settingsModal.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        Debug.Log("Back to Game");
    }
}
