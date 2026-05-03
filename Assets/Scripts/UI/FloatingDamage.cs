using UnityEngine;
using TMPro;

public enum DamageType { Physical, Magic, Critical, Heal, Miss }

// Prefab: GameObject con TextMeshPro (World Space). Se auto-destruye.
public class FloatingDamage : MonoBehaviour
{
    public TextMeshPro label;

    [Header("Animacion")]
    public float duration  = 1.3f;
    public float riseSpeed = 2.2f;
    public float fadeStart = 0.55f; // fracción del duration a partir de donde fadeea

    private float timer;
    private Color baseColor;

    void Awake()
    {
        if (label == null) label = GetComponent<TextMeshPro>();
    }

    public void Initialize(float amount, DamageType type)
    {
        switch (type)
        {
            case DamageType.Physical: label.text = Mathf.RoundToInt(amount).ToString();             baseColor = Color.white;                      break;
            case DamageType.Magic:    label.text = Mathf.RoundToInt(amount).ToString();             baseColor = new Color(0.45f, 0.65f, 1f);      break;
            case DamageType.Critical: label.text = Mathf.RoundToInt(amount) + "!";                 baseColor = Color.yellow; label.fontSize *= 1.35f; break;
            case DamageType.Heal:     label.text = "+" + Mathf.RoundToInt(amount);                 baseColor = new Color(0.3f, 1f, 0.4f);         break;
            case DamageType.Miss:     label.text = "MISS";                                          baseColor = new Color(0.7f, 0.7f, 0.7f);       break;
        }

        label.color = baseColor;

        // Offset aleatorio para que no se superpongan
        transform.position += new Vector3(Random.Range(-0.4f, 0.4f), 0f, 0f);

        Destroy(gameObject, duration);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Subir
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // Mirar a la cámara
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }

        // Fade
        float fadeFraction = fadeStart * duration;
        if (timer >= fadeFraction)
        {
            float alpha  = 1f - (timer - fadeFraction) / (duration - fadeFraction);
            label.color  = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Clamp01(alpha));
        }
    }
}
