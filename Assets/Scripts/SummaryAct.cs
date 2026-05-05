using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class SummaryAct : MonoBehaviour {
    public TextMeshProUGUI summaryText;
    private float endHour = 0f;

    public string summary = "";

    public static SummaryAct Instance;

    void Start() {
        if (PlayerPrefs.HasKey("End Hour")) {
            endHour = PlayerPrefs.GetFloat("End Hour");
        }
        List<float> data = GlucoseDataStore.collectedData;
        CreateSummary(data);
    }

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void CreateSummary(List<float> data) {
        if (data == null || data.Count == 0) {
            summaryText.text = "Belum ada data yang dikumpulkan.";
            return;
        }

        if (Mathf.Abs(endHour - 24f) > 0.01f) {
            float last = data.Last();
            if (last < 50) {
                summary = "Wahh, gula darah kamu terlalu rendah, coba makan dan minum sesuatu.";
                summaryText.text = summary;
            } else if (last >= 349) {
                summary = "Wahhh, gula darah kamu terlalu tinggi, hati-hati kalau makan dan beraktivitas ya.";
                summaryText.text = summary;
            }
            return;
        }

        // 1. Persentase zona aman
        int safeCount = data.Count(d => d >= 70 && d <= 180);
        float safePercentage = (float)safeCount / data.Count * 100f;

        // 2. Fluktuasi per 1 jam (setiap 8 detik dunia nyata)
        int fluctuationCount = 0;
        float threshold = 30f;
        for (int i = 1; i < data.Count; i++) {
            if (Mathf.Abs(data[i] - data[i - 1]) > threshold) {
                fluctuationCount++;
            }
        }

        // 3. Tren beberapa jam terakhir
        int recentWindow = 50; // misalnya 10 jam terakhir
        List<float> recentData = data.Skip(Mathf.Max(0, data.Count - recentWindow)).ToList();
        float recentAvg = recentData.Average();
        float overallAvg = data.Average();
        Debug.Log("Average Glucose:" + overallAvg);
        string trend = "stable";
        if (recentAvg - overallAvg > 20f) trend = "naik";
        else if (overallAvg - recentAvg > 20f) trend = "turun";

        // Zona aman
        if (safePercentage >= 80f) {
            summary += "Gula darah kamu stabil, kamu berada di zona aman selama " + Mathf.RoundToInt(safePercentage) + "% dari waktu. ";
        } else if (safePercentage >= 60f) {
            summary += "Cukup stabil, kamu berada di zona aman selama " + Mathf.RoundToInt(safePercentage) + "% dari waktu. ";
        } else {
            summary += "Perlu diperhatikan! Hanya " + Mathf.RoundToInt(safePercentage) + "% waktumu di zona aman. ";
        }

        // Fluktuasi
        if (fluctuationCount <= 10) {
            summary += "Gula darah kamu cukup konsisten. ";
        } else if (fluctuationCount <= 30) {
            summary += "Gula darah kamu cukup fluktuatif ";
        } else {
            summary += "Gula darah kamu sangat fluktuatif, cobalah untuk memperbaiki pola makan Anda atau istirahat yang cukup. ";
        }

        // Tren
        if (trend == "naik") {
            summary += "Tren gula darah terkini menunjukkan peningkatan.";
        } else if (trend == "down") {
            summary += "Tren gula darah terbaru menunjukkan penurunan";
        } else {
            summary += "Tidak ada tren yang signifikan, tetap stabil di akhir waktu.";
        }

        summaryText.text = summary;
    }
}
