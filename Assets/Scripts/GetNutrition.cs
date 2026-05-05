using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GetNutrition : MonoBehaviour
{
    public static GetNutrition Instance;

    [Header("API Configuration")]
    [SerializeField] private string[] apiKeys = {
        "YOUR_API_KEY_1_HERE",
        "YOUR_API_KEY_2_HERE", 
        "YOUR_API_KEY_3_HERE",
        "YOUR_API_KEY_4_HERE",
        "YOUR_API_KEY_5_HERE"
    };

    [Header("UI References")]
    public TMP_InputField searchInputField;
    public Button searchButton;
    public GameObject resultPanel;
    public TextMeshProUGUI foodNameText;
    public TextMeshProUGUI carbText;
    public TextMeshProUGUI glText;
    public TextMeshProUGUI giText;
    public Image loadingIndicator;

    [Header("Debug Info")]
    public TextMeshProUGUI debugText; // Optional: untuk show current API key index

    private SpoonacularService spoonacularService;
    private bool isLoading = false;
    private int currentApiKeyIndex = 0;
    private HashSet<int> blacklistedApiKeys = new HashSet<int>(); // Track failed API keys

    [HideInInspector] public NutrientInfo currentNutritionInfo;
    [HideInInspector] public string currentFoodName;

    void Awake()
    {
        spoonacularService = gameObject.AddComponent<SpoonacularService>();
        SetCurrentApiKey();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // searchButton.onClick.AddListener(OnSearchButtonClicked);
        searchInputField.onValueChanged.AddListener(OnInputChanged);        
        InitUI();
        UpdateDebugText();
    }

    void Update()
    {
        if (isLoading && loadingIndicator != null)
        {
            loadingIndicator.transform.Rotate(0f, 0f, -200f * Time.deltaTime);
        }
    }

    private void SetCurrentApiKey()
    {
        if (apiKeys.Length > 0 && currentApiKeyIndex < apiKeys.Length)
        {
            spoonacularService.SetApiKey(apiKeys[currentApiKeyIndex]);
            Debug.Log($"[GetNutrition] Using API Key #{currentApiKeyIndex + 1}");
        }
    }

    private bool TryNextApiKey()
    {
        // Mark current key as blacklisted
        blacklistedApiKeys.Add(currentApiKeyIndex);
        
        // Find next available API key
        int startIndex = currentApiKeyIndex;
        do
        {
            currentApiKeyIndex = (currentApiKeyIndex + 1) % apiKeys.Length;
            
            if (!blacklistedApiKeys.Contains(currentApiKeyIndex))
            {
                SetCurrentApiKey();
                UpdateDebugText();
                Debug.Log($"[GetNutrition] Switched to API Key #{currentApiKeyIndex + 1}");
                return true;
            }
        } 
        while (currentApiKeyIndex != startIndex);

        // All API keys are blacklisted
        Debug.LogError("[GetNutrition] All API keys have failed!");
        return false;
    }

    private void UpdateDebugText()
    {
        if (debugText != null)
        {
            debugText.text = $"API Key: {currentApiKeyIndex + 1}/{apiKeys.Length}";
            if (blacklistedApiKeys.Count > 0)
            {
                debugText.text += $" (Failed: {blacklistedApiKeys.Count})";
            }
        }
    }

    private void InitUI()
    {
        resultPanel.SetActive(false);
        if (loadingIndicator != null) loadingIndicator.gameObject.SetActive(false);
        searchButton.interactable = false;
    }

    private void SetLoadingState(bool loading)
    {
        isLoading = loading;
        if (loadingIndicator != null) loadingIndicator.gameObject.SetActive(loading);
        if (loading && TouchScreenKeyboard.isSupported)
        {
            TouchScreenKeyboard.hideInput = true;
        } 
    }

    private void OnInputChanged(string input)
    {
        searchButton.interactable = !string.IsNullOrWhiteSpace(input);
    }

    public void OnSearchButtonClicked()
    {
        resultPanel.SetActive(false);
        string query = searchInputField.text.Trim();
        if (!string.IsNullOrWhiteSpace(query))
        {
            currentFoodName = query;
            StartCoroutine(SearchNutritionDataWithRetry(query));
        }
    }

    IEnumerator SearchNutritionDataWithRetry(string foodName)
    {
        SetLoadingState(true);
        resultPanel.SetActive(false);

        bool success = false;
        int maxRetries = apiKeys.Length; // Try all API keys

        for (int retry = 0; retry < maxRetries && !success; retry++)
        {
            bool tempSuccess = false;
            // jalankan coroutine dan tunggu hasil callback
            yield return StartCoroutine(SearchNutritionData(foodName, (result) => tempSuccess = result));
            success = tempSuccess;

            if (!success)
            {
                if (TryNextApiKey())
                {
                    Debug.Log($"[GetNutrition] Retrying with next API key... ({retry + 1}/{maxRetries})");
                    yield return new WaitForSeconds(1f); // Brief delay before retry
                }
                else
                {
                    break; // No more API keys available
                }
            }
        }

        if (!success)
        {
            ShowErrorMessage("Semua API key telah mencapai batas atau bermasalah");
        }

        SetLoadingState(false);
    }

    IEnumerator SearchNutritionData(string foodName, System.Action<bool> onComplete)
    {
        int id = -1;
        bool isApiLimitReached = false;

        yield return spoonacularService.GetIdWithLimitCheck(foodName, (result, limitReached) => {
            id = result;
            isApiLimitReached = limitReached;
        });

        if (isApiLimitReached)
        {
            Debug.Log($"[GetNutrition] API limit reached for API key #{currentApiKeyIndex + 1}");
            onComplete(false); // Return false to try next API key
            yield break;
        }
        else if (id <= 0)
        {
            Debug.Log($"[GetNutrition] Food not found with API key #{currentApiKeyIndex + 1}");
            currentNutritionInfo = null;
            ShowErrorMessage("Tidak dapat menemukan makanan");
            onComplete(true); // True karena ini bukan masalah limit API
            yield break;
        }

        Dictionary<string, NutrientInfo> nutrients = null;
        bool nutrientLimitReached = false;

        yield return spoonacularService.GetNutrientsWithLimitCheck(foodName, id, (result, limitReached) => {
            nutrients = result;
            nutrientLimitReached = limitReached;
        });

        if (nutrientLimitReached)
        {
            Debug.Log($"[GetNutrition] API limit reached for nutrients with API key #{currentApiKeyIndex + 1}");
            onComplete(false); // Return false to try next API key
        }
        else if (nutrients != null && nutrients.TryGetValue(foodName, out var info))
        {
            UpdateNutritionUI(info, foodName);
            resultPanel.SetActive(true);
            onComplete(true); // Success
        }
        else
        {
            currentNutritionInfo = null;
            ShowErrorMessage("Tidak dapat menemukan kandungan gizi");
            onComplete(true); // True karena ini bukan masalah limit API
        }
    }

    private void ShowErrorMessage(string message)
    {
        currentNutritionInfo = null;
        
        foodNameText.text = message;
        foodNameText.fontSize = 40;
        resultPanel.SetActive(true);
        carbText.gameObject.SetActive(false);
        glText.gameObject.SetActive(false);
        giText.gameObject.SetActive(false);
    }

    private void UpdateNutritionUI(NutrientInfo info, string foodname)
    {
        currentNutritionInfo = info;

        carbText.gameObject.SetActive(true);
        glText.gameObject.SetActive(true);
        giText.gameObject.SetActive(true);        
        foodNameText.text = foodname;
        carbText.text = $"{info.carbohydrate:F1}mg/L";
        glText.text = $"{info.glycemic_load:F1}";
        giText.text = $"{info.glycemic_index:F1}";
    }

    // Method untuk reset blacklist (misal untuk testing atau reset harian)
    [ContextMenu("Reset API Keys Blacklist")]
    public void ResetApiKeysBlacklist()
    {
        blacklistedApiKeys.Clear();
        currentApiKeyIndex = 0;
        SetCurrentApiKey();
        UpdateDebugText();
        Debug.Log("[GetNutrition] API keys blacklist reset!");
    }

    // Method untuk manual switch API key (untuk testing)
    [ContextMenu("Switch to Next API Key")]
    public void ManualSwitchApiKey()
    {
        if (TryNextApiKey())
        {
            Debug.Log("[GetNutrition] Manually switched to next API key");
        }
    }

    void OnDestroy()
    {
        // Only remove listeners if they were added
        if (searchButton != null)
            searchButton.onClick.RemoveListener(OnSearchButtonClicked);
        if (searchInputField != null)
            searchInputField.onValueChanged.RemoveListener(OnInputChanged);
    }
}