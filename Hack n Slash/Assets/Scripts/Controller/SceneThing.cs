using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneThing : MonoBehaviour
{
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private GameObject _endingSceneTransition;
    [SerializeField] private int nextSceneBuildIndex;

    private void Start()
    {
        _startingSceneTransition.SetActive(true);
        Invoke("DisableStartingSceneTransition", 3f);
    }

    private void Update()
    {

    }

    private void DisableStartingSceneTransition()
    {
        _startingSceneTransition.SetActive(false);
    }

    public void StartGame()
    {
        
        _endingSceneTransition.SetActive(true);
        Invoke("LoadNextLevel", 3f);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(nextSceneBuildIndex);
    }

    private void DisableEndingSceneTransition()
    {
        _endingSceneTransition.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Method to set next scene build index
    public void SetNextSceneBuildIndex(int index)
    {
        nextSceneBuildIndex = index;
    }
}
