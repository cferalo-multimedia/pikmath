
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Pantalla de personalización del usuario.
///
/// Permite personalizar el avatar, elegir personajes predeterminados
/// y guardar el apodo y el avatar.
///
/// Al salir, regresa automáticamente a la escena desde la que se entró.
/// </summary>
public class UsuarioController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] AvatarView preview;

    [Tooltip("Campo para escribir el apodo.")]
    [SerializeField] TMP_InputField nicknameInput;

    [Tooltip("Texto opcional para mostrar avisos.")]
    [SerializeField] TMP_Text messageText;

    [Header("Navegación")]
    [Tooltip("Si está activado, vuelve a la escena desde la que se entró.")]
    [SerializeField] bool returnToPreviousScene = true;

    [Tooltip("Escena de destino si no existe una escena anterior.")]
    [SerializeField] string returnScene = "MenuPrincipal";

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
            Debug.LogError(
                "UsuarioController: falta la vista previa o su catálogo de avatares.",
                this
            );
            enabled = false;
            return;
        }

        catalog = preview.Catalog;

        current = PlayerProfile.HasProfile
            ? catalog.Validate(PlayerProfile.Avatar)
            : AvatarData.Default;

        if (nicknameInput != null)
        {
            nicknameInput.characterLimit = PlayerProfile.MaxNicknameLength;
            nicknameInput.text = PlayerProfile.Nickname;
        }

        ShowMessage("");
        Refresh();
    }

    // ===== Avatar personalizado =====

    public void SiguientePiel()
    {
        current.skinIndex = Cycle(current.skinIndex, catalog.skins, +1);
        UseCustom();
    }

    public void AnteriorPiel()
    {
        current.skinIndex = Cycle(current.skinIndex, catalog.skins, -1);
        UseCustom();
    }

    public void SiguientePelo()
    {
        current.hairIndex = Cycle(current.hairIndex, catalog.hairs, +1);
        UseCustom();
    }

    public void AnteriorPelo()
    {
        current.hairIndex = Cycle(current.hairIndex, catalog.hairs, -1);
        UseCustom();
    }

    public void SiguienteCara()
    {
        current.faceIndex = Cycle(current.faceIndex, catalog.faces, +1);
        UseCustom();
    }

    public void AnteriorCara()
    {
        current.faceIndex = Cycle(current.faceIndex, catalog.faces, -1);
        UseCustom();
    }

    public void UsarPersonalizado()
    {
        UseCustom();
    }

    void UseCustom()
    {
        current.usePreset = false;
        Refresh();
    }

    // ===== Aleatorio =====

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
        return (list == null || list.Length == 0)
            ? 0
            : Random.Range(0, list.Length);
    }

    // ===== Avatares predeterminados =====

    public void ElegirPredeterminado(int index)
    {
        current.presetIndex = index;
        current.usePreset = true;
        Refresh();
    }

    public void SiguientePredeterminado()
    {
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
        string nickname = nicknameInput != null
            ? PlayerProfile.CleanNickname(nicknameInput.text)
            : "";

        if (nicknameInput != null && nickname.Length == 0)
        {
            ShowMessage("¡Escribe tu apodo!");
            return;
        }

        // Guardar el apodo y el avatar.
        PlayerProfile.Save(nickname, current);

        // Volver a la escena desde la que se entró.
        Volver();
    }

    /// <summary>
    /// Sale de la personalización.
    /// Si se pulsa Volver, no guarda los cambios.
    /// </summary>
    
    public void Volver()
    {
        // Recuperar el destino específico desde el que se abrió Usuario.
        string target = returnScene;

        if (returnToPreviousScene &&
            !string.IsNullOrEmpty(SceneHistory.UsuarioReturnScene))
        {
            target = SceneHistory.UsuarioReturnScene;
        }

        // Si estamos dentro de una partida pausada,
        // descargamos solamente Usuario.
        Scene juego = SceneManager.GetSceneByName("Juego");

        if (juego.IsValid() && juego.isLoaded)
        {
            SceneManager.UnloadSceneAsync("Usuario");
            return;
        }

        // Volver a la escena correspondiente sin fundido.
        if (transicion != null)
        {
            transicion.LoadSceneWithoutFade(target);
        }
        else
        {
            SceneManager.LoadScene(target);
        }
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
