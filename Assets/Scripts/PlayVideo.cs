using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoGameObject;
    public Button playButton;

    private bool isPlaying = false;

    private void Start()
    {
        // Add a listener to the video player to detect when it is finished playing
        videoPlayer.loopPointReached += OnVideoFinished;

        // Set the video game object inactive to start
        videoGameObject.SetActive(false);

        // Add a listener to the play button
        playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        if (!isPlaying)
        {
            // Show the video game object and play the video if it is not playing
            videoGameObject.SetActive(true);
            videoPlayer.Play();
            isPlaying = true;
        }
        else
        {
            // Stop the video and hide the video game object if it is playing
            videoPlayer.Stop();
            videoGameObject.SetActive(false);
            isPlaying = false;
        }
    }

    private void OnVideoFinished(VideoPlayer player)
    {
        // Stop the video and hide the video game object when the video is finished
        videoPlayer.Stop();
        videoGameObject.SetActive(false);
        isPlaying = false;
    }
}