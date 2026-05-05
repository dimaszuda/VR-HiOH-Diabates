using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    void Awake()
    {
        // Cek kalau sudah ada object BackgroundMusic, jangan duplikat
        int musicObjCount = FindObjectsOfType<BackgroundMusic>().Length;
        if (musicObjCount > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
