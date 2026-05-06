using UnityEngine;
using TMPro;

public class FoodInfoDisplay : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject infoPanel;
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
            FoodData foodData = hitObj.GetComponent<FoodData>();

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

    void ShowPanel(FoodData food)
    {
        infoPanel.SetActive(true);
        infoPanel.transform.position = food.transform.position + new Vector3(0, 0.2f, 0);

        foodText.text = food.foodName;

        if (food.category == "low") {
            foodText.color = Color.green;
        }
        else if (food.category == "medium") {
            foodText.color = Color.yellow;
        }
        else if (food.category == "high") {
            foodText.color = Color.red;
        }
    }

    void HidePanel()
    {
        infoPanel.SetActive(false);
        lastFood = null;
    }
}
