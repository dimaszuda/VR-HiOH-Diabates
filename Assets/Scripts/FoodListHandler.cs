using UnityEngine;

public class FoodListHandler : MonoBehaviour {
    public MenuMakanList menuMakan;
    private MetadataFood foodData;
    public AudioSource audioSource; // Tambahkan AudioSource
    public AudioClip clickSound;

    void Start() {
        foodData = GetComponent<MetadataFood>();
    }

    public void OnClickFood() {
        PlayClickSound();
        int index = menuMakan.GetCurrentListCount();
        if (index >= menuMakan.foodItems.Length)
            return; // daftar penuh

        menuMakan.setFoodInfo(foodData.foodName, foodData.karboInfo, foodData.GIInfo, foodData.GLInfo);
        menuMakan.AddFoodItem(index);
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}