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
        else
        {
            HandleMobileControls();
        }

        // Aplicar límites al movimiento de la cámara
        ClampCameraPosition();
    }

    void HandlePCControls()
    {
        // Movimiento con WASD relativo a la orientación completa de la cámara
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Obtener las direcciones de la cámara
        Vector3 forward = transform.forward; // Dirección hacia adelante de la cámara
        Vector3 right = transform.right;    // Dirección hacia la derecha de la cámara

        // Calcular el movimiento relativo a la cámara, incluyendo el eje Y
        Vector3 move = (forward * moveZ + right * moveX) * m_moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        // Rotación con el mouse
        if (Input.GetMouseButton(1)) // Botón derecho del mouse
        {
            float rotationY = Input.GetAxis("Mouse X") * m_rotationSpeed * Time.deltaTime;
            float rotationX = -Input.GetAxis("Mouse Y") * m_rotationSpeed * Time.deltaTime;

            // Acumular las rotaciones en los ejes X e Y
            m_rotationX = Mathf.Clamp(m_rotationX + rotationX, -80f, 80f); // Limitar la rotación vertical
            m_rotationY += rotationY;

            // Aplicar las rotaciones acumuladas
            transform.rotation = Quaternion.Euler(m_rotationX, m_rotationY, 0f);
        }

        // Zoom con la rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            m_camera.fieldOfView = Mathf.Clamp(m_camera.fieldOfView - scroll * m_pcZoomSpeed, 20f, 100f);
        }
    }

    void HandleMobileControls()
    {
        if (Input.touchCount == 1)
        {
            // Movimiento con un dedo relativo a la orientación de la cámara
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 forward = transform.forward;
                Vector3 right = transform.right;

                // Calcular el movimiento relativo a la cámara
                Vector3 delta = (forward * -touch.deltaPosition.y + right * -touch.deltaPosition.x) * m_touchMoveSpeed * Time.deltaTime;
                transform.Translate(delta, Space.World);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Zoom con dos dedos (pellizcar)
            if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                float prevDistance = (touch0.position - touch0.deltaPosition).magnitude - (touch1.position - touch1.deltaPosition).magnitude;
                float currentDistance = (touch0.position - touch1.position).magnitude;
                float zoomDelta = (currentDistance - prevDistance) * m_zoomSpeed * Time.deltaTime;

                m_camera.fieldOfView = Mathf.Clamp(m_camera.fieldOfView - zoomDelta, 20f, 100f);
            }

            // Rotación con dos dedos (uno fijo y otro en movimiento)
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