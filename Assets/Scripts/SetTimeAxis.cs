using UnityEngine;
using TMPro;

public class SetTimeAxis : MonoBehaviour {
    public TextMeshProUGUI[] timeAxis;
    public float startHour = 6f;

    void Start() {
        float endHour = PlayerPrefs.GetFloat("End Hour", 24f);
        float totalHourRange = endHour - startHour;
        float interval = totalHourRange / timeAxis.Length;

        for (int i = 0; i < timeAxis.Length; i++) {
            float currentHour = startHour + interval * (i + 1); // mulai dari setelah start
            int hours = Mathf.FloorToInt(currentHour);
            int minutes = Mathf.RoundToInt((currentHour - hours) * 60);

            // Format ke "HH.mm"
            string formattedTime = $"{hours:00}.{minutes:00}";
            timeAxis[i].text = formattedTime;
        }
    }
}
