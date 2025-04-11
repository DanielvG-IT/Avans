using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour
{
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void DelayedSwitchScene(string sceneName)
    {
        StartCoroutine(SwitchSceneWithDelay(sceneName));
    }

    private IEnumerator SwitchSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}