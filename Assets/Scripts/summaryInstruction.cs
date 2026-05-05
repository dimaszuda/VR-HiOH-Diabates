using UnityEngine;
using UnityEngine.UI;

public class summaryInstruction : MonoBehaviour {
    public GameObject overlay;
    public GameObject instructionPanel;
    public GameObject saveButton;

    public void onStartButton() {
        overlay.SetActive(false);
        instructionPanel.SetActive(false); 
        saveButton.SetActive(true);
    }
}
