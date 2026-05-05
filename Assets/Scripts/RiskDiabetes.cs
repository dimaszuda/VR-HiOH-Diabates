using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RiskDiabetes : MonoBehaviour {
    public RawImage riskImage;
    public Texture lowRiskTexture;
    public Texture mediumRiskTexture;
    public Texture highRiskTexture;

    void Start()
    {
        if (PlayerPrefs.HasKey("Risk of Diabetes"))
        {
            string riskDiabetes = PlayerPrefs.GetString("Risk of Diabetes");
            switch (riskDiabetes.ToLower())
            {
                case "low":
                    riskImage.texture = lowRiskTexture;
                    break;
                case "medium":
                    riskImage.texture = mediumRiskTexture;
                    break;
                case "high":
                    riskImage.texture = highRiskTexture;
                    break;
                default:
                    Debug.LogWarning("Nilai risiko tidak dikenali: " + riskDiabetes);
                    break;
            }
        }
        else
        {
            Debug.Log("Belum ada data risiko diabetes tersimpan.");
        }
    }
}
