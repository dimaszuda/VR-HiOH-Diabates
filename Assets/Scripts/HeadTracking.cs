using UnityEngine;

public class HeadTracking : MonoBehaviour
{
    [Header("Gyro Settings")]
    public float gyroSensitivity = 1f;
    public bool enableGyroSmoothing = true;
    public float gyroSmoothness = 5f;
    
    [Header("Gyro Limits")]
    public bool limitGyroRotation = true;
    public float maxGyroTilt = 30f; // Batasi rotasi Z (miring kiri-kanan)
    public float maxGyroPitch = 45f; // Batasi rotasi X (atas-bawah)
    
    [Header("Target Settings")]
    public Transform targetPanel; // Target Panel untuk di look at
    public bool lookAtTargetOnStart = true;
    
    [Header("Calibration")]
    public bool autoRecalibrateOnStart = true;
    public float calibrationDelay = 4f; // Waktu delay sebelum auto calibrate
    public KeyCode recalibrateKey = KeyCode.R;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private Quaternion rotFix;
    private DragToLookController dragController;
    private Quaternion lastGyroRotation;
    private Quaternion smoothedGyroRotation;
    private Quaternion initialGyroRotation;
    private bool gyroInitialized = false;

    void Start()
    {
        Input.gyro.enabled = true;
        
        // Koreksi orientasi: HP landscape → Unity world
        rotFix = Quaternion.Euler(90, 0, 0);
        
        // Get drag controller
        dragController = GetComponent<DragToLookController>();
        
        // Initialize smoothed rotation
        smoothedGyroRotation = Quaternion.identity;
        
        // Look at target panel first (if specified)
        if (lookAtTargetOnStart && targetPanel != null)
        {
            LookAtTarget();
        }
        
        // Auto calibrate after delay, or manual calibrate
        if (autoRecalibrateOnStart)
        {
            Invoke("InitializeGyro", calibrationDelay);
            if (showDebugInfo) Debug.Log($"Auto-calibration will happen in {calibrationDelay} seconds. Hold phone in desired viewing position!");
        }
        else
        {
            Invoke("InitializeGyro", 0.5f);
        }
    }

    void InitializeGyro()
    {
        // Capture initial gyro rotation as reference
        Quaternion rawGyro = Input.gyro.attitude;
        rawGyro = new Quaternion(rawGyro.x, rawGyro.y, -rawGyro.z, -rawGyro.w);
        initialGyroRotation = rotFix * rawGyro;
        gyroInitialized = true;
        
        if (showDebugInfo) Debug.Log("Gyro initialized");
    }

    void Update()
    {
        if (!gyroInitialized) return;
        
        // Manual recalibrate with key press
        if (Input.GetKeyDown(recalibrateKey))
        {
            RecalibrateGyro();
        }
        
        // Get raw gyro data
        Quaternion rawGyro = Input.gyro.attitude;
        rawGyro = new Quaternion(rawGyro.x, rawGyro.y, -rawGyro.z, -rawGyro.w);
        Quaternion currentGyroRotation = rotFix * rawGyro;
        
        // Calculate relative rotation from initial position
        Quaternion relativeRotation = Quaternion.Inverse(initialGyroRotation) * currentGyroRotation;
        
        // Apply sensitivity
        Vector3 euler = relativeRotation.eulerAngles;
        
        // Convert to -180 to 180 range
        if (euler.x > 180) euler.x -= 360;
        if (euler.y > 180) euler.y -= 360;
        if (euler.z > 180) euler.z -= 360;
        
        // Apply sensitivity and limits
        euler.x = Mathf.Clamp(euler.x * gyroSensitivity, -maxGyroPitch, maxGyroPitch);
        euler.y = euler.y * gyroSensitivity; // No Y limit (horizontal rotation is OK)
        euler.z = Mathf.Clamp(euler.z * gyroSensitivity, -maxGyroTilt, maxGyroTilt);
        
        Quaternion targetGyroRotation = Quaternion.Euler(euler);
        
        // Apply smoothing
        if (enableGyroSmoothing)
        {
            smoothedGyroRotation = Quaternion.Slerp(smoothedGyroRotation, targetGyroRotation, gyroSmoothness * Time.deltaTime);
        }
        else
        {
            smoothedGyroRotation = targetGyroRotation;
        }
        
        // Apply to drag controller or transform
        if (dragController != null)
        {
            dragController.SetBaseRotation(smoothedGyroRotation);
        }
        else
        {
            transform.localRotation = smoothedGyroRotation;
        }
    }
    
    // Public methods for external control
    public void LookAtTarget()
    {
        if (targetPanel != null)
        {
            // Calculate rotation to look at target
            Vector3 directionToTarget = targetPanel.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            
            // Set initial camera rotation to look at target
            transform.rotation = lookRotation;
            
            // Update drag controller base rotation if available
            if (dragController != null)
            {
                dragController.SetBaseRotation(lookRotation);
            }
            
            if (showDebugInfo) Debug.Log("Camera now looking at target panel");
        }
        else
        {
            if (showDebugInfo) Debug.LogWarning("Target panel not assigned!");
        }
    }
    
    public void RecalibrateGyro()
    {
        if (gyroInitialized)
        {
            Quaternion rawGyro = Input.gyro.attitude;
            rawGyro = new Quaternion(rawGyro.x, rawGyro.y, -rawGyro.z, -rawGyro.w);
            initialGyroRotation = rotFix * rawGyro;
            smoothedGyroRotation = Quaternion.identity;
            
            if (showDebugInfo) Debug.Log("Gyro recalibrated");
        }
    }
    
    public void SetGyroEnabled(bool enabled)
    {
        Input.gyro.enabled = enabled;
        if (!enabled)
        {
            smoothedGyroRotation = Quaternion.identity;
            if (dragController != null)
            {
                dragController.SetBaseRotation(Quaternion.identity);
            }
        }
    }
    
    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUILayout.BeginArea(new Rect(10, 220, 300, 200));
            GUILayout.Label("=== HEAD TRACKING ===");
            GUILayout.Label($"Gyro Enabled: {Input.gyro.enabled}");
            GUILayout.Label($"Initialized: {gyroInitialized}");
            GUILayout.Label($"Target Panel: {(targetPanel != null ? targetPanel.name : "None")}");
            GUILayout.Label($"Sensitivity: {gyroSensitivity:F1}");
            GUILayout.Label($"Smoothing: {(enableGyroSmoothing ? "ON" : "OFF")}");
            
            Vector3 euler = smoothedGyroRotation.eulerAngles;
            if (euler.x > 180) euler.x -= 360;
            if (euler.y > 180) euler.y -= 360;
            if (euler.z > 180) euler.z -= 360;
            
            GUILayout.Label($"Rotation: X:{euler.x:F1} Y:{euler.y:F1} Z:{euler.z:F1}");
            GUILayout.Label("---");
            GUILayout.Label($"Press {recalibrateKey} to recalibrate");
            
            if (GUILayout.Button("Recalibrate Now"))
            {
                RecalibrateGyro();
            }
            
            if (GUILayout.Button("Look At Target"))
            {
                LookAtTarget();
            }
            
            if (!gyroInitialized && autoRecalibrateOnStart)
            {
                GUILayout.Label("CALIBRATING... Hold phone steady!");
            }
            
            GUILayout.EndArea();
        }
    }
}