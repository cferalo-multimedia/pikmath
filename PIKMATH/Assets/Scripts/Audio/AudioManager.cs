using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Gestor de audio de PIKMATH: música de fondo, efectos y silencio.
///
/// - Se crea solo al arrancar el juego (desde Assets/Resources/AudioManager.prefab)
///   y no se destruye al cambiar de escena, así la música no se corta.
/// - Añade automáticamente el sonido de clic a TODOS los botones y toggles
///   de cada escena (menos a los Pikmin y a los que tengan el componente SinSonidoClick).
/// - El volumen general lo siguen controlando los scripts de Ajustes y de pausa
///   (AudioListener.volume, clave "VolumenPIKMATH"). El silencio lo controla este script.
///
/// Desde cualquier script:
///     AudioManager.Acierto();
///     AudioManager.Error();
///     AudioManager.Reloj();
///     AudioManager.SetMuted(true);
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Música de fondo")]
    [SerializeField] AudioClip musicaFondo;
    [SerializeField, Range(0f, 1f)] float volumenMusica = 0.5f;

    [Header("Efectos")]
    [SerializeField] AudioClip sonidoClick;
    [SerializeField] AudioClip sonidoAcierto;
    [SerializeField] AudioClip sonidoError;
    [Tooltip("Opcional: al conseguir el reloj de tiempo extra.")]
    [SerializeField] AudioClip sonidoReloj;
    [SerializeField, Range(0f, 1f)] float volumenEfectos = 1f;

    const string KeyVolume = "VolumenPIKMATH";   // la misma clave que usan Ajustes y la pausa
    const string KeyMute   = "SilencioPIKMATH";

    static AudioManager instance;

    AudioSource musicSource;
    AudioSource sfxSource;
    readonly HashSet<int> hookedControls = new HashSet<int>();

    // ===== Creación automática =====

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoCreate()
    {
        if (instance != null) return;

        var prefab = Resources.Load<AudioManager>("AudioManager");
        if (prefab == null)
        {
            Debug.LogWarning("AudioManager: no se encuentra Assets/Resources/AudioManager.prefab. El juego funcionará sin sonido.");
            return;
        }
        Instantiate(prefab).name = "AudioManager";
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = volumenMusica;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = volumenEfectos;

        AudioListener.volume = PlayerPrefs.GetFloat(KeyVolume, 1f);
        ApplyMute(PlayerPrefs.GetInt(KeyMute, 0) == 1);

        if (musicaFondo != null)
        {
            musicSource.clip = musicaFondo;
            musicSource.Play();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        HookSceneControls();
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hookedControls.Clear();
        HookSceneControls();
    }

    // ===== Sonido de clic automático =====

    void HookSceneControls()
    {
        foreach (var button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!ShouldHook(button)) continue;
            button.onClick.AddListener(PlayClick);
        }

        foreach (var toggle in FindObjectsByType<Toggle>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!ShouldHook(toggle)) continue;
            toggle.onValueChanged.AddListener(_ => PlayClick());
        }
    }

    bool ShouldHook(Component control)
    {
        if (!hookedControls.Add(control.GetInstanceID())) return false;     // ya tiene sonido
        if (control.GetComponent<Pikmin>() != null) return false;            // los Pikmin tienen acierto/error
        if (control.GetComponent<SinSonidoClick>() != null) return false;
        return true;
    }

    // ===== API estática (se puede llamar aunque no exista el AudioManager) =====

    public static void Click()   { if (instance != null) instance.PlayClick(); }
    public static void Acierto() { if (instance != null) instance.PlaySfx(instance.sonidoAcierto); }
    public static void Error()   { if (instance != null) instance.PlaySfx(instance.sonidoError); }
    public static void Reloj()   { if (instance != null) instance.PlaySfx(instance.sonidoReloj); }

    public static void PlaySFX(AudioClip clip)
    {
        if (instance != null) instance.PlaySfx(clip);
    }

    /// <summary>Cambia la música de fondo (por ejemplo, una distinta para la partida).</summary>
    public static void CambiarMusica(AudioClip clip)
    {
        if (instance == null || clip == null || instance.musicSource.clip == clip) return;
        instance.musicSource.clip = clip;
        instance.musicSource.Play();
    }

    public static bool Muted => PlayerPrefs.GetInt(KeyMute, 0) == 1;

    public static void SetMuted(bool muted)
    {
        PlayerPrefs.SetInt(KeyMute, muted ? 1 : 0);
        PlayerPrefs.Save();   // necesario en WebGL
        if (instance != null) instance.ApplyMute(muted);
    }

    // ===== Interno =====

    void PlayClick()
    {
        PlaySfx(sonidoClick);
    }

    void PlaySfx(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, volumenEfectos);
    }

    void ApplyMute(bool muted)
    {
        musicSource.mute = muted;
        sfxSource.mute = muted;
    }
}
