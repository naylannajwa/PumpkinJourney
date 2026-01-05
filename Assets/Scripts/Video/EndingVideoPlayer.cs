using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

namespace PumpkinJourney.Video
{
    public class EndingVideoPlayer : MonoBehaviour
    {
        [Header("Video Settings")]
        public VideoPlayer videoPlayer;
        public VideoClip endingVideo;
        public string nextSceneName = "homePage"; // Return to homePage after ending

    [Header("Skip Settings")]
    public KeyCode skipKey = KeyCode.Escape;
    public bool allowSkip = true;

    [Header("Fade Settings")]
    public UnityEngine.UI.Image fadeImage;
    public float fadeDuration = 0.5f;

        void Start()
        {
            if (videoPlayer == null)
            {
                videoPlayer = GetComponent<VideoPlayer>();
            }

            if (videoPlayer != null && endingVideo != null)
            {
                // Pause BGM before playing ending video
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PauseBGM();
                    Debug.Log("🎵 BGM paused for ending video");
                }

                videoPlayer.clip = endingVideo;
                videoPlayer.Play();

                // Subscribe to video end event
                videoPlayer.loopPointReached += OnVideoEnd;

                Debug.Log("🎬 Ending video started");
            }
            else
            {
                Debug.LogError("❌ Ending VideoPlayer or VideoClip is missing!");
                LoadNextScene();
            }
        }

        void Update()
        {
            // Allow player to skip ending
            if (allowSkip && Input.GetKeyDown(skipKey))
            {
                SkipEnding();
            }
        }

        void OnVideoEnd(VideoPlayer vp)
        {
            Debug.Log("✅ Ending video completed - returning to homePage");

            // Stop video rendering to prevent blue screen
            vp.Stop();
            vp.enabled = false;

            // Note: BGM will automatically switch to HomePageBGM when homePage scene loads
            // No need to manually resume BGM here

            // Use fade transition if fade image is assigned
            if (fadeImage != null && fadeDuration > 0)
            {
                StartCoroutine(FadeAndLoadScene());
            }
            else
            {
                LoadNextScene();
                // Deactivate after scene loading is initiated
                vp.gameObject.SetActive(false);
            }
        }

        private System.Collections.IEnumerator FadeAndLoadScene()
        {
            Debug.Log($"Starting fade transition ({fadeDuration}s)");

            // Ensure fade image is active and visible
            fadeImage.gameObject.SetActive(true);
            Color startColor = fadeImage.color;
            Color endColor = new Color(0, 0, 0, 1f); // Fully opaque black

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                fadeImage.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }

            // Ensure fully faded
            fadeImage.color = endColor;

            Debug.Log("Fade complete, loading next scene");
            LoadNextScene();

            // Deactivate video object after everything is done
            videoPlayer.gameObject.SetActive(false);
        }

        void SkipEnding()
        {
            Debug.Log("⏭️ Ending video skipped by player");
            LoadNextScene();
        }

        void LoadNextScene()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }

            if (!string.IsNullOrEmpty(nextSceneName))
            {
                Debug.Log($"🔄 Loading scene: {nextSceneName}");

                // Check if scene exists in Build Settings
                bool sceneExists = false;
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                    string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                    if (sceneNameInBuild == nextSceneName)
                    {
                        sceneExists = true;
                        Debug.Log($"✅ Scene '{nextSceneName}' found in Build Settings at index {i}");
                        break;
                    }
                }

                if (sceneExists)
                {
                    try
                    {
                        SceneManager.LoadScene(nextSceneName);
                        Debug.Log($"✅ Successfully initiated load of scene: {nextSceneName}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"❌ Failed to load scene {nextSceneName}: {e.Message}");
                        // Fallback: try to load by index 1 (usually homePage)
                        if (SceneManager.sceneCountInBuildSettings > 1)
                        {
                            SceneManager.LoadScene(1);
                            Debug.Log("🔄 Fallback: Loading scene at build index 1");
                        }
                    }
                }
                else
                {
                    Debug.LogError($"❌ Scene '{nextSceneName}' not found in Build Settings!");
                    Debug.Log("📋 Available scenes in Build Settings:");
                    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                    {
                        string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                        string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                        Debug.Log($"  {i}: {sceneNameInBuild}");
                    }

                    // Fallback: try to load by index 1
                    if (SceneManager.sceneCountInBuildSettings > 1)
                    {
                        SceneManager.LoadScene(1);
                        Debug.Log("🔄 Fallback: Loading scene at build index 1");
                    }
                }
            }
            else
            {
                Debug.LogError("❌ Next scene name is empty!");
            }
        }

        void OnDestroy()
        {
            // Cleanup event subscription
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached -= OnVideoEnd;
            }
        }
    }
}
