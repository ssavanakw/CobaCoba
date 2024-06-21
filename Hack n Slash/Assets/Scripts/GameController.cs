using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Variables
    [Header("Score and UI")]
    public int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveTitle;

    [Header("Progress Bar")]
    [SerializeField] public Slider progressSlider;
    [SerializeField] int progressAmount;

    [Header("Music and Sounds")]
    private SoundController sC;
    private float musicLoopTimer;

    [Header("Pop-ups and Credits")]
    public GameObject popUpHelp;
    public GameObject credits;

    [Header("Game Timer")]
    public float gameTimer;
    public TextMeshProUGUI gameTimerTMP = null;

    public bool waveSoundPlayed;
    public static GameController gC; // Singleton instance

    // Initialization
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        DontDestroyOnLoad(gameObject); // Persist between scenes
        sC = GetComponent<SoundController>();
        // Assign singleton - destroy all duplicates in existence
        if (gC == null) gC = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        score = 0;
        musicLoopTimer = 50f;
        gameTimer = 3f;

        // Initialize progress bar
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
    }

    // On Scene Load, do the following...
    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        // If entering Title scene, play title music
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //sC.PlayMusic(sC.titleMusic, 0.2f);
        }
        // If entering Arena scene, set score to 0 - stops currently playing music; Arena music is played through Spawner script
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            score = 0;
            //sC.musicSource.Stop();
            //sC.songCounter = 0;
            // Assign Game Timer GUI element
            gameTimerTMP = GameObject.Find("Game Timer").GetComponent<TextMeshProUGUI>();
        }
        // If entering Game Over scene, play game over music + monster laugh
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            //sC.PlayMusic(sC.gameOverMusic, 0.2f);
            //sC.Play(sC.sEffectSource, new Vector3(0, 0, -10), sC.gameOverLaugh, 1f);
            gameTimerTMP = null;
        }
        // If entering Victory scene, play title music
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            //sC.PlayMusic(sC.titleMusic, 0.2f);
            gameTimerTMP = null;
        }
        // Get reference to score number GUI element if it exists in scene and display the current/saved score 
        GameObject _scoreTextGO;
        if ((_scoreTextGO = GameObject.Find("Score Number")) != null)
        {
            scoreText = _scoreTextGO.GetComponent<TextMeshProUGUI>();
            AdvanceScore(0);
        }
        // Get reference to wave title GUI element
        GameObject _waveTitleGO;
        if ((_waveTitleGO = GameObject.Find("Wave Title")) != null) waveTitle = _waveTitleGO.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Checks every second to see if music has stopped and if so, loops the track
        musicLoopTimer -= Time.deltaTime;
        if (musicLoopTimer < 0)
        {
            //// Title Scene
            //if (SceneManager.GetActiveScene().buildIndex == 0 && sC.musicSource.isPlaying == false) sC.PlayMusic(sC.titleMusic, 0.2f);
            //// Game Over Scene
            //if (SceneManager.GetActiveScene().buildIndex == 2 && sC.musicSource.isPlaying == false) sC.PlayMusic(sC.gameOverMusic, 0.2f);
            //// Victory Scene
            //if (SceneManager.GetActiveScene().buildIndex == 3 && sC.musicSource.isPlaying == false) sC.PlayMusic(sC.titleMusic, 0.2f);
            //musicLoopTimer = 1f;
        }

        // Displays the game timer if it exists in the scene and has a value greater than 0
        if (gameTimerTMP != null)
        {
            gameTimer -= Time.deltaTime;
            if (gameTimer >= 0) gameTimerTMP.text = gameTimer.ToString("F1");
            else gameTimerTMP.text = "";
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public void AdvanceScore(int s) // Adds s to score and displays the new score; used upon loading and each time a monster is slain in Enemy.CheckDeath()
    {
        score += s;
        scoreText.text = score.ToString();
    }

    public void LoadScene(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void ActivatePopUpHelp() // Displays the "How to Play" pop-up in the title screen
    {
        Instantiate(popUpHelp, GameObject.Find("Canvas").transform);
    }

    public void DeactivatePopUpHelp()
    {
        Destroy(GameObject.Find("PopUpHelp(Clone)"));
    }

    public void ActivateCredits() // Displays the Credits pop-up in the title screen
    {
        Instantiate(credits, GameObject.Find("Canvas").transform);
    }

    public void DeactivateCredits()
    {
        Destroy(GameObject.Find("Credits(Clone)"));
    }

    public void GameOver() // Invoked after player death 
    {
        SceneManager.LoadScene(2);
    }

    public void SetWaveTitle(int wave)
    {
        waveTitle.text = "Wave " + wave;
    }

    public void DeleteWaveTitle() // Invoked 3.75 seconds after wave title is set - see Spawner script
    {
        waveTitle.text = null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void IncreaseProgressAmount(int amount) // Increases progress and updates the progress bar
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;
        if (progressAmount >= 100)
        {
            Debug.Log("Level Complete");
        }
    }
}
