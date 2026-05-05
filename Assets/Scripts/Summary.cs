using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Summary : MonoBehaviour {
    public TextMeshProUGUI level;
    public TextMeshProUGUI kelas;
    public Image knobClass;
    public TextMeshProUGUI nama;
    public Image knobRisk;
    public TextMeshProUGUI team;
    public Image knobTeam;

    private Color levelColor = Color.green;

    void Start() {
        string riskDiabetes = PlayerPrefs.GetString("Risk of Diabetes").ToLower();
        Debug.Log("RISIKO DIABETS " + riskDiabetes);
        
        switch (riskDiabetes) {
            case "low":
                levelColor = Color.green;
                level.text = "Level Rendah";
                break;
            case "medium":
                levelColor = Color.yellow;
                level.text = "Level Sedang";
                break;
            case "high":
                levelColor = Color.red;
                level.text = "Level Tinggi";
                break;
        }

        level.color = levelColor;
        knobClass.color = levelColor;
        knobRisk.color = levelColor;
        knobTeam.color = levelColor;

        if (PlayerPrefs.HasKey("full_name")) {
            string full_name = PlayerPrefs.GetString("full_name");

            // Split jadi array kata
            string[] parts = full_name.Split(' ');

            string displayName;

            if (parts.Length <= 2) {
                // 1 atau 2 kata → tampilkan apa adanya
                displayName = full_name;
            } else {
                // lebih dari 2 kata → ambil 2 kata pertama
                displayName = parts[0] + " " + parts[1];
            }

            nama.text = displayName;
            nama.color = levelColor;
        }


        if (PlayerPrefs.HasKey("SelectedNumber")) {
            int selectedNumber = PlayerPrefs.GetInt("SelectedNumber");
            team.text = "Kelompok " + selectedNumber.ToString();
            team.color = levelColor;
        }

        if (PlayerPrefs.HasKey("SelectedLetter")) {
            string selectedLetter = PlayerPrefs.GetString("SelectedLetter").Trim();
            switch (selectedLetter.ToUpper()) {
                case "A": kelas.text = "XI MIPA A"; break;
                case "B": kelas.text = "XI MIPA B"; break;
                case "C": kelas.text = "XI MIPA C"; break;
                case "D": kelas.text = "XI MIPA D"; break;
                case "E": kelas.text = "XI MIPA E"; break;
                case "F": kelas.text = "XI MIPA F"; break;
                case "G": kelas.text = "XI MIPA G"; break;
                case "H": kelas.text = "XI MIPA H"; break;
                case "I": kelas.text = "XI MIPA I"; break;
                case "J": kelas.text = "XI MIPA J"; break;
                case "K": kelas.text = "XI MIPA K"; break;
            }
            kelas.color = levelColor;
        }
        else {
            Debug.Log("[WARNING]: Kelas tidak ditemukan!");
        }
    }
}