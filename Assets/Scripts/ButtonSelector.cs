using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonSelector : MonoBehaviour {
    [Header("Team Buttons (1-7)")]
    public Button[] teamButtons;

    [Header("Class Buttons (A1-A7)")]
    public Button[] classButtons;
    private Color defaultColor = Color.white;
    private Color selectedColor = Color.blue;
    private int selectedTeam = -1;
    private string selectedClass = "";
    public bool isTeamSelected = false;
    public bool isClassSelected = false;

    public AudioSource audioSource;
    public AudioClip clickSound;

    public static ButtonSelector Instance;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }


    void Start() {
        foreach (Button btn in teamButtons) {
            btn.onClick.AddListener(() => OnTeamButtonClick(btn));
            SetButtonColor(btn, defaultColor);
        }

        foreach (Button btn in classButtons) {
            btn.onClick.AddListener(() => OnClassButtonClick(btn));
            SetButtonColor(btn, defaultColor);
        }
    }

    // fungsi untuk handle kelompok
    void OnTeamButtonClick(Button clickedButton) {
        isTeamSelected = true;
        foreach (Button btn in teamButtons)
        {
            SetButtonColor(btn, defaultColor);
        }

        SetButtonColor(clickedButton, selectedColor);

         PlayClickSound();

        TMP_Text text = clickedButton.GetComponentInChildren<TMP_Text>();
        if (text != null && int.TryParse(text.text, out int number)) {
            selectedTeam = number;
            PlayerPrefs.SetInt("SelectedNumber", selectedTeam);
            PlayerPrefs.Save();
        }
    }

    // fungsi untuk handle kelas
    void OnClassButtonClick(Button clickedButton) {
        isClassSelected = true;
        foreach (Button btn in classButtons)
        {
            SetButtonColor(btn, defaultColor);
        }

        SetButtonColor(clickedButton, selectedColor);
        PlayClickSound();

        TMP_Text text = clickedButton.GetComponentInChildren<TMP_Text>();
        if (text != null) {
            selectedClass = text.text;
            PlayerPrefs.SetString("SelectedLetter", selectedClass);
            PlayerPrefs.Save();
            Debug.Log("Selected letter: " + selectedClass);
        }
    }

    void SetButtonColor(Button btn, Color color)
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = color;
        cb.selectedColor = color;
        cb.highlightedColor = color;
        cb.pressedColor = color;
        btn.colors = cb;
    }

    public void TryHandleClick(Button clickedButton)
    {
        // Cek apakah ini adalah tombol team
        foreach (Button btn in teamButtons)
        {
            if (btn == clickedButton)
            {
                OnTeamButtonClick(btn);
                return;
            }
        }

        // Cek apakah ini tombol class
        foreach (Button btn in classButtons)
        {
            if (btn == clickedButton)
            {
                OnClassButtonClick(btn);
                return;
            }
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

}
