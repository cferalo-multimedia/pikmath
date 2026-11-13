using UnityEngine;

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

    // Abrir selección de niveles sin fundido de salida.
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
}