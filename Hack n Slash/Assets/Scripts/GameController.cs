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

        //// Initialize progress bar
        //progressAmount = 0;
        //progressSlider.value = 0;
        //Gem.OnGemCollect += IncreaseProgressAmount;
    }

    // On Scene Load, do the following...
    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            score = 0;
        }

        GameObject _scoreTextGO;
        if ((_scoreTextGO = GameObject.Find("Score Number")) != null)
        {
            scoreText = _scoreTextGO.GetComponent<TextMeshProUGUI>();
            AdvanceScore(0);
        }

        GameObject _waveTitleGO;
        if ((_waveTitleGO = GameObject.Find("Wave Title")) != null) waveTitle = _waveTitleGO.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        musicLoopTimer -= Time.deltaTime;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public void AdvanceScore(int s)
    {
        score += s;
        scoreText.text = score.ToString();
    }

    public void LoadScene(int level)
    {
        SceneManager.LoadScene(level);
    }

    public void ActivatePopUpHelp()
    {
        Instantiate(popUpHelp, GameObject.Find("Canvas").transform);
    }

    public void DeactivatePopUpHelp()
    {
        Destroy(GameObject.Find("PopUpHelp(Clone)"));
    }

    public void ActivateCredits()
    {
        Instantiate(credits, GameObject.Find("Canvas").transform);
    }

    public void DeactivateCredits()
    {
        Destroy(GameObject.Find("Credits(Clone)"));
    }

    public void GameOver()
    {
        SceneManager.LoadScene(2);
    }

    public void SetWaveTitle(int wave)
    {
        waveTitle.text = "Wave " + wave;
    }

    public void DeleteWaveTitle()
    {
        waveTitle.text = null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;
        if (progressAmount >= 100)
        {
            Debug.Log("Level Complete");
        }
    }
}
