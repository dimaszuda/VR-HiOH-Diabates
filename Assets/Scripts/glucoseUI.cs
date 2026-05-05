using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class glucoseUI : MonoBehaviour {
    public GlucoseLevel glucoseLogic; // drag reference ke script GlucoseLevel
    public TextMeshProUGUI currentConditionText;

    void Update()
    {
        float level = glucoseLogic.currentGlucoseLevel;
        string status = "";
        Color color = Color.white;

        if (level < 70) {
            color = Color.blue;
            status = "Low Glucose";
        }
        else if (level >= 70 && level < 140) {
            color = Color.green;
            status = "Normal Glucose";
        }
        else if (level >= 140 && level < 200) {
            color = Color.yellow;
            status = "Glucose starts to get high";
        }
        else {
            color = Color.red;
            status = "High Glucose";
        }
        

        currentConditionText.text = $"| Status: {status}\n| Glucose: {level:0}";
        currentConditionText.color = color;
    }
}
