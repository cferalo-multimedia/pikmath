
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class AjustesController : MonoBehaviour
{
    [Header("Control de volumen")]
    [SerializeField] private Slider sliderVolumen;

    private const string CLAVE_VOLUMEN = "VolumenPIKMATH";

    private SceneTransition transicion;

    private void Awake()
    {
        transicion = GetComponent<SceneTransition>();
    }

    private void Start()
    {
        float volumen = PlayerPrefs.GetFloat(CLAVE_VOLUMEN, 1f);

        AudioListener.volume = volumen;

        if (sliderVolumen != null)
        {
            sliderVolumen.SetValueWithoutNotify(volumen);
        }

        if (SceneManager.GetSceneByName("Juego").isLoaded)
        {
            DesactivarEventSystemDeAjustes();
        }
    }

    private void DesactivarEventSystemDeAjustes()
    {
        foreach (GameObject root in gameObject.scene.GetRootGameObjects())
        {
            EventSystem[] sistemas =
                root.GetComponentsInChildren<EventSystem>(true);

            foreach (EventSystem sistema in sistemas)
            {
                sistema.enabled = false;
            }
        }
    }

    // Modificar volumen.
    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;

        PlayerPrefs.SetFloat(CLAVE_VOLUMEN, volumen);
        PlayerPrefs.Save();
    }

    // Abrir Usuario desde Ajustes.
    public void AbrirUsuario()
    {
        SceneHistory.SetUsuarioReturnScene("Ajustes");

        // Si estamos dentro de una partida,
        // cargar Usuario sin descargar Juego ni Ajustes.
        if (SceneManager.GetSceneByName("Juego").isLoaded)
        {
            if (!SceneManager.GetSceneByName("Usuario").isLoaded)
            {
                SceneManager.LoadScene("Usuario", LoadSceneMode.Additive);
            }

            return;
        }

        // Si estamos en los ajustes normales.
        if (transicion != null)
        {
            transicion.LoadSceneWithoutFade("Usuario");
        }
        else
        {
            SceneManager.LoadScene("Usuario");
        }
    }

    // Volver desde Ajustes.
    public void VolverMenuPrincipal()
    {
        Scene juego = SceneManager.GetSceneByName("Juego");

        if (juego.IsValid() && juego.isLoaded)
        {
            // Si estamos en una partida,
            // cerrar únicamente Ajustes.
            SceneManager.UnloadSceneAsync("Ajustes");
            return;
        }

        // En los ajustes normales, regresar al menú principal.
        if (transicion != null)
        {
            transicion.LoadSceneWithoutFade("MenuPrincipal");
        }
        else
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
    }
}
