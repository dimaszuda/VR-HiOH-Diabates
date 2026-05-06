using UnityEngine;

[CreateAssetMenu(fileName = "ApiKeyConfig", menuName = "Config/API Key Config")]
public class ApiKeyConfig : ScriptableObject
{
    public string[] apiKeys;
}
