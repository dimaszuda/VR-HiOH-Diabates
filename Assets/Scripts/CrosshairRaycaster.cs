using UnityEngine;
using UnityEngine.UI;

public class CrosshairRaycaster : MonoBehaviour
{
    public Color buttonHoverColor = Color.blue;
    public Color homepageHoverColor = Color.grey;

    public Camera mainCamera;
    private Button currentButton;
    private Button lastButton;
    private Color lastButtonOriginalColor;

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

    }

    void Update()
    {
        // Buat ray dari tengah viewport (crosshair)
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        currentButton = null;

        // Jika raycast mengenai objek
        if (Physics.Raycast(ray, out hit))
        {
            // Dapatkan tombol dari objek yang terkena ray
            currentButton = hit.collider.GetComponentInParent<Button>();

            // Jika tombol terdeteksi
            if (currentButton != null)
            {
                // Hanya ubah warna jika tombol berubah
                if (currentButton != lastButton)
                {
                    // Kembalikan warna tombol sebelumnya (jika ada)
                    if (lastButton != null)
                        ResetButtonColor(lastButton);

                    // Simpan warna asli tombol sebelum diubah
                    Image buttonImage = currentButton.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        lastButtonOriginalColor = buttonImage.color;
                    }

                    if (currentButton.CompareTag("homepage")) {
                        // Ubah warna tombol yang sedang di-hover
                        ChangeButtonColor(currentButton, homepageHoverColor);
                        lastButton = currentButton;
                    }
                    
                    else if (currentButton.CompareTag("panelMulai")) {
                        ChangeButtonColor(currentButton, buttonHoverColor);
                        lastButton = currentButton;
                    }
                }

                return;
            }
        }

        // Jika tidak ada tombol di-hover, kembalikan warna normal
        if (lastButton != null)
        {
            ResetButtonColor(lastButton);
            lastButton = null;
        }

    }

    void ExecuteButton(Button btn)
    {
        // Eksekusi semua listener tombol secara manual
        btn.onClick.Invoke();
    }

    void ChangeButtonColor(Button btn, Color color)
    {
        Image buttonImage = btn.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }

    void ResetButtonColor(Button btn)
    {
        Image buttonImage = btn.GetComponent<Image>();
        if (buttonImage != null)
        {
            // Kembalikan ke warna aslinya
            buttonImage.color = lastButtonOriginalColor;
        }
    }
}
