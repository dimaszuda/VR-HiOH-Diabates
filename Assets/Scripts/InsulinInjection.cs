using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class InsulinInjection : MonoBehaviour {
    public GlucoseLevel glucoseManager;
    private float glucoseDrop = -50f;
    private Button btn;
    private RawImage img;
    public Sprite insulinIcon; // Gambar insulin yang akan ditampilkan
    public float cooldownDuration = 10f; // waktu delay dalam detik

    private void Start() {
        btn = GetComponent<Button>();
        img = GetComponent<RawImage>();

        if (btn != null) {
            btn.onClick.AddListener(OnButtonClicked);
        }
    }

    void OnButtonClicked() {
        injectInsulin();
        btn.interactable = false;
        img.enabled = false; // Menyembunyikan gambar saat tombol tidak aktif
        StartCoroutine(ReenableButtonAfterDelay());
    }

    private IEnumerator ReenableButtonAfterDelay() {
        yield return new WaitForSeconds(cooldownDuration);
        btn.interactable = true;
        img.enabled = true; // Menampilkan gambar kembali saat tombol aktif
    }

    
    public void injectInsulin() {
        glucoseManager.SetActivity(glucoseDrop);
        glucoseManager.SetDoingIcon("injeksi");
        float currentHour = VirtualClock.Instance.GetVirtualHour();
        ActivityLogger.Instance.LogActivity("injeksi", currentHour, insulinIcon, glucoseDrop);
    }
}
