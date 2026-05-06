using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGeneticButton : MonoBehaviour {
    public GenGenerator genGenerator;
    public TextMeshProUGUI errorMessage;
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip failSound;
    public StageLoader stageLoader;

    private Button button;
    private string kelas = "";
    private string kelompok = "";
    private string full_name = "";
    private string number = "";
    private bool isKnobSaved = false;

    void Start() {
        button = GetComponent<Button>();
        errorMessage.gameObject.SetActive(false);

        // Restore status knob dari PlayerPrefs
        if (PlayerPrefs.HasKey("isKnobSaved")) {
            isKnobSaved = PlayerPrefs.GetInt("isKnobSaved") == 1;
            Debug.Log("Knob status " + isKnobSaved);
        }

        if (PlayerPrefs.HasKey("SelectedNumber")) {
            int selectedNumber = PlayerPrefs.GetInt("SelectedNumber");
            kelompok = selectedNumber.ToString();
        }

        if (PlayerPrefs.HasKey("SelectedLetter")) {
            string selectedLetter = PlayerPrefs.GetString("SelectedLetter").Trim();
            switch (selectedLetter.ToUpper()) {
                case "A": kelas = "XI MIPA A"; break;
                case "B": kelas = "XI MIPA B"; break;
                case "C": kelas = "XI MIPA C"; break;
                case "D": kelas = "XI MIPA D"; break;
                case "E": kelas = "XI MIPA E"; break;
                case "F": kelas = "XI MIPA F"; break;
                case "G": kelas = "XI MIPA G"; break;
                case "H": kelas = "XI MIPA H"; break;
                case "I": kelas = "XI MIPA I"; break;
                case "J": kelas = "XI MIPA J"; break;
                case "K": kelas = "XI MIPA K"; break;
            }
        } else {
            Debug.Log("[WARNING]: Kelas tidak ditemukan!");
        }

        if (PlayerPrefs.HasKey("full_name")) {
            full_name = PlayerPrefs.GetString("full_name");
        }

        if (PlayerPrefs.HasKey("number")) {
            number = PlayerPrefs.GetString("number");
        }
    }

    void Update() {
        if (GenGenerator.Instance != null && GenGenerator.Instance.isCreatedGen) {
            button.GetComponent<Image>().color = new Color32(255, 126, 0, 255);   // #FF7E00
        } else {
            button.GetComponent<Image>().color = new Color32(150, 75, 2, 255);   // #964B02
        }
    }

    public void OnButtonClick() {
        Debug.Log("Button save di klik");

        bool everSaved = GenGenerator.Instance.isGeneCreated;
        bool createdNow = GenGenerator.Instance != null && GenGenerator.Instance.isCreatedGen;
        bool isBack = SceneLoadHandler.Instance != null && SceneLoadHandler.Instance.isBack;

        Debug.Log("Sudah pernah pindah: " + everSaved);
        Debug.Log("Sudah pernah dibuat? " + createdNow);
        Debug.Log("Sudah pernah balik? " + isBack);

        if (createdNow) {
            PlayClickSound(successSound);

            PlayerPrefs.SetInt("isKnobSaved", 1);
            PlayerPrefs.Save();
            GeneResultData.Instance.isKnobSaved = true;
            isKnobSaved = true;

            GenGenerator.Instance.isGeneCreated = true;

            stageLoader.LoadPemilihanGeneticScene();
        }
        else if (!createdNow && everSaved && isBack) {

            errorMessage.text = "Genetik sudah dibuat, mau buat baru atau lanjut?";
            errorMessage.gameObject.SetActive(true);
            PlayClickSound(failSound);
        }
        else if (!createdNow && !everSaved && isBack) {
            errorMessage.text = "Oops, kamu belum membuat gen!";
            errorMessage.gameObject.SetActive(true);
            PlayClickSound(failSound);
        }
    }

    private void PlayClickSound(AudioClip sound) {
        if (audioSource != null && sound != null) {
            audioSource.PlayOneShot(sound);
        }
    }
}
