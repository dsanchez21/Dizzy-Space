using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento del personaje
    private Vector2 moveInput;  // Almacena la entrada de movimiento
    private Rigidbody rb;

    private Vector3 accelerometerInput; // Entrada del acelerómetro

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Habilitar el acelerómetro si está disponible
        if (UnityEngine.InputSystem.Accelerometer.current != null)
        {
            InputSystem.EnableDevice(UnityEngine.InputSystem.Accelerometer.current);
        }
    }

    void FixedUpdate()
    {
        // Calcular el movimiento desde el teclado o el acelerómetro
        Vector3 movement = Vector3.zero;

        if (UnityEngine.InputSystem.Accelerometer.current != null)
        {
            // Leer los datos del acelerómetro
            accelerometerInput = UnityEngine.InputSystem.Accelerometer.current.acceleration.ReadValue();
            movement = new Vector3(accelerometerInput.x, 0, accelerometerInput.y) * moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            // Usar la entrada del teclado si no hay acelerómetro
            movement = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.fixedDeltaTime;
        }

        // Mover el objeto directamente sin rotarlo
        rb.MovePosition(rb.position + movement);
    }

    // Método llamado por el Input System cuando se detecta movimiento del teclado
    public void OnMove(InputValue value)
    {
        // Leer la entrada del movimiento desde el teclado
        moveInput = value.Get<Vector2>();
    }
}
