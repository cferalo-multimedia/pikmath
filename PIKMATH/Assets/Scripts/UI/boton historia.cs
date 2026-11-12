using System.Collections;
using UnityEngine;

public class HistoriaController : MonoBehaviour
{
    [Header("Configuración de la historia")]
    [SerializeField] private float tiempoAntesDeSaltar = 30f;

    [SerializeField] private string siguienteEscena = "MenuPrincipal";

    private SceneTransition transicion;
    private bool saltoIniciado = false;

    private void Awake()
    {
        // Obtenemos el componente de transición del Canvas.
        transicion = GetComponent<SceneTransition>();
    }

    private void Start()
    {
        // Iniciamos la cuenta atrás al entrar en la historia.
        StartCoroutine(SaltoAutomatico());
    }

    private IEnumerator SaltoAutomatico()
    {
        // Esperamos 30 segundos reales.
        yield return new WaitForSecondsRealtime(tiempoAntesDeSaltar);

        // Si nadie ha pulsado SALTAR, avanzamos automáticamente.
        SaltarHistoria();
    }

    public void SaltarHistoria()
    {
        // Evitamos ejecutar el salto varias veces.
        if (saltoIniciado)
            return;

        if (transicion == null)
        {
            Debug.LogError("No se ha encontrado SceneTransition.");
            return;
        }

        saltoIniciado = true;

        // Pasamos al menú principal con fundido.
        transicion.LoadSceneWithFade(siguienteEscena);
    }
}