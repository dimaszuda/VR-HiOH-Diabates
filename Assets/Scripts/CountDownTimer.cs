using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VirtualClock : MonoBehaviour
{
    public static VirtualClock Instance; // <-- Tambahkan ini

    public TextMeshProUGUI timeText;
    private float virtualSeconds = 0f;
    private float realDuration = 186f; // 3 menit 6 detik
    private float virtualDuration = 18 * 60 * 60f; // 18 jam
    private float timeScale;
    private int startHour = 6;
    private bool isRunning = false;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timeScale = virtualDuration / realDuration;
    }

    void Update()
    {
        if (!isRunning) return;

        if (virtualSeconds < virtualDuration)
        {
            virtualSeconds += Time.deltaTime * timeScale;

            int totalMinutes = Mathf.FloorToInt(virtualSeconds / 60f);
            int hours = startHour + totalMinutes / 60;
            int minutes = totalMinutes % 60;

            if (hours > 23) hours = 23;
            if (hours == 23 && minutes > 59) minutes = 59;

            timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        }
        else
        {
            float endHour = 24f;
            PlayerPrefs.SetFloat("End Hour", endHour);
            timeText.text = "00:00"; // Reset waktu saat selesai
            SceneManager.LoadScene("Show Result");
        }
    }

    // Tambahkan method ini untuk akses dari luar
    public float GetVirtualHour()
    {
        int totalMinutes = Mathf.FloorToInt(virtualSeconds / 60f);
        float hour = startHour + (totalMinutes / 60f);
        return Mathf.Min(hour, 23.983f); // Maksimal 23:59
    }

    public void StartClock() {
        isRunning = true;
    }

}
