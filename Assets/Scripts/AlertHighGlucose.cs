using TMPro;
using UnityEngine;
using System.Collections;

public class AlertHighGlucose : MonoBehaviour {
    public GameObject alertPanel;
    public GameObject arrowSign;
    public GameObject buttonSign;
    public TextMeshProUGUI textSign;

    public GlucoseLevel glucoseLogic;

    // Settings untuk kedip-kedip
    [Header("Blinking Settings")]
    public float beepDuration = 0.2f;        // Durasi 1 kedip (beep)
    public float beepInterval = 0.1f;        // Jeda antar kedip dalam 1 set
    public float pauseAfterSet = 2f;         // Jeda setelah beep-beep selesai
    
    private Coroutine blinkingCoroutine;
    private bool isAlertActive = false;

    void Update() {
        float level = glucoseLogic.currentGlucoseLevel;

        if (level > 200f) {
            // Jika alert belum aktif, mulai alert
            if (!isAlertActive) {
                StartAlert();
            }
        }
        else {
            // Jika alert aktif tapi glucose sudah normal, stop alert
            if (isAlertActive) {
                StopAlert();
            }
        }
    }

    void StartAlert() {
        isAlertActive = true;
        
        // Tampilkan panel dan text (ini tetap tampil)
        alertPanel.SetActive(true);
        textSign.gameObject.SetActive(true);
        
        // Mulai coroutine kedip-kedip untuk arrow dan button
        if (blinkingCoroutine != null) {
            StopCoroutine(blinkingCoroutine);
        }
        blinkingCoroutine = StartCoroutine(BlinkingPattern());
    }

    void StopAlert() {
        isAlertActive = false;
        
        // Sembunyikan semua
        alertPanel.SetActive(false);
        arrowSign.SetActive(false);
        buttonSign.SetActive(false);
        textSign.gameObject.SetActive(false);
        
        // Stop coroutine kedip-kedip
        if (blinkingCoroutine != null) {
            StopCoroutine(blinkingCoroutine);
            blinkingCoroutine = null;
        }
    }

    IEnumerator BlinkingPattern() {
        while (isAlertActive) {
            // === BEEP PERTAMA ===
            // Nyalakan
            arrowSign.SetActive(true);
            buttonSign.SetActive(true);
            yield return new WaitForSeconds(beepDuration);
            
            // Matikan
            arrowSign.SetActive(false);
            buttonSign.SetActive(false);
            yield return new WaitForSeconds(beepInterval);

            // === BEEP KEDUA ===
            // Nyalakan lagi
            arrowSign.SetActive(true);
            buttonSign.SetActive(true);
            yield return new WaitForSeconds(beepDuration);
            
            // Matikan
            arrowSign.SetActive(false);
            buttonSign.SetActive(false);
            
            // === PAUSE 2 DETIK ===
            yield return new WaitForSeconds(pauseAfterSet);
            
            // Loop akan mengulang otomatis selama isAlertActive = true
        }
    }

    // Optional: Fungsi untuk ubah pola dari luar (bisa dipanggil script lain)
    public void SetBlinkingPattern(float beepTime, float intervalTime, float pauseTime) {
        beepDuration = beepTime;
        beepInterval = intervalTime;
        pauseAfterSet = pauseTime;
    }

    public void onClickInjection() {
        alertPanel.SetActive(false);
    }
}