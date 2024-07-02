using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoLogoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("SplashScreenScene");
    }
}
