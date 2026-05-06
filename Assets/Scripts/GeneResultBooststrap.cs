using UnityEngine;

public class GeneResultBooststrap : MonoBehaviour {
    void Awake()
    {
        // Kalau belum ada GeneResultData, buat GameObject-nya
        if (GeneResultData.Instance == null)
        {
            GameObject go = new GameObject("GeneResultManager");
            go.AddComponent<GeneResultData>();         // tambah script penyimpan data
            go.AddComponent<SceneLoadHandler>();       // tambah script yang pantau scene loaded
            DontDestroyOnLoad(go);                     // biar tidak dihancurkan saat pindah scene
        }

        Destroy(gameObject); 
    }
}
