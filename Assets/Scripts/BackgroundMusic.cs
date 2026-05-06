using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    void Awake()
    {
        int musicObjCount = FindObjectsOfType<BackgroundMusic>().Length;
        if (musicObjCount > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
