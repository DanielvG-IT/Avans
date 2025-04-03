//Dit scherm heeft alleen vier buttons: Laad monitor scene, Laad spel scene, Logout en Open account.

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour
{
    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void switchSceneWithDelay(string sceneName)
    {
        switchSceneWithDelay(sceneName);
    }

    public IEnumerator SwitchSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}