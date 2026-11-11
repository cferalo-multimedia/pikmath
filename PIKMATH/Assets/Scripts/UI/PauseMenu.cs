using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Menú de pausa (apartado 7.2.7 del GDD): Continuar, Reiniciar, Ajustes y Salir.
/// Conecta cada botón a su método en el evento OnClick.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [Tooltip("Opcional: panel de ajustes (volumen, usuario).")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] string menuScene = "MenuPrincipal";

    public static bool IsPaused { get; private set; }

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        SetPaused(false);
    }

    // Botón de pausa del HUD
    public void Pause()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        SetPaused(true);
    }

    // Continuar
    public void Resume()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        SetPaused(false);
    }

    // Reiniciar el nivel actual
    public void Restart()
    {
        SetPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Ajustes
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // Salir al menú principal
    public void ExitToMenu()
    {
        SetPaused(false);
        SceneManager.LoadScene(menuScene);
    }

    void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;   // detiene temporizadores y animaciones
        AudioListener.pause = paused;
    }

    void OnDestroy()
    {
        // Evita que el juego se quede congelado al cambiar de escena
        if (IsPaused) SetPaused(false);
    }
}
