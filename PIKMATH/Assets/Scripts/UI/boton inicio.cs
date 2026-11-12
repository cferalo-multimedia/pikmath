using UnityEngine;

public class InicioController : MonoBehaviour
{
    private SceneTransition transicion;

    private void Awake()
    {
        // Obtenemos el componente de transición
        // que está en el mismo Canvas.
        transicion = GetComponent<SceneTransition>();
    }

    public void IrAHistoria()
    {
        if (transicion != null)
        {
            transicion.LoadSceneWithFade("Historia");
        }
    }
}