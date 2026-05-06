using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FoodClickHandler : MonoBehaviour
{
    public GlucoseLevel glucoseManager;
    private FoodData foodData;

    public AudioSource audioSource; 
    public AudioClip clickSound;

    [Header("Cooldown Settings")]
    public float cooldownTime = 3f; // lama cooldown (detik)
    
    private bool isCooldown = false;
    private Coroutine cooldownCoroutine = null; // Track active coroutine
    private float cooldownEndTime = 0f; // Track when cooldown should end

    private Button buttonComponent;
    private Image imageComponent;

    void Start()
    {
        foodData = GetComponent<FoodData>();
        
        buttonComponent = GetComponent<Button>();
        imageComponent = GetComponent<Image>();
    }

    void Update()
    {
        // Pastikan cooldown state konsisten
        if (isCooldown && Time.time >= cooldownEndTime && cooldownEndTime > 0)
        {
            Debug.LogWarning($"🔧 Cooldown fallback reset for {foodData.foodName}");
            ResetCooldownState();
        }
    }

    public void OnClickFood()
    {
        // Enhanced cooldown check with detailed logging
        if (isCooldown)
        {
            float remainingTime = cooldownEndTime - Time.time;
            Debug.Log($"⏳ {foodData.foodName} sedang cooldown, sisa {remainingTime:F1} detik");
            return;
        }

        Debug.Log($"🍽️ {foodData.foodName} diklik - memulai proses");

        // Immediately set cooldown to prevent double-clicks
        isCooldown = true;
        cooldownEndTime = Time.time + cooldownTime;

        // Stop any existing cooldown coroutine
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            Debug.Log($"🛑 Stopped existing cooldown coroutine for {foodData.foodName}");
        }

        // Process food click
        ProcessFoodClick();

        // Start new cooldown
        cooldownCoroutine = StartCoroutine(CooldownRoutine());
        
        UpdateVisualFeedback(false);
    }

    private void ProcessFoodClick()
    {
        float karbo = 0f;
        float gi = 0f;
        float gl = 0f;

        float.TryParse(foodData.karboInfo, out karbo);
        float.TryParse(foodData.GIInfo, out gi);
        float.TryParse(foodData.GLInfo, out gl);

        PlayClickSound();
        Debug.Log($"Set Glucose Level: {foodData.glucoseRise} for {foodData.foodName}");
        glucoseManager.SetGlucoseLevel(foodData.glucoseRise);

        Debug.Log($"Set Act Name: {foodData.foodName}");
        glucoseManager.SetActIcon(foodData.foodName, foodData.foodIcon);

        Debug.Log("Set Doing Text: Makan");
        glucoseManager.SetDoingIcon("makan");

        Debug.Log("Set Activity Log");
        float currentHour = VirtualClock.Instance.GetVirtualHour();  
        
        Debug.Log($"Current Hour: {currentHour:F2}");
        FoodLogger.Instance.LogFood(
            foodData.foodName,
            currentHour,
            foodData.foodIcon,
            karbo,
            gi,
            gl,
            foodData.glucoseRise
        );
        Debug.Log($"✅ Food data logged: {foodData.foodName}");
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private IEnumerator CooldownRoutine()
    {
        Debug.Log($"⏳ Cooldown dimulai untuk {foodData.foodName}: {cooldownTime} detik");
        
        // Wait for cooldown duration
        yield return new WaitForSeconds(cooldownTime);
        
        // Double-check: pastikan coroutine ini masih yang aktif
        if (cooldownCoroutine == null)
        {
            Debug.LogWarning($"⚠️ Cooldown coroutine sudah di-stop untuk {foodData.foodName}");
            yield break;
        }

        // Reset cooldown state
        ResetCooldownState();
        Debug.Log($"✅ Cooldown selesai: {foodData.foodName} bisa diklik lagi");
    }

    private void ResetCooldownState()
    {
        isCooldown = false;
        cooldownEndTime = 0f;
        cooldownCoroutine = null;
        
        // Update visual feedback
        UpdateVisualFeedback(true);
    }

    private void UpdateVisualFeedback(bool isClickable)
    {
        if (buttonComponent != null)
        {
            buttonComponent.interactable = isClickable;
        }
        
        if (imageComponent != null)
        {
            Color color = imageComponent.color;
            color.a = isClickable ? 1f : 0.6f; // Semi-transparent when cooling down
            imageComponent.color = color;
        }
    }

    [ContextMenu("Force Reset Cooldown")]
    public void DebugForceResetCooldown()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        ResetCooldownState();
        Debug.Log($"🔧 Force reset cooldown untuk {foodData.foodName}");
    }

    [ContextMenu("Debug Cooldown Status")]
    public void DebugCooldownStatus()
    {
        float remainingTime = cooldownEndTime - Time.time;
        Debug.Log($"🔍 {foodData.foodName} Status:" +
                 $"\n- isCooldown: {isCooldown}" +
                 $"\n- cooldownCoroutine: {(cooldownCoroutine != null ? "Active" : "Null")}" +
                 $"\n- remainingTime: {remainingTime:F2}s" +
                 $"\n- Time.time: {Time.time:F2}");
    }

    void OnDestroy()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
    }

    void OnDisable()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            ResetCooldownState();
        }
    }
}