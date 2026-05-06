using UnityEngine;
using UnityEngine.UI;

public class GenerateButtonBinder : MonoBehaviour
{
    public Button generateButton;

    void Start()
    {
        if (GenGenerator.Instance != null && generateButton != null)
        {
            // hapus listener lama lalu tambah listener baru
            generateButton.onClick.RemoveAllListeners();
            generateButton.onClick.AddListener(GenGenerator.Instance.GenerateGenes);
        }
    }
}
