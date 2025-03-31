using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 20f; // Velocidad de movimiento del personaje

    PlayerInput m_PlayerInput = null;

    Vector3 m_Accelerometer = Vector3.zero;
    Vector3 m_Gyroscope = Vector3.zero;

    void Awake()
    {
        if (UnityEngine.InputSystem.Accelerometer.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Accelerometer.current);
        }

        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
        }

        m_PlayerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Accelerometer.current != null)
        {
            m_Accelerometer = UnityEngine.InputSystem.Accelerometer.current.acceleration.ReadValue();

            // Crear un vector de movimiento basado en el acelerómetro
            Vector3 movement = new Vector3(m_Accelerometer.x, 0, -m_Accelerometer.y);

            // Aplicar el movimiento al personaje
            transform.Translate(movement * moveSpeed * Time.deltaTime);
        }
        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            m_Gyroscope = UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue();
        }
    } 
}
