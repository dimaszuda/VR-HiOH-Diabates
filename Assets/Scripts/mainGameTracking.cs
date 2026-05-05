using UnityEngine;

public class mainGameTracking : MonoBehaviour
{    
    private Quaternion rotFix;
    
    [Range(0.1f, 1f)]
    public float sensitivity = 1f; // 1 = full sensitivity, 0.1 = sangat lambat

    void Start()
    {
        Input.gyro.enabled = true;
        // Koreksi orientasi awal: HP landscape → Unity world
        rotFix = Quaternion.Euler(90, 0, 0);
    }

    void Update()
    {      
        // Update rotasi dari gyroscope
        Quaternion gyro = Input.gyro.attitude;
        gyro = new Quaternion(gyro.x, gyro.y, -gyro.z, -gyro.w);
        
        Quaternion targetRotation = rotFix * gyro;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, sensitivity);
    }
}