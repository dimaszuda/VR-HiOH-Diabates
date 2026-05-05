using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class ActivityLogger : MonoBehaviour {
    public static ActivityLogger Instance;

    public List<StoredData> activityHistory = new List<StoredData>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void LogActivity(string name, float virtualHour, Sprite icon, float glucose_change) {
        activityHistory.Add(new StoredData {
            actName = name,
            virtualHour = virtualHour,
            icon = icon,
            glucose_change = glucose_change
        });
    }
}
