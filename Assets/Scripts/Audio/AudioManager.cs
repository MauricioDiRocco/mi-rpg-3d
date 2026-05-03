using UnityEngine;

// Singleton. Maneja SFX y música de fondo.
// Colocar en la escena en un GameObject "AudioManager" y asignar los clips.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Música")]
    public AudioClip bgmMain;
    public AudioClip bgmBoss;
    [Range(0f, 1f)] public float bgmVolume = 0.4f;

    [Header("SFX - Combate")]
    public AudioClip sfxHit;
    public AudioClip sfxCritical;
    public AudioClip sfxHeal;
    public AudioClip sfxSkill;
    public AudioClip sfxPlayerDeath;
    public AudioClip sfxEnemyDeath;

    [Header("SFX - Inventario/UI")]
    public AudioClip sfxPickup;
    public AudioClip sfxEquip;
    public AudioClip sfxGold;
    public AudioClip sfxLevelUp;
    public AudioClip sfxPortal;
    public AudioClip sfxError;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop   = true;
        bgmSource.volume = bgmVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    void Start()
    {
        PlayBGM(bgmMain);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource?.Stop();

    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (Instance == null || clip == null) return;
        Instance.sfxSource.PlayOneShot(clip, volume);
    }

    public static void PlayHit()      => Play(Instance?.sfxHit,         0.8f);
    public static void PlayCritical() => Play(Instance?.sfxCritical,    1.0f);
    public static void PlayHeal()     => Play(Instance?.sfxHeal,        0.9f);
    public static void PlaySkill()    => Play(Instance?.sfxSkill,       0.85f);
    public static void PlayPickup()   => Play(Instance?.sfxPickup,      0.7f);
    public static void PlayEquip()    => Play(Instance?.sfxEquip,       0.8f);
    public static void PlayGold()     => Play(Instance?.sfxGold,        0.7f);
    public static void PlayLevelUp()  => Play(Instance?.sfxLevelUp,     1.0f);
    public static void PlayPortal()   => Play(Instance?.sfxPortal,      0.9f);
    public static void PlayError()    => Play(Instance?.sfxError,       0.6f);

    public void SetBGMVolume(float v) { bgmVolume = v; if (bgmSource) bgmSource.volume = v; }
    public void SetSFXVolume(float v) { if (sfxSource) sfxSource.volume = v; }
}
