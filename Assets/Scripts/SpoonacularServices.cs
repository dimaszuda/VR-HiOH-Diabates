using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NutrientInfo
{
    public double carbohydrate;
    public double glycemic_load;
    public double glycemic_index;
}

[System.Serializable]
class GlycemicLoadResponse
{
    public List<Ingredient> ingredients;
}

[System.Serializable]
class Ingredient
{
    public int id;
}

[System.Serializable]
class NutritionResponse
{
    public Nutrition nutrition;
}

[System.Serializable]
class Nutrition
{
    public List<Nutrient> nutrients;
    public List<Property> properties;
}

[System.Serializable]
class Nutrient
{
    public string name;
    public double amount;
}

[System.Serializable]
class Property
{
    public string name;
    public double amount;
}

// Tambahkan class pembungkus untuk payload
[System.Serializable]
class IngredientPayload
{
    public List<string> ingredients;

    public IngredientPayload(List<string> ingredients)
    {
        this.ingredients = ingredients;
    }
}

public class SpoonacularService : MonoBehaviour
{
    private const string BaseUrl = "https://api.spoonacular.com/food/ingredients";
    private string _apiKey;

    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
    }

    // NEW: Methods with limit check for multiple API keys support
    public IEnumerator GetIdWithLimitCheck(string menuItem, System.Action<int, bool> callback)
    {
        var list = new List<string> { menuItem };
        yield return GetIdWithLimitCheck(list, callback);
    }

    public IEnumerator GetIdWithLimitCheck(List<string> menu, System.Action<int, bool> callback)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            Debug.LogError("API Key not set!");
            callback(-1, false);
            yield break;
        }

        string url = $"{BaseUrl}/glycemicLoad?language=en&apiKey={_apiKey}";

        var payload = new IngredientPayload(menu);
        string jsonPayload = JsonUtility.ToJson(payload);

        Debug.Log($"[SpoonacularService] Payload JSON: {jsonPayload}");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Error getting ID: {request.error}");
                Debug.Log($"Response: {request.downloadHandler.text}");
                Debug.Log($"Response Code: {request.responseCode}");

                // Check for API limit based on response content
                bool isLimitReached = CheckApiLimit(request.downloadHandler.text);
                callback(-1, isLimitReached);
            }
            else
            {
                GlycemicLoadResponse response = JsonUtility.FromJson<GlycemicLoadResponse>(request.downloadHandler.text);
                if (response != null && response.ingredients != null && response.ingredients.Count > 0)
                {
                    callback(response.ingredients[0].id, false);
                }
                else
                {
                    Debug.Log("Invalid response format or no ingredients returned.");
                    callback(-1, false);
                }
            }
        }
    }

    public IEnumerator GetNutrientsWithLimitCheck(string foodName, int id, System.Action<Dictionary<string, NutrientInfo>, bool> callback)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            Debug.LogError("API Key not set!");
            callback(null, false);
            yield break;
        }

        string url = $"{BaseUrl}/{id}/information?amount=1&unit=piece&apiKey={_apiKey}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error getting nutrients: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                Debug.LogError($"Response Code: {request.responseCode}");

                // Check for API limit based on response content
                bool isLimitReached = CheckApiLimit(request.downloadHandler.text);
                callback(null, isLimitReached);
            }
            else
            {
                NutritionResponse response = JsonUtility.FromJson<NutritionResponse>(request.downloadHandler.text);

                if (response == null || response.nutrition == null)
                {
                    Debug.LogError("Invalid nutrition response format");
                    callback(null, false);
                    yield break;
                }

                var nutrients = new Dictionary<string, NutrientInfo>();
                var nutrientInfo = new NutrientInfo();

                foreach (Nutrient nutrient in response.nutrition.nutrients)
                {
                    if (nutrient.name == "Carbohydrates")
                    {
                        nutrientInfo.carbohydrate = nutrient.amount;
                    }
                }

                foreach (Property property in response.nutrition.properties)
                {
                    if (property.name == "Glycemic Load")
                    {
                        nutrientInfo.glycemic_load = property.amount;
                    }
                    else if (property.name == "Glycemic Index")
                    {
                        nutrientInfo.glycemic_index = property.amount;
                    }
                }

                nutrients[foodName] = nutrientInfo;
                callback(nutrients, false);
            }
        }
    }

    // Helper method to check if API limit is reached based on response
    private bool CheckApiLimit(string responseText)
    {
        if (string.IsNullOrEmpty(responseText))
            return false;

        try
        {
            // Check for specific limit response structure from Spoonacular API
            if (responseText.Contains("\"status\":\"failure\"") && 
                (responseText.Contains("\"code\":402") || responseText.Contains("daily points limit")))
            {
                Debug.LogWarning($"API Limit reached: {responseText}");
                return true;
            }

            // Also check for 429 status (Too Many Requests)
            if (responseText.Contains("429") || responseText.Contains("Too Many Requests"))
            {
                Debug.LogWarning($"Rate limit reached: {responseText}");
                return true;
            }

            // Check for quota exceeded messages
            if (responseText.Contains("quota") && responseText.Contains("exceeded"))
            {
                Debug.LogWarning($"Quota exceeded: {responseText}");
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing limit check: {e.Message}");
        }

        return false;
    }

    // LEGACY: Original methods for backward compatibility
    public IEnumerator GetId(string menuItem, System.Action<int> callback)
    {
        var list = new List<string> { menuItem };
        yield return GetId(list, callback);
    }

    public IEnumerator GetId(List<string> menu, System.Action<int> callback)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            Debug.LogError("API Key not set!");
            callback(-1);
            yield break;
        }

        string url = $"{BaseUrl}/glycemicLoad?language=en&apiKey={_apiKey}";

        var payload = new IngredientPayload(menu);
        string jsonPayload = JsonUtility.ToJson(payload);

        Debug.Log($"[SpoonacularService] Payload JSON: {jsonPayload}");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Error getting ID: {request.error}");
                Debug.Log($"Response: {request.downloadHandler.text}");
                callback(-1);
            }
            else
            {
                GlycemicLoadResponse response = JsonUtility.FromJson<GlycemicLoadResponse>(request.downloadHandler.text);
                if (response != null && response.ingredients != null && response.ingredients.Count > 0)
                {
                    callback(response.ingredients[0].id);
                }
                else
                {
                    Debug.Log("Invalid response format or no ingredients returned.");
                    callback(-1);
                }
            }
        }
    }

    public IEnumerator GetNutrients(string foodName, int id, System.Action<Dictionary<string, NutrientInfo>> callback)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            Debug.LogError("API Key not set!");
            callback(null);
            yield break;
        }

        string url = $"{BaseUrl}/{id}/information?amount=1&unit=piece&apiKey={_apiKey}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error getting nutrients: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                callback(null);
            }
            else
            {
                NutritionResponse response = JsonUtility.FromJson<NutritionResponse>(request.downloadHandler.text);

                if (response == null || response.nutrition == null)
                {
                    Debug.LogError("Invalid nutrition response format");
                    callback(null);
                    yield break;
                }

                var nutrients = new Dictionary<string, NutrientInfo>();
                var nutrientInfo = new NutrientInfo();

                foreach (Nutrient nutrient in response.nutrition.nutrients)
                {
                    if (nutrient.name == "Carbohydrates")
                    {
                        nutrientInfo.carbohydrate = nutrient.amount;
                    }
                }

                foreach (Property property in response.nutrition.properties)
                {
                    if (property.name == "Glycemic Load")
                    {
                        nutrientInfo.glycemic_load = property.amount;
                    }
                    else if (property.name == "Glycemic Index")
                    {
                        nutrientInfo.glycemic_index = property.amount;
                    }
                }

                nutrients[foodName] = nutrientInfo;
                callback(nutrients);
            }
        }
    }
}