using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Configuración del fundido")]
    [SerializeField] private CanvasGroup panelFundido;
    [SerializeField] private float duracionFundido = 1f;

    private bool cambiandoEscena = false;

    private void Start()
    {
        // Comprobamos que tenemos asignado el Canvas Group.
        if (panelFundido == null)
        {
            Debug.LogError("No se ha asignado el Canvas Group del fundido.");
            enabled = false;
            return;
        }

        // Comenzamos con la pantalla negra.
        panelFundido.alpha = 1f;
        panelFundido.blocksRaycasts = true;

        // Hacemos aparecer progresivamente la escena.
        StartCoroutine(Fundido(1f, 0f));
    }

    // Método público para cambiar de escena con fundido.
    public void LoadSceneWithFade(string nombreEscena)
    {
        if (cambiandoEscena || panelFundido == null)
            return;

        cambiandoEscena = true;

        StartCoroutine(CambiarEscena(nombreEscena));
    }

    private IEnumerator CambiarEscena(string nombreEscena)
    {
        // Evitamos que se pueda interactuar durante el cambio.
        panelFundido.blocksRaycasts = true;

        // Oscurecemos la pantalla.
        yield return StartCoroutine(
            Fundido(panelFundido.alpha, 1f)
        );

        // Cargamos la siguiente escena cuando todo está negro.
        SceneManager.LoadScene(nombreEscena);
    }

    private IEnumerator Fundido(float inicio, float final)
    {
        float tiempo = 0f;
        float duracion = Mathf.Max(0.01f, duracionFundido);

        while (tiempo < duracion)
        {
            tiempo += Time.unscaledDeltaTime;

            float progreso = Mathf.Clamp01(tiempo / duracion);

            panelFundido.alpha = Mathf.Lerp(
                inicio,
                final,
                progreso
            );

            yield return null;
        }

        panelFundido.alpha = final;

        // Cuando termina de aparecer la escena,
        // volvemos a permitir las interacciones.
        if (final == 0f)
        {
            panelFundido.blocksRaycasts = false;
        }


    }

    // Cambiar de escena inmediatamente, sin fundido de salida.
    public void LoadSceneWithoutFade(string nombreEscena)
    {
        if (cambiandoEscena)
            return;

        cambiandoEscena = true;

        SceneManager.LoadScene(nombreEscena);
    }
}