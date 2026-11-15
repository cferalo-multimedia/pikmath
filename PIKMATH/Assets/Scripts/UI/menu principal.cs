
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalController : MonoBehaviour
{
    private SceneTransition transicion;

    private void Awake()
    {
        transicion = GetComponent<SceneTransition>();
    }

    // Botón JUEGO BASE.
    public void SeleccionarJuegoBase()
    {
        GameSession.SetMode(GameMode.Basico);
        IrASeleccionNiveles();
    }

    // Botón CONTRARRELOJ.
    public void SeleccionarContrarreloj()
    {
        GameSession.SetMode(GameMode.Contrarreloj);
        IrASeleccionNiveles();
    }

    // Abrir selección de niveles sin fundido.
    private void IrASeleccionNiveles()
    {
        if (transicion != null)
        {
            transicion.LoadSceneWithoutFade("SeleccionNiveles");
        }
        else
        {
            Debug.LogError("No se encuentra SceneTransition en Canvas.");
        }
    }

    // Abrir Usuario desde el menú principal.
    public void AbrirUsuario()
    {
        SceneHistory.SetUsuarioReturnScene("MenuPrincipal");

        if (transicion != null)
        {
            transicion.LoadSceneWithoutFade("Usuario");
        }
        else
        {
            SceneManager.LoadScene("Usuario");
        }
    }
}
