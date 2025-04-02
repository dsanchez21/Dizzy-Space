using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("PC Controls")]
    public float m_moveSpeed = 10f; // Velocidad de movimiento con WASD
    public float m_rotationSpeed = 100f; // Velocidad de rotación con teclado
    public float m_pcZoomSpeed = 10f; // Velocidad de zoom con la rueda del ratón

    [Header("Mobile Controls")]
    public float m_touchMoveSpeed = 0.1f; // Velocidad de movimiento con un dedo
    public float m_zoomSpeed = 0.5f; // Velocidad de zoom con dos dedos
    public float m_touchRotationSpeed = 0.2f; // Velocidad de rotación con dos dedos

    [Header("Gamepad Controls")]
    public float m_gamepadMoveSpeed = 10f; // Velocidad de movimiento con el mando
    public float m_gamepadRotationSpeed = 100f; // Velocidad de rotación con los gatillos
    public float m_gamepadZoomSpeed = 10f; // Velocidad de zoom con el joystick derecho

    [Header("Movement Limits")]
    public Vector3 m_minLimits = new Vector3(-50f, 5f, -50f); // Límites mínimos en X, Y, Z
    public Vector3 m_maxLimits = new Vector3(50f, 20f, 50f);  // Límites máximos en X, Y, Z

    private Camera m_camera;
    private float m_rotationX = 0f; // Rotación acumulada en el eje X
    private float m_rotationY = 0f; // Rotación acumulada en el eje Y

    void Start()
    {
        m_camera = Camera.main;
    }

    void Update()
    {
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            HandlePCControls();
        }
        else if (Input.GetJoystickNames().Length > 0) // Detectar si hay un mando conectado
        {
            HandleGamepadControls();
        }
        else
        {
            HandleMobileControls();
        }

        // Aplicar límites al movimiento de la cámara
        ClampCameraPosition();
    }

    void HandlePCControls()
    {
        // Movimiento con WASD
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 move = (forward * moveZ + right * moveX) * m_moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        // Rotación con el ratón
        if (Input.GetMouseButton(1)) // Botón derecho del ratón
        {
            float rotationY = Input.GetAxis("Mouse X") * m_rotationSpeed * Time.deltaTime;
            float rotationX = -Input.GetAxis("Mouse Y") * m_rotationSpeed * Time.deltaTime;

            m_rotationX = Mathf.Clamp(m_rotationX + rotationX, -80f, 80f);
            m_rotationY += rotationY;

            transform.rotation = Quaternion.Euler(m_rotationX, m_rotationY, 0f);
        }

        // Zoom con la rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            m_camera.fieldOfView = Mathf.Clamp(m_camera.fieldOfView - scroll * m_pcZoomSpeed, 20f, 100f);
        }
    }

    void HandleGamepadControls()
    {
        // Movimiento con el joystick izquierdo
        float moveX = Input.GetAxis("GamepadHorizontal");
        float moveZ = Input.GetAxis("GamepadVertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 move = (forward * moveZ + right * moveX) * m_gamepadMoveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        // Rotación con los gatillos
        float rotationY = Input.GetAxis("GamepadRightTrigger") * m_gamepadRotationSpeed * Time.deltaTime;
        float rotationX = -Input.GetAxis("GamepadLeftTrigger") * m_gamepadRotationSpeed * Time.deltaTime;

        m_rotationX = Mathf.Clamp(m_rotationX + rotationX, -80f, 80f);
        m_rotationY += rotationY;

        transform.rotation = Quaternion.Euler(m_rotationX, m_rotationY, 0f);

        // Zoom con el joystick derecho
        float zoom = Input.GetAxis("GamepadZoom");
        if (zoom != 0)
        {
            m_camera.fieldOfView = Mathf.Clamp(m_camera.fieldOfView - zoom * m_gamepadZoomSpeed * Time.deltaTime, 20f, 100f);
        }
    }

    void HandleMobileControls()
    {
        if (Input.touchCount == 1)
        {
            // Movimiento con un dedo
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 forward = transform.forward;
                Vector3 right = transform.right;

                Vector3 delta = (forward * -touch.deltaPosition.y + right * -touch.deltaPosition.x) * m_touchMoveSpeed * Time.deltaTime;
                transform.Translate(delta, Space.World);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Zoom con dos dedos
            if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                float prevDistance = (touch0.position - touch0.deltaPosition).magnitude - (touch1.position - touch1.deltaPosition).magnitude;
                float currentDistance = (touch0.position - touch1.position).magnitude;
                float zoomDelta = (currentDistance - prevDistance) * m_zoomSpeed * Time.deltaTime;

                m_camera.fieldOfView = Mathf.Clamp(m_camera.fieldOfView - zoomDelta, 20f, 100f);
            }

            // Rotación con dos dedos
            if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Stationary)
            {
                float rotationDelta = touch0.deltaPosition.x * m_touchRotationSpeed * Time.deltaTime;
                m_rotationY += rotationDelta;
                transform.rotation = Quaternion.Euler(m_rotationX, m_rotationY, 0f);
            }
            else if (touch1.phase == TouchPhase.Moved && touch0.phase == TouchPhase.Stationary)
            {
                float rotationDelta = touch1.deltaPosition.x * m_touchRotationSpeed * Time.deltaTime;
                m_rotationY += rotationDelta;
                transform.rotation = Quaternion.Euler(m_rotationX, m_rotationY, 0f);
            }
        }
    }

    void ClampCameraPosition()
    {
        // Limitar la posición de la cámara dentro de los límites definidos
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, m_minLimits.x, m_maxLimits.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, m_minLimits.y, m_maxLimits.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, m_minLimits.z, m_maxLimits.z);
        transform.position = clampedPosition;
    }
}