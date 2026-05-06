using System.Collections;
using UnityEngine;

public class ActivityNavigator : MonoBehaviour
{
    public GameObject[] actBatches;
    private int currentIndex = 0;

    public AudioSource audioSource;
    public AudioClip clickSound;

    void Start()
    {
        ShowBatch(currentIndex);
    }

    public void NextMenu()
    {
        PlayClickSound();
        int nextIndex = (currentIndex + 1) % actBatches.Length;
        currentIndex = nextIndex;
        ShowBatch(currentIndex);
    }

    public void PrevMenu()
    {
        PlayClickSound();
        int prevIndex = (currentIndex - 1 + actBatches.Length) % actBatches.Length;
        currentIndex = prevIndex;
        ShowBatch(currentIndex);
    }

    private void ShowBatch(int index)
    {
        for (int i = 0; i < actBatches.Length; i++)
        {
            bool active = (i == index);
            actBatches[i].SetActive(active);
        }
    }

    private void PlayClickSound() {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
