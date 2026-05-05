using UnityEngine;

public class guideplay : MonoBehaviour {
    public GameObject homepageCanvas;
    public GameObject panelGuide;
    public GameObject[] guide;
    public StageLoader stageLoader;

    private int index = 0;

    public void showGuide() {
        homepageCanvas.SetActive(false);
        panelGuide.SetActive(true);

        // matikan semua dulu biar aman
        foreach (GameObject g in guide) {
            g.SetActive(false);
        }

        // mulai dari index 0
        index = 0;
        if (guide.Length > 0) {
            guide[index].SetActive(true);
        }

        Debug.Log($"showGuide → index = {index}, total = {guide.Length}");
    }

    public void onNextButton() {
        if (index < guide.Length - 1) {
            guide[index].SetActive(false);
            index++;
            guide[index].SetActive(true);
            Debug.Log($"Next → index = {index}");
        } else {
            Debug.Log("Next → sudah di guide terakhir");
        }
    }

    public void onBackButton() {
        if (index > 0 && index < guide.Length) {
            guide[index].SetActive(false);
            index--;
            guide[index].SetActive(true);
            Debug.Log($"Back → index = {index}");
        } else {
            Debug.Log("Back → sudah di guide pertama atau index invalid");
        }
    }

    public void onCloseButton() {
        // matikan semua guide
        foreach (GameObject g in guide) {
            g.SetActive(false);
        }

        panelGuide.SetActive(false);
        homepageCanvas.SetActive(true);

        index = 0; // reset index
        Debug.Log("Close → reset ke index 0");

        stageLoader.LoadHomepageScene();
    }
}
