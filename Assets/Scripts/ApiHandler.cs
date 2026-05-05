using TMPro;
using UnityEngine;

public class ApiHandler : MonoBehaviour {
    public GetNutrition Nutrient;
    public MenuMakanList menuMakan;
    public GameObject searchResult;
    public TextMeshProUGUI foodNameText;

    private string foodName;
    private string carb;
    private string gl;
    private string gi;

    public void showPanel() {
        foodName = foodNameText.text;
        Debug.Log("FOOD NAME: " + foodName);

        if (foodName.ToLower() == "tidak dapat menemukan makanan" || 
            foodName.ToLower().Contains("tidak dapat menemukan") ||
            foodName.ToLower().Contains("semua api key")) {
            Debug.Log("Tidak dapat menemukan makanan atau API error");
            searchResult.SetActive(false);
            return;
        } else {
            Debug.Log("Makanan ditemukan");
            OnSearchFood();
        }
    }

    public void OnSearchFood() {
        // ✅ TAMBAHAN: Cek apakah nutrition data valid
        if (GetNutrition.Instance.currentNutritionInfo == null) {
            Debug.LogWarning("No valid nutrition data available!");
            searchResult.SetActive(false);
            return;
        }

        int index = menuMakan.GetCurrentListCount();
        if (index >= menuMakan.foodItems.Length)
            return; // daftar penuh

        // ✅ Sekarang aman karena sudah di-cek null
        carb = GetNutrition.Instance.currentNutritionInfo.carbohydrate.ToString("F1");
        gl = GetNutrition.Instance.currentNutritionInfo.glycemic_index.ToString("F1");
        gi = GetNutrition.Instance.currentNutritionInfo.glycemic_load.ToString("F1");

        menuMakan.setFoodInfo(foodName, carb, gi, gl);
        menuMakan.AddFoodItem(index);
        searchResult.SetActive(false);
    }
}