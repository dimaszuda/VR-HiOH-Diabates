using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShowGeneResult : MonoBehaviour {
    [Header("Dad Genes Result")]
    public Image[] dadGenes;

    [Header("Mom Genes Result")]
    public Image[] momGenes;

    public TextMeshProUGUI status;
    public TextMeshProUGUI child;

    private int currentIndex = 0;
    private string kelompok = "";
    private string kelas = "Null";
    private string anak_ke = "";
    private string riskDiabetes = "";
    private string full_name = "";
    private string number = "";

    public AudioSource audioSource; // Tambahkan AudioSource
    public AudioClip clickSound;

    public Image loadingIcon;
    public TextMeshProUGUI loadingText;
    public StageLoader stageLoader;
    public GameObject button;
    public GameObject backButton;

    void Start() {
        ShowGenes(currentIndex);

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

    public void ShowGenes(int index) {
        anak_ke = (index + 1).ToString();
        child.text = "Anak " + anak_ke;
        if (GeneResultData.Instance.dadChild == null || GeneResultData.Instance.momChild == null) return;
        if (GeneResultData.Instance.dadChild[index] == null || GeneResultData.Instance.momChild[index] == null) return;

        Color[] dadColors = GeneResultData.Instance.dadChild[index];
        Color[] momColors = GeneResultData.Instance.momChild[index];

        for (int i = 0; i < dadGenes.Length; i++) {
            dadGenes[i].color = dadColors[i];
            momGenes[i].color = momColors[i];
        }

        int blackCount = CountBlackColors(dadColors) + CountBlackColors(momColors);
        if (blackCount == 6) {
            riskDiabetes = "low";
            status.text = "Rendah";
        }
        else if (blackCount > 2 && blackCount < 6) {
            riskDiabetes = "medium";
            status.text = "sedang";

        }
        else if (blackCount == 2) {
            riskDiabetes = "high";
            status.text = "tinggi";
        }
        PlayerPrefs.SetString("Risk of Diabetes", riskDiabetes);
        PlayerPrefs.Save();
    }

    private int CountBlackColors(Color[] colors) {
        int count = 0;
        foreach (Color c in colors)
        {
            if (c == Color.black)
                count++;
        }
        return count;
    }


    public void Next() {
        PlayClickSound();
        if (currentIndex < 4) {
            currentIndex++;
            ShowGenes(currentIndex); // kapitalisasi!
        }
        else if (currentIndex == 4) {
            currentIndex = 0;
            ShowGenes(currentIndex);
        }
    }

    public void Prev() {
        PlayClickSound();
        if (currentIndex > 0) {
            currentIndex--;
            ShowGenes(currentIndex); // kapitalisasi!
        }
        else if (currentIndex == 0) {
            currentIndex = 4;
            ShowGenes(currentIndex);
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void onClickSave() {
        if (GeneResultData.Instance == null || GeneResultData.Instance.dadChild == null) {
            Debug.LogError("GeneResultData belum diinisialisasi!");
            return;
        }

        button.SetActive(false);
        backButton.SetActive(false);

        var dadColors = GeneResultData.Instance.dadChild[currentIndex];
        var momColors = GeneResultData.Instance.momChild[currentIndex];

        GoogleFormSender.ChoicedGene data = new GoogleFormSender.ChoicedGene {
            targetSheet = "gen_yang_dipilih",
            class_name = kelas,
            team = kelompok,
            full_name = full_name,
            number = number,
            anak_ke = (currentIndex + 1).ToString(),
            gen_ayah_1 = ColorToName(dadColors[0]),
            gen_ayah_2 = ColorToName(dadColors[1]),
            gen_ayah_3 = ColorToName(dadColors[2]),
            gen_ibu_1 = ColorToName(momColors[0]),
            gen_ibu_2 = ColorToName(momColors[1]),
            gen_ibu_3 = ColorToName(momColors[2]),
            risk = riskDiabetes
        };

        GoogleFormSender.Instance.SendChoicedGen(data);

        List<GoogleFormSender.OptionGene> dataList = new List<GoogleFormSender.OptionGene>();

        for (int i = 0; i < 5; i++) {
            var dadColorsLoop = GeneResultData.Instance.dadChild[i];
            var momColorsLoop = GeneResultData.Instance.momChild[i];

            GoogleFormSender.OptionGene newData = new GoogleFormSender.OptionGene {
                targetSheet = "pilihan_gen",
                class_name = kelas,
                team = kelompok,
                full_name = full_name,
                number = number,
                anak_ke = (i + 1).ToString(),
                gen_ayah_1 = ColorToName(dadColorsLoop[0]),
                gen_ayah_2 = ColorToName(dadColorsLoop[1]),
                gen_ayah_3 = ColorToName(dadColorsLoop[2]),
                gen_ibu_1 = ColorToName(momColorsLoop[0]),
                gen_ibu_2 = ColorToName(momColorsLoop[1]),
                gen_ibu_3 = ColorToName(momColorsLoop[2])
            };

            dataList.Add(newData);
        }

        // Tampilkan loading
        ShowLoadingUI(true);

        GoogleFormSender.Instance.SendOptionGenesSequentially(dataList, () => {
            Debug.Log("✅ Semua data berhasil dikirim.");
            ShowLoadingUI(false);
            stageLoader.LoadHomepageScene();
        });
    }


    private string ColorToName(Color color) {
        if (color == Color.red) return "merah";
        if (color == Color.black) return "hitam";
        if (color == Color.blue) return "biru";
        if (color == Color.green) return "hijau";
        if (color == Color.yellow) return "kuning";
        // tambahkan lagi jika kamu pakai warna lain

        return "tidak diketahui";
    }

    private void ShowLoadingUI(bool show) {
        if (loadingIcon != null)
            loadingIcon.gameObject.SetActive(show);
        if (loadingText != null) {
            loadingText.gameObject.SetActive(show);
            loadingText.text = show ? "Mengirim data..." : "";
        }
    }
}
