using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FullGameController : MonoBehaviour
{
    // Variables
    public int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveTitle;
    public static GameController gC; // Creates a singleton class for GameController
    private float musicLoopTimer;
    public GameObject popUpHelp;
    public GameObject credits;
    public float gameTimer;
    public TextMeshProUGUI gameTimerTMP = null;
    public bool waveSoundPlayed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
