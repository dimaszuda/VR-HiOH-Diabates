using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class FoodLogger : MonoBehaviour {
    public static FoodLogger Instance;

    public List<StoredData> foodHistory = new List<StoredData>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void LogFood(string name, float virtualHour, Sprite icon, float carbohydrate, float gi, float gl, float glucose_change) {
        foodHistory.Add(new StoredData {
            foodName = name,
            virtualHour = virtualHour,
            icon = icon,
            carbohydrate = carbohydrate,
            glycemic_index = gi,
            glycemic_load = gl,
            glucose_change = glucose_change
        });
    }
}
