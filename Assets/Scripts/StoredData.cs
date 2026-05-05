using UnityEngine;

[System.Serializable]
public class StoredData {
    public string foodName;    // Misalnya "Nasi Goreng", "Sate Ayam"
    public string actName; // Misalnya "Burger", "Jogging"
    public float virtualHour;   // Misalnya 9.0 untuk jam 09:00, 13.5 untuk 13:30
    public Sprite icon;         // Ikon yang akan ditampilkan di grafik
    public float carbohydrate;
    public float glycemic_index;
    public float glycemic_load;
    public float glucose_change;
}
