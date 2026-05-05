using UnityEngine;

public class GeneResultData : MonoBehaviour
{
    public static GeneResultData Instance;

    public bool isKnobSaved = false;
    public bool isGeneCreated = false;

    // Mom Gene Result Data
    public Color[][] momChild = new Color[5][];

    // Dad Gene Result Data
    public Color[][] dadChild = new Color[5][];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Tetap hidup di scene lain
            // Inisialisasi isi array 5 anak, masing-masing punya 3 warna
            for (int i = 0; i < 5; i++)
            {
                momChild[i] = new Color[3];
                dadChild[i] = new Color[3];
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
