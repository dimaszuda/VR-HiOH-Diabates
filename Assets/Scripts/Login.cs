using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;

public class Login : MonoBehaviour {
    [Header("Canvas References")]
    public GameObject homepageCanvas;
    public GameObject loginCanvas;
    public GameObject validationCanvas;
    public GameObject welcomeCanvas;
    
    [Header("Game Specific Panels")]
    public GameObject PolaAct;
    public GameObject welcomePola;
    public GameObject welcomeKantin;
    public GameObject KantinSehat;

    [Header("Input Fields")]
    public TMP_InputField pinPola;
    public TMP_InputField pinKantin;

    [Header("UI Text")]
    public TextMeshProUGUI warning;
    public TextMeshProUGUI polaText;
    public TextMeshProUGUI kantinText;

    [Header("Dependencies")]
    public StageLoader stageLoader;

    private string welcomeText = "";

    private void Start() {
        // Pastikan state awal bersih
        ResetAllPanels();
        homepageCanvas.SetActive(true);
    }

    public void loginPolaAct() {
        if (!IsPlayerDataComplete()) {
            ShowValidation();
        }
        else {
            welcomeText = "Selamat datang kembali, " + PlayerPrefs.GetString("full_name") + 
                         "! Di scene ini, kamu akan ditantang untuk mengatur pola makan dan aktivitas kamu untuk menjaga kestabilan gula darah. Selamat bermain!";
            polaText.text = welcomeText;
            
            ShowWelcome();
            welcomePola.SetActive(true);
        }
    }

    public void nextPola() {
        HideWelcome();
        ShowLogin();
        PolaAct.SetActive(true);
    }

    private const string hashPola = "64b2fa4d1a559557f9934b9a375abb04b2899694a9feb1b9e9e36d8766f8a3f2";

    public void validatePola() {
        Debug.Log("LOGIN POLA ACT");
        HideWarning(); // Reset warning sebelum validasi
        
        if (HashPin(pinPola.text) == hashPola) {
            stageLoader.LoadMainGame();
        }
        else {
            ShowWarning("PIN salah! Coba lagi!");
        }
    }

    public void loginKantin() {
        if (!IsPlayerDataComplete()) {
            ShowValidation();
        }
        else {
            welcomeText = "Selamat datang kembali, " + PlayerPrefs.GetString("full_name") + 
                         "! Di scene ini, kamu akan ditantang untuk meracik menu makan sehat berdasarkan risiko diabetes yang kamu punya. Selamat bermain!";
            kantinText.text = welcomeText;
            
            ShowWelcome();
            welcomeKantin.SetActive(true);
        }
    }

    public void nextKantin() {
        HideWelcome();
        ShowLogin();
        KantinSehat.SetActive(true);
    }

    private const string hashKantin = "46554c36bfddf0d05b9f5bb9f2fb02c0838c294ced9ad6caa45aab73abe20d1d";

    public void validateKantin() {
        Debug.Log("LOGIN KANTIN SEHAT");
        HideWarning(); // Reset warning sebelum validasi
        
        if (HashPin(pinKantin.text) == hashKantin) {
            stageLoader.LoadKantinSehat();
        }
        else {
            ShowWarning("PIN salah! Coba lagi!");
        }
    }

    private static string HashPin(string pin) {
        using (SHA256 sha256 = SHA256.Create()) {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pin));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    public void onBackButton() {
        ResetAllPanels();
        homepageCanvas.SetActive(true);
    }

    public void onCancelButton() {
        ResetAllPanels();
        homepageCanvas.SetActive(true);
    }

    // Helper Methods untuk State Management
    private bool IsPlayerDataComplete() {
        return PlayerPrefs.HasKey("SelectedNumber") && 
               PlayerPrefs.HasKey("SelectedLetter") && 
               PlayerPrefs.HasKey("Risk of Diabetes") && 
               PlayerPrefs.HasKey("full_name");
    }

    private void ResetAllPanels() {
        // Disable semua canvas
        homepageCanvas.SetActive(false);
        loginCanvas.SetActive(false);
        validationCanvas.SetActive(false);
        welcomeCanvas.SetActive(false);
        
        // Disable semua sub-panels
        PolaAct.SetActive(false);
        welcomePola.SetActive(false);
        welcomeKantin.SetActive(false);
        KantinSehat.SetActive(false);
        
        // Reset UI elements
        HideWarning();
        ClearInputFields();
    }

    private void ShowValidation() {
        ResetAllPanels();
        validationCanvas.SetActive(true);
    }

    private void ShowWelcome() {
        ResetAllPanels();
        welcomeCanvas.SetActive(true);
    }

    private void HideWelcome() {
        welcomeCanvas.SetActive(false);
        welcomePola.SetActive(false);
        welcomeKantin.SetActive(false);
    }

    private void ShowLogin() {
        loginCanvas.SetActive(true);
    }

    private void ShowWarning(string message) {
        warning.gameObject.SetActive(true);
        warning.text = message;
    }

    private void HideWarning() {
        if (warning != null && warning.gameObject != null) {
            warning.gameObject.SetActive(false);
            warning.text = "";
        }
    }

    private void ClearInputFields() {
        if (pinPola != null) pinPola.text = "";
        if (pinKantin != null) pinKantin.text = "";
    }
}