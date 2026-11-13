using UnityEngine;
using UnityEngine.SceneManagement;

public class SeleccionNivelesController : MonoBehaviour
{
    // Seleccionar el nivel 1.
    public void SeleccionarNivel1()
    {
        IniciarNivel(0);
    }

    // Seleccionar el nivel 2.
    public void SeleccionarNivel2()
    {
        IniciarNivel(1);
    }

    // Seleccionar el nivel 3.
    public void SeleccionarNivel3()
    {
        IniciarNivel(2);
    }

    // Guardar la selección y acceder al juego.
    private void IniciarNivel(int nivel)
    {
        GameSession.Select(GameSession.Mode, nivel);

        SceneManager.LoadScene("Juego");
    }

    // Volver al menú principal.
    public void VolverMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}