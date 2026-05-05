using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHandler : MonoBehaviour
{
    public static SceneLoadHandler Instance;
    public bool isBack = true;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Generate Genetik") 
        {
            if (GenGenerator.Instance != null)
            {
                GenGenerator.Instance.ResetState();

                // Kalau balik dari Pemilihan Genetik
                if (isBack)
                {
                    Debug.Log("Kembali dari Pemilihan Genetik");
                    // misalnya: munculin pesan, disable tombol, dsb
                    isBack = true; // reset lagi biar gak terus-terusan true
                }
            }

            // otomatis munculkan tombol Next kalau isGeneCreated true
            NextButton nextBtn = Object.FindFirstObjectByType<NextButton>();
            if (nextBtn != null)
            {
                nextBtn.gameObject.SetActive(GenGenerator.Instance.isGeneCreated);
            }
        }

        if (scene.name == "Pemilihan Genetik")
        {
            ShowCreatedGen showGen = Object.FindFirstObjectByType<ShowCreatedGen>();
            if (showGen != null)
            {
                GenGenerator.Instance.isGeneCreated = true;
                isBack = true;
                showGen.showCreatedResult();
            }
        }
    }
}
