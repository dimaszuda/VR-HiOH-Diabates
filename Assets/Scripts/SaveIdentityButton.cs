using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveIdentityButton : MonoBehaviour {
    public ButtonSelector selector;
    public StageLoader stageLoader;
    public TextMeshProUGUI message;
    public Color disabledColor = Color.gray;
    public Color enabledColor = Color.green;
    public TMP_InputField name;
    public TMP_InputField number;

    private Button button;
    private bool hasShownError = false;

    void Start() {
        message.gameObject.SetActive(false);
        button = GetComponent<Button>();

        // biarkan tombol interactable = true di Inspector
        // warna default sesuai kondisi awal
        button.GetComponent<Image>().color = disabledColor;

        // Tambahkan listener supaya update terus kalau input berubah
        name.onValueChanged.AddListener(delegate { ValidateInputs(); });
        number.onValueChanged.AddListener(delegate { ValidateInputs(); });
    }

    void Update() {
        ValidateInputs();
    }

    void ValidateInputs() {
        if (selector == null) return;

        bool teamSelected = selector.isTeamSelected;
        bool classSelected = selector.isClassSelected;
        bool nameFilled = !string.IsNullOrEmpty(name.text.Trim());
        bool numberFilled = !string.IsNullOrEmpty(number.text.Trim());

        if (teamSelected && classSelected && nameFilled && numberFilled) {
            // Semua syarat terpenuhi
            message.gameObject.SetActive(true);
            message.text = "Aku siap bermain permainan ini";
            message.color = Color.green;
            button.GetComponent<Image>().color = enabledColor;
            hasShownError = false;
        }
        else {
            // Belum lengkap
            button.GetComponent<Image>().color = disabledColor;

            if (hasShownError) {
                message.gameObject.SetActive(true);
                message.text = "Oops, data kamu belum lengkap. Silahkan dilengkapi dulu!";
                message.color = Color.red;
            }
            else {
                message.gameObject.SetActive(false);
            }
        }
    }

    public void OnButtonClick() {
        if (selector == null) return;

        bool teamSelected = selector.isTeamSelected;
        bool classSelected = selector.isClassSelected;
        bool nameFilled = !string.IsNullOrEmpty(name.text.Trim());
        bool numberFilled = !string.IsNullOrEmpty(number.text.Trim());

        if (teamSelected && classSelected && nameFilled && numberFilled) {
            // Simpan ke PlayerPrefs
            PlayerPrefs.SetString("full_name", name.text.Trim());
            PlayerPrefs.SetString("number", number.text.Trim());
            PlayerPrefs.Save();

            Debug.Log("Saved full_name: " + name.text + ", number: " + number.text);

            stageLoader.LoadGenerateGeneticScene();
        } 
        else {
            // User klik tombol tapi belum lengkap → trigger pesan error
            hasShownError = true;
            ValidateInputs();
        }
    }
}
