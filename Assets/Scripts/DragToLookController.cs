using UnityEngine;

public class DragToLookController : MonoBehaviour
{
    [Header("Touch Look Settings")]
    public bool enableDragLook = true;
    public float touchSensitivity = 1.5f; // Dikurangi dari 2f
    public bool invertY = false;
    
    [Header("Rotation Limits")]
    public bool limitVerticalRotation = true; // Default true
    public float minVerticalAngle = -60f;
    public float maxVerticalAngle = 60f;
    public bool limitHorizontalRotation = true;
    public float maxHorizontalAngle = 90f; // Batasi horizontal juga
    
    [Header("Smooth Rotation")]
    public bool useSmoothRotation = true; // Default true
    public float rotationSmoothness = 8f; // Dikurangi dari 10f
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    // Touch input variables
    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    
    // Rotation offset from drag input
    private float dragRotationX = 0f; // Vertical (pitch)
    private float dragRotationY = 0f; // Horizontal (yaw)
    
    // Touch velocity for damping
    private Vector2 touchVelocity = Vector2.zero;
    
    // Target rotation for smooth interpolation
    private Quaternion targetRotation;
    private Quaternion baseRotation;
    
    void Start()
    {
        // Store the initial rotation as base
        baseRotation = transform.rotation;
        targetRotation = baseRotation;
        
        if (showDebugInfo) Debug.Log("Drag To Look Controller initialized");
    }
    
    void Update()
    {
        if (enableDragLook)
        {
            // Handle touch input
            HandleTouchInput();
            
            // Handle mouse input (for editor testing)
            HandleMouseInput();
            
            // Apply drag rotation
            ApplyDragRotation();
        }
        
        // Reset drag rotation (optional hotkey)
        if (Input.GetKeyDown(KeyCode.T))
        {
            ResetDragRotation();
        }
        
        // Recalibrate gyro (optional hotkey)
        if (Input.GetKeyDown(KeyCode.R))
        {
            HeadTracking headTracking = GetComponent<HeadTracking>();
            if (headTracking != null)
            {
                headTracking.RecalibrateGyro();
            }
            ResetDragRotation();
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
                touchVelocity = Vector2.zero;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 touchDelta = touch.position - lastTouchPosition;
                
                // Store velocity for damping
                touchVelocity = touchDelta;
                
                // Apply touch delta to drag rotation
                ApplyTouchDelta(touchDelta);
                
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }
    
    void HandleMouseInput()
    {
        // Mouse input for testing in editor (only when no touch)
        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastTouchPosition = Input.mousePosition;
                isDragging = true;
                touchVelocity = Vector2.zero;
            }
            
            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector2 mouseDelta = (Vector2)Input.mousePosition - lastTouchPosition;
                
                // Store velocity for damping
                touchVelocity = mouseDelta;
                
                // Apply mouse delta to drag rotation
                ApplyTouchDelta(mouseDelta);
                
                lastTouchPosition = Input.mousePosition;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
    }
    
    void ApplyTouchDelta(Vector2 delta)
    {
        // Horizontal rotation (yaw) - lebih sensitive
        float horizontalDelta = delta.x * touchSensitivity * 0.15f;
        dragRotationY += horizontalDelta;
        
        // Vertical rotation (pitch) - kurang sensitive
        float verticalDelta;
        if (invertY)
            verticalDelta = delta.y * touchSensitivity * 0.1f;
        else
            verticalDelta = -delta.y * touchSensitivity * 0.1f;
        
        dragRotationX += verticalDelta;
        
        // Apply rotation limits
        if (limitVerticalRotation)
        {
            dragRotationX = Mathf.Clamp(dragRotationX, minVerticalAngle, maxVerticalAngle);
        }
        
        if (limitHorizontalRotation)
        {
            dragRotationY = Mathf.Clamp(dragRotationY, -maxHorizontalAngle, maxHorizontalAngle);
        }
    }
    
    void ApplyDragRotation()
    {
        // Calculate target rotation with drag offset
        Quaternion dragRotation = Quaternion.Euler(dragRotationX, dragRotationY, 0f);
        targetRotation = baseRotation * dragRotation;
        
        // Apply rotation (smooth or instant)
        if (useSmoothRotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
    
    // Public methods for external control
    public void ResetDragRotation()
    {
        dragRotationX = 0f;
        dragRotationY = 0f;
        touchVelocity = Vector2.zero;
        targetRotation = baseRotation;
        
        if (showDebugInfo) Debug.Log("Drag rotation reset");
    }
    
    public void SetBaseRotation(Quaternion newBaseRotation)
    {
        // Update base rotation from head tracking
        baseRotation = newBaseRotation;
        targetRotation = baseRotation * Quaternion.Euler(dragRotationX, dragRotationY, 0f);
    }
    
    public void SetBaseRotationToCurrent()
    {
        // Set current rotation as new base, keeping drag offset
        Quaternion currentDragRotation = Quaternion.Euler(dragRotationX, dragRotationY, 0f);
        baseRotation = transform.rotation * Quaternion.Inverse(currentDragRotation);
        
        if (showDebugInfo) Debug.Log("Base rotation updated to current");
    }
    
    public void SetDragOffset(float offsetX, float offsetY)
    {
        // Manually set drag offset
        dragRotationX = offsetX;
        dragRotationY = offsetY;
        
        // Apply limits
        if (limitVerticalRotation)
        {
            dragRotationX = Mathf.Clamp(dragRotationX, minVerticalAngle, maxVerticalAngle);
        }
        if (limitHorizontalRotation)
        {
            dragRotationY = Mathf.Clamp(dragRotationY, -maxHorizontalAngle, maxHorizontalAngle);
        }
    }
    
    public Vector2 GetDragOffset()
    {
        return new Vector2(dragRotationX, dragRotationY);
    }
    
    public void SetEnabled(bool enabled)
    {
        enableDragLook = enabled;
        if (!enabled)
        {
            touchVelocity = Vector2.zero;
            isDragging = false;
        }
    }
    
    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("=== DRAG TO LOOK ===");
            GUILayout.Label($"Enabled: {enableDragLook}");
            GUILayout.Label($"Is Dragging: {isDragging}");
            GUILayout.Label($"Drag Offset X: {dragRotationX:F1}°");
            GUILayout.Label($"Drag Offset Y: {dragRotationY:F1}°");
            GUILayout.Label($"Touch Velocity: {touchVelocity.magnitude:F1}");
            GUILayout.Label($"Touch Count: {Input.touchCount}");
            GUILayout.Label("---");
            GUILayout.Label("Drag screen to look around");
            GUILayout.Label("Press T to reset drag");
            GUILayout.Label("Press R to recalibrate gyro");
            
            if (GUILayout.Button("Reset Drag"))
            {
                ResetDragRotation();
            }
            
            if (GUILayout.Button($"Toggle: {(enableDragLook ? "ON" : "OFF")}"))
            {
                SetEnabled(!enableDragLook);
            }
            
            GUILayout.EndArea();
        }
    }
}