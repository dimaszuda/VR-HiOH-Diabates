using UnityEngine;
using UnityEngine.UI;

public class NewInstruction : MonoBehaviour {
    public GameObject overlay;
    public GameObject instructionPanel;
    public GameObject[] instruction;

    public void onNextButton() {
        instruction[1].SetActive(true);
        instruction[0].SetActive(false);
    }

    public void onSecondButton() {
        instruction[1].SetActive(false);
        instruction[2].SetActive(true);
    }

    public void onEndButton() {
        overlay.SetActive(false);
        instructionPanel.SetActive(false);
    }
}