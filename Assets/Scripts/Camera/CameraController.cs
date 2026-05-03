using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public float heightOffset = 2f;

    [Header("Zoom")]
    public float distance    = 10f;
    public float minDistance = 3f;
    public float maxDistance = 20f;
    public float zoomSpeed   = 4f;

    [Header("Rotation")]
    public float rotationSpeed = 120f;
    public float minPitch      = 15f;
    public float maxPitch      = 75f;

    [Header("Occlusion")]
    public LayerMask occlusionLayers = ~0;  // todo por defecto; excluir Player/UI en Inspector
    public float clipRadius = 0.25f;        // radio del SphereCast
    public float clipOffset = 0.15f;        // margen frente al obstáculo

    private float yaw;
    private float pitch = 35f;

    // Detección si estamos arrastrando la cámara
    private Vector2 mouseDownPos;
    private Vector2 lastMousePos;
    private const float DragThreshold = 5f;

    public bool IsDraggingCamera { get; private set; }

    void Start()
    {
        // Inicializar yaw desde la rotación actual
        yaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;
        HandleZoom();
        HandleRotation();
        ApplyTransform();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);
    }

    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            mouseDownPos     = Input.mousePosition;
            lastMousePos     = Input.mousePosition;
            IsDraggingCamera = false;
        }

        if (Input.GetMouseButton(1))
        {
            Vector2 current = Input.mousePosition;
            Vector2 delta   = current - lastMousePos;

            if (!IsDraggingCamera && Vector2.Distance(current, mouseDownPos) > DragThreshold)
                IsDraggingCamera = true;

            if (IsDraggingCamera)
            {
                yaw   += delta.x * rotationSpeed * Time.deltaTime;
                pitch -= delta.y * rotationSpeed * Time.deltaTime;
                pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);
            }

            lastMousePos = current;
        }

        if (Input.GetMouseButtonUp(1))
            IsDraggingCamera = false;
    }

    void ApplyTransform()
    {
        Quaternion rot   = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivotPos = target.position + Vector3.up * heightOffset;
        Vector3 dir      = rot * new Vector3(0f, 0f, -distance);  // pivot → cámara deseada

        // Retroceder la cámara si un edificio/estructura bloquea la línea de visión
        float desiredDist = dir.magnitude;
        float actualDist  = desiredDist;

        if (Physics.SphereCast(pivotPos, clipRadius, dir.normalized,
                               out RaycastHit hit, desiredDist, occlusionLayers))
            actualDist = Mathf.Max(hit.distance - clipOffset, minDistance);

        transform.position = pivotPos + dir.normalized * actualDist;
        transform.LookAt(pivotPos);
    }

    // Devuelve la dirección forward de la cámara proyectada al plano horizontal
    public Vector3 CameraForwardFlat()
    {
        Vector3 f = transform.forward;
        f.y = 0f;
        return f.normalized;
    }
}
