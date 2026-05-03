using UnityEngine;
using UnityEngine.UI;

// Minimapa top-down usando una segunda cámara y RawImage en el HUD.
// Setup:
// 1. Crear una Camera secundaria llamada "MinimapCamera"
// 2. Crear un RenderTexture (128x128) y asignarlo a MinimapCamera.targetTexture
// 3. Crear un RawImage en el Canvas y asignar el RenderTexture
// 4. Asignar la MinimapCamera y el RawImage a este script
public class MinimapUI : MonoBehaviour
{
    [Header("Referencias")]
    public Camera      minimapCamera;
    public RawImage    minimapImage;

    [Header("Configuracion")]
    public float       height          = 40f;   // altura de la cam sobre el player
    public float       orthographicSize = 25f;   // campo de visión (metros)
    [Range(0f, 360f)]
    public float       rotationOffset  = 0f;    // rotar el minimapa si querés que N sea arriba

    [Header("Iconos en el minimapa")]
    public RectTransform playerIcon;            // icono del player (flecha)
    public RectTransform[] enemyIcons;          // iconos de enemies (puntos rojos) - opcionales

    private Transform playerTransform;

    void Start()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null) playerTransform = playerGO.transform;

        if (minimapCamera != null)
        {
            minimapCamera.orthographic     = true;
            minimapCamera.orthographicSize = orthographicSize;
            minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            minimapCamera.cullingMask = ~0;
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null || minimapCamera == null) return;

        minimapCamera.transform.position = playerTransform.position + Vector3.up * height;

        // Rotar icono del player según su orientación
        if (playerIcon != null)
        {
            float playerYaw = playerTransform.eulerAngles.y;
            playerIcon.localEulerAngles = new Vector3(0f, 0f, -playerYaw + rotationOffset);
        }
    }

    public void SetVisible(bool visible)
    {
        if (minimapImage != null) minimapImage.gameObject.SetActive(visible);
        if (minimapCamera != null) minimapCamera.gameObject.SetActive(visible);
    }

    public void SetSize(float size)
    {
        orthographicSize = size;
        if (minimapCamera != null) minimapCamera.orthographicSize = size;
    }
}
