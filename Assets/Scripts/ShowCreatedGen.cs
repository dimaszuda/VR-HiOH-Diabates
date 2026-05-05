using UnityEngine;
using UnityEngine.UI;


public class ShowCreatedGen : MonoBehaviour {
    [Header("Dad Knob Images")]
    public Image[] dadKnobImages;

    [Header("Mom Knob Images")]
    public Image[] momKnobImages;

    public Button NextButton;

    void Start() {
        if (GeneResultData.Instance.isKnobSaved) {
            showCreatedResult();
        } else {
            Debug.Log("Genetik belum dibuat, tidak ada data untuk ditampilkan.");
        }
    }

    public void showCreatedResult() {
        NextButton.gameObject.SetActive(true);
        if (GeneResultData.Instance.dadChild == null || GeneResultData.Instance.momChild == null) return;
        Debug.Log("Warna anak pertama (ayah): " + GeneResultData.Instance.dadChild[0][0]);
        Debug.Log("Warna anak pertama (ibu): " + GeneResultData.Instance.momChild[0][0]);
        
        for (int index = 0; index < 5; index++) {
            Color[] dadColors = GeneResultData.Instance.dadChild[index];
            Color[] momColors = GeneResultData.Instance.momChild[index];

            for (int i = 0; i < 3; i++) {
                int imageIndex = index * 3 + i;
                dadKnobImages[imageIndex].color = dadColors[i];
                momKnobImages[imageIndex].color = momColors[i];
            }
        }
    }
}