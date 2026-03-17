using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MissionManager : MonoBehaviour
{
    public GameObject letterOne;
    public GameObject letterTwo;
    public GameObject letterThree;

    void Start()
    {
        if (letterOne == null || letterTwo == null || letterThree == null)
        {
            Debug.LogError("One or more letter panels are not assigned in MissionManager.");
            return;
        }

        letterOne.SetActive(true);
        letterTwo.SetActive(false);
        letterThree.SetActive(false);

        
    }

    public void GoToLetterTwo()
    {
        letterOne.SetActive(false);
        letterTwo.SetActive(true);
        letterThree.SetActive(false);
    }

    public void GoToLetterThree()
    {
        letterOne.SetActive(false);
        letterTwo.SetActive(false);
        letterThree.SetActive(true);
    }

    public void BackToLetterOne()
    {
        letterOne.SetActive(true);
        letterTwo.SetActive(false);
        letterThree.SetActive(false);
    }

    public void BackToLetterTwo()
    {
        letterOne.SetActive(false);
        letterTwo.SetActive(true);
        letterThree.SetActive(false);
    }

    public void StartMission()
    {
        SceneManager.LoadScene("LevelOne");
        
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
