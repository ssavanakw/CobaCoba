using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private SceneThing sceneThing;
    [SerializeField] private int sceneIndexToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger, loading scene: " + sceneIndexToLoad);
            sceneThing.StartGameWithIndex(sceneIndexToLoad);
        }
    }
}
