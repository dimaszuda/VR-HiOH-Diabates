using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI profileNameText;
    
    [Header("Settings")]
    public string playerPrefsKey = "full_name"; // Key untuk PlayerPrefs
    
    // Variable untuk menyimpan nama profil
    private string currentProfileName = "";
    
    void Start()
    {
        // Panggil fungsi untuk cek dan load profil
        LoadProfile();
    }
    
    void LoadProfile()
    {
        // Cek apakah profil sudah ada di PlayerPrefs
        if (PlayerPrefs.HasKey(playerPrefsKey))
        {
            // Ambil nama dari PlayerPrefs
            currentProfileName = PlayerPrefs.GetString(playerPrefsKey);
            
            // Cek apakah nama tidak kosong
            if (!string.IsNullOrEmpty(currentProfileName))
            {
                // Tampilkan nama di UI
                ShowProfile();
                Debug.Log("Profil ditemukan: " + currentProfileName);
            }
            else
            {
                // Nama kosong, sembunyikan profil
                HideProfile();
                Debug.Log("Profil kosong, disembunyikan");
            }
        }
        else
        {
            // PlayerPrefs belum ada, sembunyikan profil
            HideProfile();
            Debug.Log("Profil belum diset, disembunyikan");
        }
    }
    
    void ShowProfile()
    {
        // Tampilkan text dengan format yang bagus
        profileNameText.text = "Player: " + currentProfileName;
        
        profileNameText.gameObject.SetActive(true);
    }
    
    void HideProfile()
    {
        // Sembunyikan text profil
        profileNameText.gameObject.SetActive(false);
    }
    
    // Fungsi untuk update profil (bisa dipanggil dari script lain)
    public void UpdateProfile()
    {
        LoadProfile();
    }
    
    [System.Obsolete("Hanya untuk testing")]
    public void TestSetProfile(string testName)
    {
        PlayerPrefs.SetString(playerPrefsKey, testName);
        PlayerPrefs.Save();
        LoadProfile();
    }
}