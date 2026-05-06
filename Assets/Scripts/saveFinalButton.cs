using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class saveFinalButton : MonoBehaviour {
    public static saveFinalButton Instance;
    public MenuMakanList menuMakan; // game object untuk mendapatkan data
    public GameObject finalResult; // panel buat tampilin result
    public GameObject menuMakanPanel;
    public GameObject button;
    public GameObject collectButton;
    public StageLoader stageLoader;

    private TextMeshProUGUI karboTxt;
    private TextMeshProUGUI GITxt;
    private TextMeshProUGUI GLTxt;

    public TextMeshProUGUI karboText;
    public TextMeshProUGUI giText;
    public TextMeshProUGUI glText;
    public TextMeshProUGUI summaryText;

    public TextMeshProUGUI loadingText;
    public Image loadingIcon;
    public GameObject saveButton;

    private float karboTotal = 0f;
    private float giTotal = 0f;
    private float glTotal = 0f;
    private float avgGI = 0f;
    private string summary = "Saya tidak tahu";
    private string riskDiabetes = "";
    private bool isClicked = false;
    private string kelas = "";
    private string kelompok = "";
    private string full_name = "";
    private string number = "";


    private List<GoogleFormSender.KantinSehat> makananToSend = new List<GoogleFormSender.KantinSehat>(); // ✅

    void Start() {
        if (PlayerPrefs.HasKey("Risk of Diabetes")) {
            riskDiabetes = PlayerPrefs.GetString("Risk of Diabetes");
            Debug.Log("Risiko Diabetes: " + riskDiabetes);
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
        }
        else {
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
        // Animasi loading icon
        if (loadingIcon != null && loadingIcon.gameObject.activeSelf) {
            loadingIcon.transform.Rotate(Vector3.forward * -200f * Time.deltaTime); // arah kiri, bisa dibalik kalau mau searah jarum jam
        }
    }

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void onSaveButton() {
        finalResult.SetActive(true);
        menuMakanPanel.SetActive(false);
        button.SetActive(false);
        collectButton.SetActive(true);
        SumNutritionValue();
        CreateSummary();
        showSummary();
        isClicked = true;
    }

    public void onSendData() {
        saveButton.SetActive(false);

        ShowLoadingUI(true); // tampilkan loading

        // kumpulkan semua data makanan dulu
        makananToSend.Clear();
        for (int i = 0; i < 4; i++) {
            var namaGO = GetChildByName(menuMakan.foodItems[i], "nama makanan");
            var karboGO = GetChildByName(menuMakan.foodItems[i], "karbo-val");
            var giGO = GetChildByName(menuMakan.foodItems[i], "val GI");
            var glGO = GetChildByName(menuMakan.foodItems[i], "val GL");

            if (namaGO == null || karboGO == null || giGO == null || glGO == null) continue;

            string foodName = namaGO.GetComponent<TextMeshProUGUI>().text;
            float.TryParse(karboGO.GetComponent<TextMeshProUGUI>().text, out float karbo);
            float.TryParse(giGO.GetComponent<TextMeshProUGUI>().text, out float gi);
            float.TryParse(glGO.GetComponent<TextMeshProUGUI>().text, out float gl);

            var data = new GoogleFormSender.KantinSehat {
                targetSheet = "kantin_sehat",
                class_name = kelas,
                team = kelompok,
                full_name = full_name,
                number = number,
                food_name = foodName,
                carbohydrate = karbo,
                glycemic_index = gi,
                glycemic_load = gl
            };

            makananToSend.Add(data); // tambahkan ke list
        }

        // kirim makanan dulu, lalu baru hasil ringkasan
        GoogleFormSender.Instance.SendKantinSehatSequentially(makananToSend, () => {
            UploadKantinResult(); // lanjut kirim hasil
        });
    }


    void SumNutritionValue() {
        karboTotal = 0f;
        giTotal = 0f;
        glTotal = 0f;

        for (int i = 0; i < 4; i++) {
            var karboGO = GetChildByName(menuMakan.foodItems[i], "karbo-val");
            var giGO = GetChildByName(menuMakan.foodItems[i], "val GI");
            var glGO = GetChildByName(menuMakan.foodItems[i], "val GL");

            if (karboGO == null || giGO == null || glGO == null) continue;

            karboTxt = karboGO.GetComponent<TextMeshProUGUI>();
            GITxt = giGO.GetComponent<TextMeshProUGUI>();
            GLTxt = glGO.GetComponent<TextMeshProUGUI>();

            karboTotal += float.Parse(karboTxt.text);
            giTotal += float.Parse(GITxt.text);
            glTotal += float.Parse(GLTxt.text);
        }
    }

    void CreateSummary() {
        Debug.Log("Create summary");
        avgGI = giTotal / 4;
        if (riskDiabetes.ToLower() == "low") {
            if (karboTotal < 250 && avgGI < 60 && glTotal < 100) {
                summary = "Great Work! Menu makan kamu sudah sangat sehat dan sangat direkomendasikan";
            }
            else if (karboTotal < 250 && avgGI < 60 && glTotal > 100) {
                summary = "Cukup aman, tapi beban glikemik tinggi. Pilih porsi lebih kecil untuk jaga stabilitas gula darah";
            }
            else if (karboTotal < 250 && avgGI > 60 && glTotal < 100) {
                summary = "Karbo aman, tapi GI tinggi. Waspadai lonjakan gula darah meski beban glikemik masih terkendali";
            }
            else if (karboTotal > 250 && avgGI < 60 && glTotal < 100) {
                summary = "Total karbo tinggi, bisa memicu kelebihan energi. Tetap aman karena GI dan GL rendah";
            }
            else if (karboTotal < 250 && avgGI > 60 && glTotal > 100) {
                summary = "GI dan GL tinggi, risiko lonjakan gula darah. Kurangi porsi dan pilih makanan GI rendah";
            }
            else if (karboTotal > 250 && avgGI < 60 && glTotal > 100) {
                summary = "Karbo dan GL tinggi, potensi kelebihan energi dan glukosa. Batasi konsumsi dan imbangi dengan aktivitas";
            }
            else if (karboTotal > 250 && avgGI > 60 && glTotal < 100) {
                summary = "GI tinggi dan karbo berlebih. GL masih aman, tapi kontrol asupan agar tidak berisiko";
            }
            else if (karboTotal > 250 && avgGI > 60 && glTotal > 100) {
                summary = "Semua nilai tinggi. Menu tidak disarankan meski risiko rendah. Ganti dengan opsi lebih seimbang";
            }
        }
        else if (riskDiabetes.ToLower() == "medium") {
            if (karboTotal < 250 && avgGI < 60 && glTotal < 100) {
                summary = "Excellent! Pertahankan menu makan yang seperti ini ya karena sangat aman bagi tubuh kamu";
            }
            else if (karboTotal < 180 && avgGI < 50 && glTotal > 80) {
                summary = "Karbo dan GI aman, tapi GL tinggi. Kurangi porsi atau pilih makanan dengan glikemik lebih rendah.";
            }
            else if (karboTotal < 180 && avgGI > 50 && glTotal < 80) {
                summary = "Karbo aman, tapi GI tinggi. Potensi lonjakan gula darah, sebaiknya ganti dengan opsi GI rendah";
            }
            else if (karboTotal > 180 && avgGI < 50 && glTotal < 80) {
                summary = "Karbo sedikit berlebih, tapi GI dan GL rendah. Masih dapat ditoleransi dengan kontrol porsi";
            }
            else if (karboTotal < 180 && avgGI > 50 && glTotal > 80) {
                summary = "GL dan GI tinggi. Hindari kombinasi ini untuk mengurangi risiko lonjakan gula darah";
            }
            else if (karboTotal > 180 && avgGI < 50 && glTotal > 80) {
                summary = "Karbo dan GL tinggi meski GI aman. Perlu pengurangan porsi untuk cegah beban glikemik berlebih";
            }
            else if (karboTotal > 180 && avgGI > 50 && glTotal < 80) {
                summary = "Karbo dan GI tinggi. Meski GL aman, tetap berisiko bagi penderita risiko sedang";
            }
            else if (karboTotal > 180 && avgGI > 50 && glTotal > 80) {
                summary = "Semua indikator melebihi batas. Menu tidak disarankan untuk risiko sedang";
            }
        }
        else if (riskDiabetes.ToLower() == "high") {
            if (karboTotal < 250 && avgGI < 60 && glTotal < 100) {
                summary = "Superb! Dengan risiko gula darah kamu, kamu telah memilih menu makan yang sangat tepat";
            }
            else if (karboTotal < 120 && avgGI < 45 && glTotal > 60) {
                summary = "Karbo dan GI aman, tapi GL tinggi. Tetap perlu dikurangi untuk mencegah lonjakan gula darah";
            }
            else if (karboTotal < 120 && avgGI > 45 && glTotal < 60) {
                summary = "GI agak tinggi, tapi GL dan karbo masih aman. Pilih makanan GI rendah untuk keamanan maksimal";
            }
            else if (karboTotal > 120 && avgGI < 45 && glTotal < 60) {
                summary = "Karbo melebihi batas. Meski GI dan GL aman, tetap perlu dikurangi demi stabilitas glukosa";
            }
            else if (karboTotal < 120 && avgGI > 45 && glTotal > 60) {
                summary = "GI dan GL tinggi. Kombinasi ini berisiko, hindari untuk menjaga kestabilan gula darah";
            }
            else if (karboTotal > 120 && avgGI < 45 && glTotal > 60) {
                summary = "GL dan karbo tinggi. Tidak disarankan untuk risiko tinggi, kurangi porsi dan ganti makanan";
            }
            else if (karboTotal > 120 && avgGI > 45 && glTotal < 60) {
                summary = "Karbo dan GI tinggi meski GL aman. Masih berisiko, ganti dengan pilihan lebih seimbang";
            }
             else if (karboTotal > 120 && avgGI > 45 && glTotal > 60) {
                summary = "Semua nilai melebihi batas. Menu sangat tidak disarankan untuk risiko tinggi.";
            }
        }
        Debug.Log(summary);
    }

    void showSummary() {
        avgGI = giTotal / 4;
        karboText.text = karboTotal.ToString() + "g";
        giText.text = avgGI.ToString() + "mg/L";
        glText.text = glTotal.ToString() + "mg/L";
        summaryText.text = summary;
    }

    // ini untuk mendapatkan child gameObject dari foodItems yang dipilih
    GameObject GetChildByName(GameObject parent, string childName) {
        foreach (Transform child in parent.transform)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }
        }
        return null; // Child tidak ditemukan
    }

    public bool checkClicked() {
        return isClicked;
    }

    void UploadKantinResult() {
        var data = new GoogleFormSender.KantinResult {
            targetSheet = "hasil_kantin_sehat",
            class_name = kelas,
            team = kelompok,
            full_name = full_name,
            number = number,
            total_carbohydrate = karboTotal,
            average_gi = avgGI,
            total_gl = glTotal,
            summary = summary
        };

        GoogleFormSender.Instance.SendKantinSummary(data);
        ShowLoadingUI(false);
        stageLoader.LoadHomepageScene();
    }


    private void ShowLoadingUI(bool show) {
        if (loadingIcon != null) loadingIcon.gameObject.SetActive(show);
        if (loadingText != null) {
            loadingText.gameObject.SetActive(show);
            loadingText.text = show ? "Mengirim data..." : "";
        }
    }
}
