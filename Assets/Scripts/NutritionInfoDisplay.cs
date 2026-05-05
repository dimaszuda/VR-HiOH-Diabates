using UnityEngine;
using TMPro;

public class NutritionInfoDisplay : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject infoPanel;
    public TextMeshProUGUI karboText;
    public TextMeshProUGUI GIText;
    public TextMeshProUGUI GLText;
    public TextMeshProUGUI foodText;

    public Camera mainCamera;

    private GameObject lastFood;

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObj = hit.collider.gameObject;
            MetadataFood foodData = hitObj.GetComponent<MetadataFood>();

            if (foodData != null)
            {
                if (hitObj != lastFood)
                {
                    ShowPanel(foodData);
                    lastFood = hitObj;
                }
            }
            else
            {
                HidePanel();
            }
        }
        else
        {
            HidePanel();
        }
    }

    void ShowPanel(MetadataFood food)
    {
        infoPanel.SetActive(true);
        // infoPanel.transform.position = food.transform.position + new Vector3(0, 0f, 0.25f);
        infoPanel.transform.position = food.transform.position + new Vector3(0.3f, 0.35f, 0f);
        // x = depan belakang
        // y = atas bawah
        // z = kanan kiri
        foodText.text = food.foodName;
        karboText.text = food.karboInfo;
        GIText.text = food.GIInfo;
        GLText.text = food.GLInfo;
    }

    void HidePanel()
    {
        infoPanel.SetActive(false);
        lastFood = null;
    }
}
