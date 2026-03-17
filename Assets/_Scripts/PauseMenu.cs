using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject levelPanel;

    public AudioSource levelMusic;
    public AudioSource menuMusic;

    private bool isPaused = false;

    void Start()
    {
        menuPanel.SetActive(false);
        levelPanel.SetActive(true);

        menuMusic.Stop();
        levelMusic.Play();

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        menuPanel.SetActive(true);
        levelPanel.SetActive(false);

        levelMusic.Stop();
        menuMusic.Play();

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        levelPanel.SetActive(true);

        menuMusic.Stop();
        levelMusic.Play();

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame()
    {
        StartCoroutine(QuitAfterDelay());
    }

    IEnumerator QuitAfterDelay()
    {
        Debug.Log("Exiting game in 1 second...");

        yield return new WaitForSecondsRealtime(1f);

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}