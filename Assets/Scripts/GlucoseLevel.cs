using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GlucoseLevel : MonoBehaviour {
    public TextMeshProUGUI glucoseLevelText;
    public TextMeshProUGUI statusText;
    public Image actIcon;
    public TextMeshProUGUI actName;
    public Image glucoseBar;
    public TextMeshProUGUI doingText;
    public RawImage doingIcon;
    public Sprite[] doingImage;

    public float currentGlucoseLevel = 120f;
    private string doingNem = "";
    private Sprite doingImg;
    private float saveTimer = 0f;
    private float saveInterval = 0.2f;
    private float levelMultiplier = 1f;
    
    private bool isRunning = false;

    public static GlucoseLevel Instance;

    private List<GlucoseEvent> activeEvents = new List<GlucoseEvent>();

    // Representasi dari satu kenaikan gula
    private class GlucoseEvent {
        public float amount;     // Bisa positif (makanan) atau negatif (aktivitas)
        public float duration;
        public float timeLeft;
        public string activity;

        public GlucoseEvent(float amount, float duration, string activity)
        {
            this.amount = amount;
            this.duration = duration;
            this.timeLeft = duration;
            this.activity = activity;
        }
    }

    void Start() {
        setLevel();
        Debug.Log("LEVEL MULTIPLIER: " + levelMultiplier);
        doingIcon.texture = doingImage[3].texture;
        actIcon.sprite = doingImage[4];
        
        doingText.text = "Menunggu";   
        actName.text = "Menunggu";
    }

    void Update() {
        if (!isRunning) return;

        // 1. Proses semua makanan dulu
        ProcessGlucoseEvents();

        // 2. Turunkan gula darah setelah naik
        if (currentGlucoseLevel <= 0 || currentGlucoseLevel > 350) {
            float endHour = VirtualClock.Instance.GetVirtualHour();
            PlayerPrefs.SetFloat("End Hour", endHour);
            glucoseLevelText.text = "0.0";
            SceneManager.LoadScene("Show Result");
            return;
        }
        else if (currentGlucoseLevel > 300 && currentGlucoseLevel <= 350) {
            currentGlucoseLevel -= Time.deltaTime / 2;
        }
        else if (currentGlucoseLevel > 50) {
            currentGlucoseLevel -= Time.deltaTime * levelMultiplier;
        }
        else if (currentGlucoseLevel > 0 && currentGlucoseLevel <= 50) {
            currentGlucoseLevel -= Time.deltaTime * 5;
        }


        saveTimer += Time.deltaTime;

        // save data
        if (saveTimer >= saveInterval) {
            GlucoseDataStore.collectedData.Add(currentGlucoseLevel);
            saveTimer = 0f;
        }

        // 3. Update UI
        glucoseLevelText.text = string.Format("{0:0}", currentGlucoseLevel);
        glucoseView();
        actView();
    }

    void ProcessGlucoseEvents() {
        float totalChangeThisFrame = 0f;

        for (int i = activeEvents.Count - 1; i >= 0; i--) {
            var e = activeEvents[i];
            float deltaPerSecond = 0f;

            switch (e.activity) {
                case "makan":
                    if (Mathf.Abs(e.amount) <= 10f) {
                        // Kenaikan kecil → fixed rate
                        deltaPerSecond = Mathf.Sign(e.amount) * 1f;
                    } else {
                        // Kenaikan besar → rata-rata per durasi
                        deltaPerSecond = e.amount / e.duration;
                    }
                    break;

                case "beraktivitas":
                    if (Mathf.Abs(e.amount) <= 3.5f) {
                        // Penurunan kecil → fixed rate
                        deltaPerSecond = Mathf.Sign(e.amount) * 1f;
                    } else {
                        // Penurunan besar → rata-rata per durasi
                        deltaPerSecond = e.amount / e.duration;
                    }
                    break;

                default:
                    break;
            }

            // Hitung perubahan glukosa di frame ini
            float deltaThisFrame = deltaPerSecond * Time.deltaTime;
            totalChangeThisFrame += deltaThisFrame;

            // Kurangi waktu event
            e.timeLeft -= Time.deltaTime;
            if (e.timeLeft <= 0f) {
                activeEvents.RemoveAt(i);
            }
        }

        // Update level glukosa
        currentGlucoseLevel += totalChangeThisFrame;
    }



    // Dipanggil ketika user pilih makanan
    public void SetGlucoseLevel(float glucoseLevel) {
        glucoseLevel += 10f;
        float durasi = (glucoseLevel >= 10) ? 10f / levelMultiplier : glucoseLevel / levelMultiplier;
        activeEvents.Add(new GlucoseEvent(glucoseLevel, durasi, "makan"));
    }

    public void SetActivity(float glucoseDrop) {
        float adjustedDrop = 0f;
        float durasi = (glucoseDrop >= 10 || glucoseDrop <= -10f) ? 10f / levelMultiplier : Mathf.Abs(glucoseDrop) / levelMultiplier;
        if (glucoseDrop > 10f) {
            adjustedDrop = glucoseDrop / 1.94f;
        }
        else if ((glucoseDrop > 0f && glucoseDrop < 10f) || (glucoseDrop < 0f && glucoseDrop > -10f)) {
            adjustedDrop = glucoseDrop;
        }
        else if (glucoseDrop < -10f) {
            adjustedDrop = glucoseDrop / 1.94f + 8f;
        }
        activeEvents.Add(new GlucoseEvent(adjustedDrop, durasi, "beraktivitas"));
    }

     public void SetActIcon(string sActName, Sprite actImage) {
        doingNem = sActName;
        doingImg = actImage;
    }

    public void SetDoingIcon(string doingName) {
        if (doingName == "makan") {
            doingIcon.texture = doingImage[0].texture;
            doingText.text = doingName;
        }
        else if (doingName == "aktivitas") {
            doingIcon.texture = doingImage[1].texture;
            doingText.text = doingName;
        }
        else if (doingName == "injeksi") {
            doingIcon.texture = doingImage[2].texture;
            doingText.text = doingName;
        }
    }

    void glucoseView() {
        float fill = currentGlucoseLevel / 350f;
        glucoseBar.fillAmount = Mathf.Clamp01(fill);

        if (currentGlucoseLevel < 70f) {
            glucoseLevelText.color = Color.blue;
            statusText.color = Color.blue;
            statusText.text = "Hati-hati!";
            glucoseBar.color = Color.blue;
        }
        else if (currentGlucoseLevel >= 70f && currentGlucoseLevel < 140f) {
            glucoseLevelText.color = Color.green;
            statusText.color = Color.green;
            glucoseBar.color = Color.green;
            statusText.text = "KERJA BAGUS";
        }
        else if (currentGlucoseLevel >= 140f && currentGlucoseLevel < 200f) {
            glucoseLevelText.color = Color.yellow;
            statusText.color = Color.yellow;
            glucoseBar.color = Color.yellow;
            statusText.text = "Awas!";
        }
        else {
            glucoseLevelText.color = Color.red;
            statusText.color = Color.red;
            glucoseBar.color = Color.red;
            statusText.text = "Bahaya!";
        }
    }

    void actView() {
        actIcon.sprite = doingImg;
        actName.text = doingNem;
    }

    void setLevel() {
        if (PlayerPrefs.HasKey("Risk of Diabetes")) {
            string riskDiabetes = PlayerPrefs.GetString("Risk of Diabetes");
            Debug.Log("Risiko Diabetes: " + riskDiabetes);
            switch (riskDiabetes.ToLower()) {
                case "low": levelMultiplier = 1f; break;
                case "medium": levelMultiplier = 1.5f; break;
                case "high": levelMultiplier = 2f; break;
            }
        } else {
            Debug.Log("Belum ada data risiko diabetes tersimpan.");
        }
    }

    public void StartGlucose() {
        isRunning = true;
        currentGlucoseLevel = 120f;
    }
}