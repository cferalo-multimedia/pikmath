
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JuegoUIController : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private GameObject panelAjustesPausa;

    [Header("Volumen")]
    [SerializeField] private Slider sliderVolumen;

    private const string CLAVE_VOLUMEN = "VolumenPIKMATH";

    private void Awake()
    {
        // Nos aseguramos de que el tiempo
        // no esté detenido al entrar en Juego.
        Time.timeScale = 1f;
    }

    private void Start()
    {
        panelPausa.SetActive(false);
        panelAjustesPausa.SetActive(false);

        // Recuperar volumen guardado.
        float volumen = PlayerPrefs.GetFloat(CLAVE_VOLUMEN, 1f);

        AudioListener.volume = volumen;

        if (sliderVolumen != null)
        {
            sliderVolumen.SetValueWithoutNotify(volumen);
        }
    }

    // Abrir el menú de pausa.
    public void Pausar()
    {
        panelPausa.SetActive(true);
        panelAjustesPausa.SetActive(false);

        Time.timeScale = 0f;
    }

    // Continuar la partida.
    public void Continuar()
    {
        panelPausa.SetActive(false);
        panelAjustesPausa.SetActive(false);

        Time.timeScale = 1f;
    }

    // Reiniciar el nivel actual.
    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    // Abrir los ajustes desde la pausa.
    public void AbrirAjustesPausa()
    {
        panelPausa.SetActive(false);
        panelAjustesPausa.SetActive(true);
    }

    // Volver al menú de pausa.
    public void VolverAPausa()
    {
        panelAjustesPausa.SetActive(false);
        panelPausa.SetActive(true);
    }

    // Modificar volumen.
    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;

        PlayerPrefs.SetFloat(CLAVE_VOLUMEN, volumen);
        PlayerPrefs.Save();
    }

    // Salir de la partida y regresar al menú.
    public void SalirAlMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MenuPrincipal");
    }

    private void OnDestroy()
    {
        // Evita dejar el juego pausado
        // al abandonar la escena.
        Time.timeScale = 1f;
    }
}
