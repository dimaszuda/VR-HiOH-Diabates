using UnityEngine;

public class showButton : MonoBehaviour {
    public MenuMakanList menuMakan;
    public GameObject button;
    public saveFinalButton saveFinal;

    void Update() {
        int listActive = menuMakan.GetCurrentListCount();
        bool isClick = saveFinal.checkClicked();

        if (listActive == 4 & isClick == false) {
            button.SetActive(true);
        } // true
        else {
            button.SetActive(false);
        }
    }
}

