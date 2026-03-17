using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public TMP_Text letterText;

    [TextArea(5, 15)]
    public string message;

    public float typingSpeed = 0.04f;
    public float punctuationPause = 0.15f;

    public AudioSource typingAudio;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isFinished = false;

    void OnEnable()
    {
        StartTyping();
    }

    public void StartTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(ShowLetter());
    }

    IEnumerator ShowLetter()
    {
        isTyping = true;
        isFinished = false;

        letterText.text = message;
        letterText.ForceMeshUpdate();

        int totalCharacters = letterText.textInfo.characterCount;
        letterText.maxVisibleCharacters = 0;

        yield return new WaitForSecondsRealtime(1f);

        if (typingAudio != null)
            typingAudio.Play();

        for (int i = 0; i <= totalCharacters; i++)
        {
            letterText.maxVisibleCharacters = i;

            if (i < message.Length && (message[i] == '.' || message[i] == ',' || message[i] == '!'))
                yield return new WaitForSecondsRealtime(punctuationPause);
            else
                yield return new WaitForSecondsRealtime(typingSpeed);
        }

        if (typingAudio != null)
            typingAudio.Stop();

        isTyping = false;
        isFinished = true;
    }

    public void CompleteTextImmediately()
    {
        if (!isTyping)
            return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        letterText.text = message;
        letterText.ForceMeshUpdate();
        letterText.maxVisibleCharacters = letterText.textInfo.characterCount;

        if (typingAudio != null)
            typingAudio.Stop();

        isTyping = false;
        isFinished = true;
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    public bool IsFinished()
    {
        return isFinished;
    }
}