using UnityEngine;

public class AutoRecenter : MonoBehaviour
{
    [Header("Assign di Inspector")]
    public Transform worldOrigin;      // Parent seluruh dunia
    public Transform monitorTarget;    // Monitor atau target yang mau dihadap
    public Camera mainCamera;          // Kamera user (XR camera)

    [Header("Delay opsional")]
    public float delayBeforeRecenter = 0.2f; // beri jeda sedikit supaya sensor stabil

    private void Start()
    {
        // Jalankan fungsi recenter setelah jeda
        Invoke(nameof(RecenterToMonitor), delayBeforeRecenter);
    }

    void RecenterToMonitor()
    {
        if (worldOrigin == null || monitorTarget == null || mainCamera == null)
        {
            Debug.LogWarning("AutoRecenter: Pastikan semua field sudah di-assign di Inspector.");
            return;
        }

        // 1. Arah kamera sekarang (XZ)
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        // 2. Arah ke monitor (XZ)
        Vector3 toMonitor = (monitorTarget.position - mainCamera.transform.position);
        toMonitor.y = 0f;
        toMonitor.Normalize();

        // 3. Hitung sudut delta yaw
        float deltaYaw = Vector3.SignedAngle(camForward, toMonitor, Vector3.up);

        // 4. Putar worldOrigin kebalikan sudut itu
        worldOrigin.Rotate(Vector3.up, -deltaYaw, Space.World);
    }
}
