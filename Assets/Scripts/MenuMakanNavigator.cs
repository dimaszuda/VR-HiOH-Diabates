using System.Collections;
using UnityEngine;

public class MenuMakanNavigator : MonoBehaviour
{
    public GameObject[] menuBatches;
    private int currentIndex = 0;
    public float fadeDuration = 0.5f;

    public AudioSource audioSource;
    public AudioClip clickSound;

    void Start()
    {
        ShowBatch(currentIndex);
    }

    public void NextMenu()
    {
        PlayClickSound();
        int nextIndex = (currentIndex + 1) % menuBatches.Length;
        StartCoroutine(TransitionBatch(currentIndex, nextIndex));
        currentIndex = nextIndex;
    }

    public void PrevMenu()
    {
        PlayClickSound();
        int prevIndex = (currentIndex - 1 + menuBatches.Length) % menuBatches.Length;
        StartCoroutine(TransitionBatch(currentIndex, prevIndex));
        currentIndex = prevIndex;
    }

    IEnumerator TransitionBatch(int fromIndex, int toIndex)
    {
        CanvasGroup fromCG = menuBatches[fromIndex].GetComponent<CanvasGroup>();
        CanvasGroup toCG = menuBatches[toIndex].GetComponent<CanvasGroup>();

        toCG.gameObject.SetActive(true);
        toCG.alpha = 0;

        // Fade out current
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float a = 1 - t / fadeDuration;
            fromCG.alpha = a;
            yield return null;
        }

        fromCG.alpha = 0;
        fromCG.gameObject.SetActive(false);

        // Fade in next
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float a = t / fadeDuration;
            toCG.alpha = a;
            yield return null;
        }

        toCG.alpha = 1;
    }

    private void ShowBatch(int index)
    {
        for (int i = 0; i < menuBatches.Length; i++)
        {
            bool active = (i == index);
            menuBatches[i].SetActive(active);
        }
    }

    private void PlayClickSound() {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
