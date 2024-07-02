using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    public float splashScreenDuration = 3.0f;

    void Start()
    {
        StartCoroutine(ShowSplashScreen());
    }

    IEnumerator ShowSplashScreen()
    {
        yield return new WaitForSeconds(splashScreenDuration);
        SceneManager.LoadScene("GameBoard"); // Replace with your actual game scene name
    }
}
