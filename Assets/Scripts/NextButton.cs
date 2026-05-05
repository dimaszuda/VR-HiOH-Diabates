using UnityEngine;
using UnityEngine.UI;

public class NextButton : MonoBehaviour {
    public GenGenerator genGenerator;
    public StageLoader stageLoader;
    public AudioSource audioSource;
    public AudioClip clickSound;

    private Button button;

    void Awake() {
        button = GetComponent<Button>();
    }

    void Start() {
        // hanya tampil kalau sudah pernah ke Pemilihan Genetik
        if (GenGenerator.Instance != null && GenGenerator.Instance.isGeneCreated) {
            gameObject.SetActive(true);
        } else {
            gameObject.SetActive(false);
        }
    }

    public void OnButtonClick() {
        // tombol Next bekerja kalau belum ada gen baru di-generate
        PlayClickSound();
        stageLoader.LoadPemilihanGeneticScene();
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
