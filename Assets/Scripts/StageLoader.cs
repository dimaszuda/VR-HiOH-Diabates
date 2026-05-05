using UnityEngine;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    public AudioSource audioSource; // Tambahkan AudioSource
    public AudioClip clickSound;    // Tambahkan AudioClip

    public void LoadPanelMulaiScene()
    {
        PlayClickSound();
        SceneManager.LoadScene("Panel Mulai");
    }

    public void LoadHomepageScene()
    {
        PlayClickSound();
        SceneManager.LoadScene("Homepage");
    }

    public void LoadGenerateGeneticScene()
    {
        PlayClickSound();
        SceneManager.LoadScene("Generate Genetik");
    }

    public void LoadPemilihanGeneticScene()
    {
        PlayClickSound();
        SceneManager.LoadScene("Pemilihan Genetik");
    }

    public void LoadMainGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("Main Game");
    }

    public void LoadKantinSehat()
    {
        PlayClickSound();
        SceneManager.LoadScene("Kantin Sehat");
    }

    public void Keluar()
    {
        PlayClickSound();
        Application.Quit();
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
