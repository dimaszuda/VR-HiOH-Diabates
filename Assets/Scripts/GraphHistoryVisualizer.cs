using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GraphHistoryVisualizer : MonoBehaviour {
    public float xAxisWidth;
    public float startHour = 6f;
    public GameObject exitButton;
    public GameObject iconPrefab;
    public StageLoader stageLoader;
    public TextMeshProUGUI petunjuk;
    public RectTransform foodIconContainer;
    public RectTransform activityIconContainer;

    public Image loadingIcon;
    public GameObject button;
    public TextMeshProUGUI loadingText;

    private string kelas = "";
    private string kelompok = "";
    private string full_name = "";
    private string number = "";
    private string summary = "";
    private Coroutine exitButtonCoroutine;

    // Timer variables untuk mengukur waktu pengiriman
    private System.DateTime sendStartTime;
    private bool isSending = false;

    // Anti-duplicate variables
    private bool hasDataBeenSent = false; // Flag untuk mencegah pengiriman berulang
    private bool isButtonProcessing = false; // Flag untuk mencegah multiple button clicks

    // Progress tracking variables
    private int totalDataCount = 0;
    private float averageTimePerData = 2.5f; 
    private bool autoRedirectAfterSend = true;

    private List<GoogleFormSender.ActPattern> dataToSend = new List<GoogleFormSender.ActPattern>();
    
    void Update() {
        // animasi loading icon diputar jika aktif
        if (loadingIcon != null && loadingIcon.gameObject.activeSelf) {
            loadingIcon.transform.Rotate(Vector3.forward * -200f * Time.deltaTime);
        }

        // Update loading text dengan progress percentage jika sedang mengirim
        if (isSending && loadingText != null && loadingText.gameObject.activeSelf) {
            var elapsedTime = System.DateTime.Now - sendStartTime;
            
            // Calculate estimated progress based on time elapsed
            float estimatedProgress = Mathf.Min((float)(elapsedTime.TotalSeconds / (totalDataCount * averageTimePerData)), 1f);
            int progressPercentage = Mathf.RoundToInt(estimatedProgress * 100f);
            
            // Show progress with estimated completion
            float estimatedTotalTime = totalDataCount * averageTimePerData;
            float remainingTime = Mathf.Max(estimatedTotalTime - (float)elapsedTime.TotalSeconds, 0);
            
            loadingText.text = $"Mengirim data... {progressPercentage}% ({elapsedTime.TotalSeconds:F1}s/{estimatedTotalTime:F1}s)";
        }
    }

    void Start() {
        Debug.Log("⏳ Mulai menampilkan histori...");

        if (ActivityLogger.Instance == null || FoodLogger.Instance == null) {
            Debug.LogWarning("Data histori tidak tersedia.");
            return;
        }

        // Reset flags saat mulai
        hasDataBeenSent = false;
        isButtonProcessing = false;

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

        summary = SummaryAct.Instance.summary;
        
        float endHour = PlayerPrefs.GetFloat("End Hour", 24f);
        float totalHourRange = Mathf.Max(endHour - startHour, 1f);

        List<StoredData> activityHistory = ActivityLogger.Instance.activityHistory;
        Debug.Log($"📊 Total Activity History Count: {activityHistory.Count}");
        
        List<StoredData> foodHistory = FoodLogger.Instance.foodHistory;
        Debug.Log($"🍽️ Total Food History Count: {foodHistory.Count}");

        // Tampilkan aktivitas
        foreach (var activity in activityHistory) {
            float relativeTime = (activity.virtualHour - startHour) / totalHourRange;
            float posX = relativeTime * activityIconContainer.rect.width;

            GameObject icon = Instantiate(iconPrefab, activityIconContainer);
            icon.GetComponent<Image>().sprite = activity.icon;

            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(posX, -10f);

            var data = new GoogleFormSender.ActPattern {
                targetSheet = "pola_aktivitas",
                class_name = kelas,
                team = kelompok,
                full_name = full_name,
                number = number,
                virtual_hour = activity.virtualHour.ToString("F2"),
                activity_type = "aktivitas",
                activity_name = activity.actName,
                carbohydrate = 0,
                glycemic_index = 0,
                glycemic_load = 0,
                glucose_change = activity.glucose_change
            };

            dataToSend.Add(data);
        }

        // Tampilkan makanan
        foreach (var food in foodHistory) {
            float relativeTime = (food.virtualHour - startHour) / totalHourRange;
            float posX = relativeTime * foodIconContainer.rect.width;

            GameObject icon = Instantiate(iconPrefab, foodIconContainer);
            icon.GetComponent<Image>().sprite = food.icon;

            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(posX, -10f);

            var data = new GoogleFormSender.ActPattern {
                targetSheet = "pola_aktivitas",
                class_name = kelas,
                team = kelompok,
                full_name = full_name,
                number = number,
                virtual_hour = food.virtualHour.ToString("F2"),
                activity_type = "makan",
                activity_name = food.foodName,
                carbohydrate = food.carbohydrate,
                glycemic_index = food.glycemic_index,
                glycemic_load = food.glycemic_load,
                glucose_change = food.glucose_change
            };

            dataToSend.Add(data);
        }

        Debug.Log($"📊 Total data yang akan dikirim: {dataToSend.Count}");
        
        // Set total data count untuk progress tracking
        totalDataCount = dataToSend.Count;
    }

    /// <summary>
    /// Fungsi ini dipanggil saat tombol Save diklik - DENGAN ANTI-DUPLICATE
    /// </summary>
    public void OnSaveButton() {
        // STEP 1: Cek apakah sedang processing atau sudah pernah kirim
        if (isButtonProcessing) {
            Debug.LogWarning("⚠️ Button sedang diproses, abaikan klik ganda!");
            return;
        }

        if (hasDataBeenSent) {
            Debug.LogWarning("⚠️ Data sudah pernah dikirim sebelumnya!");
            return;
        }

        if (dataToSend.Count == 0) {
            Debug.LogWarning("⚠️ Tidak ada data yang bisa dikirim.");
            return;
        }

        // STEP 2: Set flags untuk mencegah duplicate calls
        isButtonProcessing = true;
        hasDataBeenSent = true;

        // STEP 3: Disable button immediately
        if (button != null) {
            button.SetActive(false);
        }

        // ⏱START TIMER - Catat waktu mulai mengirim
        sendStartTime = System.DateTime.Now;
        isSending = true;
        Debug.Log($"[{sendStartTime:HH:mm:ss.fff}] Mulai mengirim {dataToSend.Count} data aktivitas dan makanan...");
        
        ShowLoadingUI(true);
        
        if (exitButtonCoroutine != null)
            StopCoroutine(exitButtonCoroutine);
        exitButtonCoroutine = StartCoroutine(ShowExitButtonWithDelay(3f));

        // STEP 4: Validasi GoogleFormSender instance
        if (GoogleFormSender.Instance == null) {
            Debug.LogError("❌ GoogleFormSender.Instance is null!");
            ResetSendingState();
            return;
        }

        // STEP 5: Kirim data dengan callback yang aman
        GoogleFormSender.Instance.SendActPatternsSequentially(dataToSend, OnDataSendComplete);
        
        // Kirim summary
        var summaryPattern = new GoogleFormSender.PatternResult {
            targetSheet = "hasil_pola",
            class_name = kelas,
            team = kelompok,
            full_name = full_name,
            number = number,
            summary = summary
        };
        GoogleFormSender.Instance.SendPolaSummary(summaryPattern);
    }

    /// <summary>
    /// Callback yang dipanggil setelah pengiriman selesai
    /// </summary>
    private void OnDataSendComplete() {
        // END TIMER - Hitung total waktu pengiriman
        var sendEndTime = System.DateTime.Now;
        var totalDuration = sendEndTime - sendStartTime;
        isSending = false;
        
        Debug.Log($"[{sendEndTime:HH:mm:ss.fff}] Semua data aktivitas & makanan berhasil dikirim.");
        Debug.Log($"TOTAL WAKTU PENGIRIMAN: {totalDuration.TotalSeconds:F2} detik ({totalDuration.TotalMilliseconds:F0} ms)");
        Debug.Log($"STATISTIK: {dataToSend.Count} data dikirim dalam {totalDuration.TotalSeconds:F2}s = {(dataToSend.Count / totalDuration.TotalSeconds):F2} data/detik");
        
        // Update loading text untuk menampilkan hasil
        if (loadingText != null) {
            loadingText.text = $"Selesai! 100% ({totalDuration.TotalSeconds:F1}s)";
        }

        // Reset processing state (tapi tetap keep hasDataBeenSent = true)
        isButtonProcessing = false;
        
        // Auto redirect ke homepage setelah delay singkat
        if (autoRedirectAfterSend) {
            StartCoroutine(AutoRedirectToHomepage());
        }
    }

    /// <summary>
    /// Auto redirect ke homepage dengan delay
    /// </summary>
    private IEnumerator AutoRedirectToHomepage() {
        Debug.Log("Auto redirect ke homepage dalam 2 detik...");
        
        // Update loading text untuk countdown
        if (loadingText != null) {
            for (int i = 2; i > 0; i--) {
                loadingText.text = $"Kembali ke menu dalam {i} detik...";
                yield return new WaitForSeconds(1f);
            }
            loadingText.text = "Redirecting...";
        } else {
            yield return new WaitForSeconds(2f);
        }
        
        Debug.Log("Redirecting to homepage...");
        stageLoader.LoadHomepageScene();
    }

    /// <summary>
    /// Reset state jika terjadi error
    /// </summary>
    private void ResetSendingState() {
        isSending = false;
        isButtonProcessing = false;
        // NOTE: hasDataBeenSent tetap true untuk mencegah pengiriman berulang
        
        ShowLoadingUI(false);
        if (button != null) {
            button.SetActive(true); // tampilkan lagi button jika error
        }
    }

    private void ShowLoadingUI(bool show) {
        if (loadingIcon != null)
            loadingIcon.gameObject.SetActive(show);
        if (loadingText != null) {
            loadingText.gameObject.SetActive(show);
            if (!show) {
                loadingText.text = "";
                isSending = false;
            }
        }

        // petunjuk tetap langsung tampil
        petunjuk.gameObject.SetActive(show);
    }

    public void onExitButton() {
        stageLoader.LoadHomepageScene();
    }

    private IEnumerator ShowExitButtonWithDelay(float delay) {
        exitButton.SetActive(false);
        yield return new WaitForSeconds(delay);
        exitButton.SetActive(true);
    }

    /// <summary>
    /// Debug method untuk reset state (bisa dipanggil dari inspector saat testing)
    /// </summary>
    [ContextMenu("Reset Send State")]
    public void DebugResetSendState() {
        hasDataBeenSent = false;
        isButtonProcessing = false;
        isSending = false;
        if (button != null) {
            button.SetActive(true);
        }
        Debug.Log("🔧 Send state direset untuk testing");
    }
}