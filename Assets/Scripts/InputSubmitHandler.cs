using UnityEngine;
using TMPro;

public class InputSubmitHandler : MonoBehaviour
{
    public GetNutrition SearchFood;
    public TMP_InputField inputField;

    void Start()
    {
        if (inputField != null)
        {
            inputField.onSubmit.AddListener(OnSubmit);
        }
    }

    void OnSubmit(string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            if (SearchFood != null)
            {
                SearchFood.OnSearchButtonClicked();
            }
        }
    }

    void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onSubmit.RemoveListener(OnSubmit);
        }
    }
}
