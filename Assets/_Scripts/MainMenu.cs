using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public GameObject controlsPanel;

    void Start()
    {
        controlsPanel.SetActive(false);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }
    
    
    public void PlayGame()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GameScene");
    }
    public void QuitGame()
    {
        StartCoroutine(QuitAfterDelay());
    }

    IEnumerator QuitAfterDelay()
    {
        Debug.Log("Exiting game in 2 seconds...");

        yield return new WaitForSeconds(1f);

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}