using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class actClickHandler : MonoBehaviour
{
    public GlucoseLevel glucoseManager;
    private ActivityData actData;
    private Button btn;
    private RawImage image;
    public float cooldownDuration = 5f; // waktu delay dalam detik

    public AudioSource audioSource;
    public AudioClip clickSound;

    private bool isProcessing = false;

    void Start()
    {
        actData = GetComponent<ActivityData>();
        btn = GetComponent<Button>();
        image = GetComponent<RawImage>();

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            
            btn.onClick.AddListener(OnActivityClicked);
        }
    }

    void OnActivityClicked()
    {
        // Cegah multiple clicks dalam waktu singkat
        if (isProcessing) {
            Debug.LogWarning($"⚠️ Activity {actData.actName} sedang diproses, abaikan klik!");
            return;
        }

        Debug.Log($"🎯 Activity clicked: {actData.actName}");
        
        // Set processing flag
        isProcessing = true;

        // Jalankan logik activity
        ProcessActivityClick();

        // Disable tombol, lalu aktifkan kembali setelah cooldown
        SetButtonState(false);
        StartCoroutine(ReenableButtonAfterDelay());
    }

    private void ProcessActivityClick()
    {
        PlayClickSound();
        
        // Update glucose
        glucoseManager.SetActivity(actData.glucoseDrop);
        glucoseManager.SetActIcon(actData.actName, actData.actIcon);
        glucoseManager.SetDoingIcon("aktivitas");
        
        // Log activity dengan timestamp
        float currentHour = VirtualClock.Instance.GetVirtualHour();
        Debug.Log($"📝 Logging activity: {actData.actName} at hour {currentHour:F2} with glucose drop {actData.glucoseDrop}");
        
        ActivityLogger.Instance.LogActivity(
            actData.actName, 
            currentHour,
            actData.actIcon,
            actData.glucoseDrop
        );
    }

    /// <summary>
    /// 🎵 Play sound effect
    /// </summary>
    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    /// <summary>
    /// 🔄 Set button state (enabled/disabled with visual feedback)
    /// </summary>
    private void SetButtonState(bool enabled)
    {
        if (btn != null) {
            btn.interactable = enabled;
        }
        
        if (image != null) {
            image.enabled = enabled;
            Color color = image.color;
            color.a = enabled ? 1f : 0.5f;
            image.color = color;
        }
    }

    /// <summary>
    /// ⏰ Coroutine untuk re-enable button setelah cooldown
    /// </summary>
    IEnumerator ReenableButtonAfterDelay()
    {
        yield return new WaitForSeconds(cooldownDuration);
        
        // Reset processing flag dan enable button
        isProcessing = false;
        SetButtonState(true);
        
        Debug.Log($"✅ Activity {actData.actName} ready to use again");
    }

    [System.Obsolete("Use OnActivityClicked() instead. Remove this from Inspector OnClick events!")]
    public void OnClickFood()
    {
        Debug.LogWarning($"⚠️ DEPRECATED: OnClickFood() called for {actData.actName}. Please remove from Inspector and use code-based event only!");
    }

    /// <summary>
    /// 🔧 Debug method untuk testing
    /// </summary>
    [ContextMenu("Test Activity Click")]
    public void DebugTestClick()
    {
        if (Application.isPlaying) {
            OnActivityClicked();
        }
    }

    [ContextMenu("Reset State")]
    public void DebugResetState()
    {
        isProcessing = false;
        SetButtonState(true);
        Debug.Log($"🔧 State reset for {actData.actName}");
    }

    void OnValidate()
    {
        // Validasi di editor
        if (actData == null) {
            actData = GetComponent<ActivityData>();
        }
    }
}