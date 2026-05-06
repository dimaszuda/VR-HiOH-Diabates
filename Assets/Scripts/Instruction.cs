using UnityEngine;
using UnityEngine.UI;

public class Instruction : MonoBehaviour {
    public GameObject overlay;
    public GameObject instructionPanel;

    public void onStartButton() {
        overlay.SetActive(false);
        instructionPanel.SetActive(false);
    }
}
