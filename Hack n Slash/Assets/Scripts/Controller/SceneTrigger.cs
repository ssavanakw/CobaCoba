using TMPro;
using UnityEngine;
using System.Collections;

public class SceneTrigger : MonoBehaviour
{
    #region Private Variables
    [SerializeField] private SceneThing sceneThing;
    [SerializeField] private int sceneIndexToLoad;
    [SerializeField] private int coinsRequired = 10; // Number of coins required to trigger the scene
    #endregion

    #region Public Variables
    [SerializeField] public GameObject moreCoinsText;
    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCollecting player = collision.GetComponent<PlayerCollecting>();
            if (player != null && player.GetCoinCount() >= coinsRequired)
            {
                Debug.Log("Player entered trigger with enough coins, loading scene: " + sceneIndexToLoad);
                sceneThing.StartGameWithIndex(sceneIndexToLoad);
            }
            else
            {
                Debug.Log("Player does not have enough coins to enter the scene."); // Optionally provide feedback to the player that they need more coins
                moreCoinsText.SetActive(true);
                StartCoroutine(DeactivateMoreCoinsText());
            }
        }
    }
    private IEnumerator DeactivateMoreCoinsText()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Deactivate the moreCoinsText game object
        moreCoinsText.SetActive(false);
    }


}
