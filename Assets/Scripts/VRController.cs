using UnityEngine;

public class VRController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform targetPanel; // Panel to look at initially
    
    [Header("Gyroscope Settings")]
    public bool enableGyro = true;
    public float gyroSensitivity = 1f;
    public bool gyroSmoothing = true;
    public float gyroSmoothness = 10f;
    
    [Header("Touch Drag Settings")]
    public bool enableTouchDrag = true;
    public float dragSensitivity = 2f;
    public bool dragSmoothing = false;
    public float dragSmoothness = 15f;
    
    [Header("Rotation Limits (PUBG Style)")]
    public bool limitVerticalRotation = true;
    public float maxLookUp = 60f;      // Batas lihat ke atas
    public float maxLookDown = -60f;   // Batas lihat ke bawah
    
    [Header("Comfort Settings")]
    public bool lockRoll = true;       // PENTING! Lock Z-axis
    public float deadZone = 0.5f;      // Dead zone untuk gerakan kecil
    public bool useWorldReference = true; // Horizon lock reference
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    // Internal variables
    private Camera mainCamera;
    private Quaternion initialRotation;
    private Quaternion gyroBaseRotation;
    private bool gyroCalibrated = false;
    
    // Current rotation values (Euler)
    private float currentPitch = 0f;   // X-axis (up/down)
    private float currentYaw = 0f;     // Y-axis (left/right)
    // Roll is LOCKED to 0 (no Z-axis rotation)
    
    // Touch input
    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    private float dragPitch = 0f;
    private float dragYaw = 0f;
    
    // Smoothing
    private float targetPitch = 0f;
    private float targetYaw = 0f;
    
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        
        // Fix camera background
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = Color.black;
        
        // Initialize camera direction
        InitializeCamera();
        
        // Initialize gyroscope
        if (SystemInfo.supportsGyroscope && enableGyro)
        {
            Input.gyro.enabled = true;
            Input.gyro.updateInterval = 0.0167f; // 60Hz
            Invoke("CalibrateGyro", 1f);
        }
        
        Debug.Log("PUBG-Style VR Controller initialized");
    }
    
    void InitializeCamera()
    {
        if (targetPanel != null)
        {
            // Point to target
            Vector3 directionToPanel = targetPanel.position - transform.position;
            transform.LookAt(targetPanel);
            
            // Get initial rotation (only pitch and yaw, ignore roll)
            Vector3 euler = transform.rotation.eulerAngles;
            currentPitch = NormalizeAngle(euler.x);
            currentYaw = euler.y;
            
            // Force roll to 0 (horizon lock)
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            
            Debug.Log($"Camera initialized - Pitch: {currentPitch:F1}°, Yaw: {currentYaw:F1}°");
        }
        
        initialRotation = transform.rotation;
        targetPitch = currentPitch;
        targetYaw = currentYaw;
    }
    
    void CalibrateGyro()
    {
        if (Input.gyro.enabled)
        {
            gyroBaseRotation = Input.gyro.attitude;
            gyroCalibrated = true;
            Debug.Log("Gyroscope calibrated");
        }
    }
    
    void Update()
    {
        // Handle gyroscope input
        if (enableGyro && gyroCalibrated)
        {
            HandleGyroInput();
        }
        
        // Handle touch drag input
        if (enableTouchDrag)
        {
            HandleTouchInput();
        }
        
        // Apply smoothing and rotation limits
        ApplyRotation();
        
        // Recenter hotkey
        if (Input.GetKeyDown(KeyCode.R))
        {
            RecenterView();
        }
    }
    
    void HandleGyroInput()
    {
        // Get current gyro rotation
        Quaternion currentGyroRotation = Input.gyro.attitude;
        Quaternion deltaGyro = Quaternion.Inverse(gyroBaseRotation) * currentGyroRotation;
        
        // Convert to Unity coordinate system
        deltaGyro = new Quaternion(deltaGyro.x, deltaGyro.y, -deltaGyro.z, -deltaGyro.w);
        
        // Extract only Pitch and Yaw from gyro (IGNORE ROLL!)
        Vector3 gyroEuler = deltaGyro.eulerAngles;
        float gyroPitch = NormalizeAngle(gyroEuler.x);
        float gyroYaw = NormalizeAngle(gyroEuler.y);
        
        // Apply dead zone
        if (Mathf.Abs(gyroPitch) > deadZone)
        {
            targetPitch = currentPitch + (gyroPitch * gyroSensitivity);
        }
        
        if (Mathf.Abs(gyroYaw) > deadZone)
        {
            targetYaw = currentYaw + (gyroYaw * gyroSensitivity);
        }
    }
    
    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 touchDelta = touch.position - lastTouchPosition;
                
                // Apply touch delta to target rotation
                dragYaw += touchDelta.x * dragSensitivity * 0.1f;
                dragPitch -= touchDelta.y * dragSensitivity * 0.1f; // Invert Y for natural feel
                
                targetPitch = currentPitch + dragPitch;
                targetYaw = currentYaw + dragYaw;
                
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }
    
    void ApplyRotation()
    {
        // Apply rotation limits
        if (limitVerticalRotation)
        {
            targetPitch = Mathf.Clamp(targetPitch, maxLookDown, maxLookUp);
        }
        
        // Smooth rotation or direct
        if (gyroSmoothing || dragSmoothing)
        {
            float smoothness = isDragging ? dragSmoothness : gyroSmoothness;
            currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, smoothness * Time.deltaTime);
            currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, smoothness * Time.deltaTime);
        }
        else
        {
            currentPitch = targetPitch;
            currentYaw = targetYaw;
        }
        
        // CRITICAL: Always lock roll to 0
        if (lockRoll)
        {
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, transform.rotation.eulerAngles.z);
        }
    }
    
    public void RecenterView()
    {
        Debug.Log("Recentering view...");
        
        // Reset drag offsets
        dragPitch = 0f;
        dragYaw = 0f;
        
        // Reinitialize
        InitializeCamera();
        
        if (gyroCalibrated)
        {
            CalibrateGyro();
        }
    }
    
    // Utility function to normalize angles to -180 to 180
    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
    
    // Public methods for external control
    public void SetGyroEnabled(bool enabled)
    {
        enableGyro = enabled;
    }
    
    public void SetTouchEnabled(bool enabled)
    {
        enableTouchDrag = enabled;
    }
    
    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 300));
            GUILayout.Label("=== PUBG-STYLE VR CONTROLLER ===");
            GUILayout.Label($"Gyro: {(enableGyro && gyroCalibrated ? "ON" : "OFF")}");
            GUILayout.Label($"Touch Drag: {(enableTouchDrag ? "ON" : "OFF")}");
            GUILayout.Label($"Is Dragging: {isDragging}");
            GUILayout.Label("---");
            GUILayout.Label($"Current Pitch: {currentPitch:F1}° (Vertical)");
            GUILayout.Label($"Current Yaw: {currentYaw:F1}° (Horizontal)");
            GUILayout.Label($"Roll: LOCKED at 0° (Horizon Stable)");
            GUILayout.Label("---");
            GUILayout.Label($"Drag Offset - Pitch: {dragPitch:F1}°, Yaw: {dragYaw:F1}°");
            
            if (targetPanel != null)
            {
                float distance = Vector3.Distance(transform.position, targetPanel.position);
                GUILayout.Label($"Panel Distance: {distance:F2}");
            }
            
            GUILayout.Label("---");
            GUILayout.Label("Roll Locked (No Horizon Tilt)");
            GUILayout.Label("Rotation Limits Applied");
            GUILayout.Label("Dead Zone Active");
            
            if (GUILayout.Button("Recenter"))
            {
                RecenterView();
            }
            
            GUILayout.EndArea();
        }
    }
}