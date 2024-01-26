using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            print($"{other.gameObject.name} exit");
            StartCoroutine(LoadNextlevel(other.gameObject));
        }
    }

    IEnumerator LoadNextlevel(GameObject go)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        go.GetComponent<PlayerInput>().enabled = false;
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        FindObjectOfType<ScenePersist>().ResetScenePersist();

        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene == SceneManager.sceneCountInBuildSettings)
        {
            FindObjectOfType<GameSession>().ResetGameSession();
        }
        else SceneManager.LoadScene(nextScene);
    }
}
