using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Colocar en un trigger (Collider con IsTrigger = true).
// Lleva al player a otra zona/escena cuando entra en el trigger.
public class ZonePortal : MonoBehaviour
{
    [Header("Destino")]
    public string     destinationScene = "";
    public string     portalName       = "Portal";

    [Header("Nivel recomendado")]
    public int        recommendedLevel = 1;

    [Header("UI")]
    public TextMeshPro labelText;

    private bool  playerInRange;
    private float interactTimer;

    void Start()
    {
        if (labelText != null)
            labelText.text = $"{portalName}\n(Lv. {recommendedLevel}+)";
    }

    void Update()
    {
        if (!playerInRange) return;

        // Hold F para entrar
        if (Input.GetKey(KeyCode.F))
        {
            interactTimer += Time.deltaTime;
            if (interactTimer >= 1.5f)
                EnterPortal();
        }
        else
        {
            interactTimer = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        interactTimer = 0f;
        Debug.Log($"[Portal] Mantene F para entrar a {portalName}");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        interactTimer = 0f;
    }

    void EnterPortal()
    {
        if (string.IsNullOrEmpty(destinationScene)) return;

        var player = GameObject.FindGameObjectWithTag("Player");
        var playerStats = player?.GetComponent<PlayerStats>();

        if (playerStats != null && playerStats.level < recommendedLevel)
        {
            Debug.Log($"[Portal] Se recomienda ser nivel {recommendedLevel}+ (sos nivel {playerStats.level})");
        }

        Debug.Log($"[Portal] Cargando zona: {destinationScene}");
        SceneManager.LoadScene(destinationScene);
    }
}
