using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenGenerator : MonoBehaviour
{
    public MomGenPicker momPicker;
    public DadGenPicker dadPicker;
    public bool isCreatedGen;
    public Button NextButton;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public TextMeshProUGUI message;

    public bool isGeneCreated;

    public static GenGenerator Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Reset state saat scene awal dibuka lagi
    // Reset state saat scene awal dibuka lagi
    public void ResetState()
    {
        isCreatedGen = false;

        if (NextButton == null) {
            GameObject nextObj = GameObject.Find("Panel Generate Genetic/Generate Genetic Panel/Next Button");
            if (nextObj != null)
                NextButton = nextObj.GetComponent<Button>();
        }

        if (message == null) {
            GameObject msgObj = GameObject.Find("Panel Generate Genetic/Generate Genetic Panel/warning");
            if (msgObj != null)
                message = msgObj.GetComponent<TMPro.TextMeshProUGUI>();
        }

        if (NextButton != null) 
            NextButton.gameObject.SetActive(true);

        if (message != null) 
            message.gameObject.SetActive(true);

        momPicker = Object.FindFirstObjectByType<MomGenPicker>();
        dadPicker = Object.FindFirstObjectByType<DadGenPicker>();
    }




    // Dipanggil dari tombol Generate
    public void GenerateGenes()
    {
        PlayClickSound();
        if (momPicker != null) momPicker.StartSpin();
        if (dadPicker != null) dadPicker.StartSpin();
        isCreatedGen = true;
        message.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(false);
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
