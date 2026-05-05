using UnityEngine;

public class RemoveFoodList : MonoBehaviour {
    public MenuMakanList menuMakan;
    private int indexToRemove;

    public void SetIndex(int index) {
        indexToRemove = index;
    }

    public void OnRemoveFood() {
        menuMakan.RemoveFoodItems(indexToRemove);
    }
}
