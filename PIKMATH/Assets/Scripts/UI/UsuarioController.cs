using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Pantalla de personalización del usuario (apartado 7.2.6 del GDD).
///
/// Botones (conecta cada OnClick a su método):
///   boton-usuario-piel    → SiguientePiel
///   boton-usuario-pelo    → SiguientePelo
///   boton-usuario-cara    → SiguienteCara
///   boton aleatorio       → Aleatorio
///   botones de personajes → ElegirPredeterminado (con el número 0-5 en el OnClick)
///                           o un solo botón → SiguientePredeterminado
///   boton-usuario-guardar → Guardar
///   boton-atras           → Volver (sale sin guardar)
///
/// Si el jugador ha elegido un personaje predeterminado y pulsa piel, pelo o cara,
/// se vuelve automáticamente al avatar personalizado.
/// </summary>
public class UsuarioController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] AvatarView preview;

    [Tooltip("Campo para escribir el apodo (TextMeshPro - Input Field).")]
    [SerializeField] TMP_InputField nicknameInput;

    [Tooltip("Opcional: texto para avisos (por ejemplo, «Escribe tu apodo»).")]
    [SerializeField] TMP_Text messageText;

    [Header("Navegación")]
    [Tooltip("Si está activado, al guardar o salir vuelve a la escena desde la que se entró (Ajustes o MenuPrincipal).")]
    [SerializeField] bool returnToPreviousScene = true;

    [Tooltip("Escena a la que vuelve si no se sabe cuál es la anterior (por ejemplo, al probar la escena directamente).")]
    [SerializeField] string returnScene = "Ajustes";

    AvatarData current;
    AvatarCatalog catalog;
    SceneTransition transicion;

    void Awake()
    {
        transicion = GetComponent<SceneTransition>();
    }

    void Start()
    {
        if (preview == null || preview.Catalog == null)
        {
            Debug.LogError("UsuarioController: falta la vista previa o su catálogo de avatares.", this);
            enabled = false;
            return;
        }

        catalog = preview.Catalog;
        current = PlayerProfile.HasProfile ? catalog.Validate(PlayerProfile.Avatar) : AvatarData.Default;

        if (nicknameInput != null)
        {
            nicknameInput.characterLimit = PlayerProfile.MaxNicknameLength;
            nicknameInput.text = PlayerProfile.Nickname;
        }

        ShowMessage("");
        Refresh();
    }

    // ===== Avatar personalizado =====

    public void SiguientePiel()    { current.skinIndex = Cycle(current.skinIndex, catalog.skins, +1); UseCustom(); }
    public void AnteriorPiel()     { current.skinIndex = Cycle(current.skinIndex, catalog.skins, -1); UseCustom(); }
    public void SiguientePelo()    { current.hairIndex = Cycle(current.hairIndex, catalog.hairs, +1); UseCustom(); }
    public void AnteriorPelo()     { current.hairIndex = Cycle(current.hairIndex, catalog.hairs, -1); UseCustom(); }
    public void SiguienteCara()    { current.faceIndex = Cycle(current.faceIndex, catalog.faces, +1); UseCustom(); }
    public void AnteriorCara()     { current.faceIndex = Cycle(current.faceIndex, catalog.faces, -1); UseCustom(); }

    /// <summary>Vuelve al avatar personalizado manteniendo la piel, el pelo y la cara elegidos.</summary>
    public void UsarPersonalizado() { UseCustom(); }

    void UseCustom()
    {
        current.usePreset = false;
        Refresh();
    }

    // ===== Aleatorio =====

    /// <summary>Combina al azar piel, pelo y cara (siempre distinto del avatar actual).</summary>
    public void Aleatorio()
    {
        AvatarData before = current;
        bool wasPreset = current.usePreset;

        for (int attempt = 0; attempt < 10; attempt++)
        {
            current.skinIndex = RandomIndex(catalog.skins);
            current.hairIndex = RandomIndex(catalog.hairs);
            current.faceIndex = RandomIndex(catalog.faces);

            bool same = !wasPreset
                && current.skinIndex == before.skinIndex
                && current.hairIndex == before.hairIndex
                && current.faceIndex == before.faceIndex;
            if (!same) break;
        }

        UseCustom();
    }

    static int RandomIndex(Sprite[] list)
    {
        return (list == null || list.Length == 0) ? 0 : Random.Range(0, list.Length);
    }

    // ===== Avatares predeterminados =====

    /// <summary>Para un botón por personaje: en el OnClick escribe su número (0, 1, 2...).</summary>
    public void ElegirPredeterminado(int index)
    {
        current.presetIndex = index;
        current.usePreset = true;
        Refresh();
    }

    public void SiguientePredeterminado()
    {
        // Si venimos del personalizado, empezamos por el personaje actual sin avanzar
        if (current.usePreset)
            current.presetIndex = Cycle(current.presetIndex, catalog.presets, +1);
        current.usePreset = true;
        Refresh();
    }

    public void AnteriorPredeterminado()
    {
        if (current.usePreset)
            current.presetIndex = Cycle(current.presetIndex, catalog.presets, -1);
        current.usePreset = true;
        Refresh();
    }

    // ===== Guardar y salir =====

    public void Guardar()
    {
        string nickname = nicknameInput != null ? PlayerProfile.CleanNickname(nicknameInput.text) : "";

        if (nicknameInput != null && nickname.Length == 0)
        {
            ShowMessage("¡Escribe tu apodo!");
            return;
        }

        PlayerProfile.Save(nickname, current);
        Volver();
    }

    /// <summary>Sale sin guardar los cambios.</summary>
    public void Volver()
    {
        string target = returnScene;
        if (returnToPreviousScene && !string.IsNullOrEmpty(SceneHistory.Previous))
            target = SceneHistory.Previous;

        if (transicion != null)
            transicion.LoadSceneWithFade(target);
        else
            SceneManager.LoadScene(target);
    }

    // ===== Utilidades =====

    void Refresh()
    {
        preview.Show(current);
    }

    void ShowMessage(string message)
    {
        if (messageText == null) return;
        messageText.text = message;
        messageText.gameObject.SetActive(!string.IsNullOrEmpty(message));
    }

    static int Cycle(int index, Sprite[] list, int step)
    {
        if (list == null || list.Length == 0) return 0;
        int n = list.Length;
        return ((index + step) % n + n) % n;
    }
}
