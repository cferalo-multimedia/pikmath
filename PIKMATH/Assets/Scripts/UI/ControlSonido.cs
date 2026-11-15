
using UnityEngine;
using UnityEngine.UI;

public class ControlSonido : MonoBehaviour
{
    [Header("Botón de sonido")]
    [SerializeField] private Button botonSonido;

    [Header("Textos del botón")]
    [SerializeField] private TMPro.TMP_Text textoBoton;

    private bool silenciado = false;

    private const string CLAVE_SILENCIO = "JuegoSilenciado";

    private void Start()
    {
        // Recuperar el estado guardado.
        silenciado = PlayerPrefs.GetInt(CLAVE_SILENCIO, 0) == 1;

        ActualizarSonido();
    }

    public void AlternarSonido()
    {
        silenciado = !silenciado;

        PlayerPrefs.SetInt(CLAVE_SILENCIO, silenciado ? 1 : 0);
        PlayerPrefs.Save();

        ActualizarSonido();
    }

    private void ActualizarSonido()
    {
        AudioListener.pause = silenciado;

        if (textoBoton != null)
        {
            textoBoton.text = silenciado ? "SONIDO: OFF" : "SONIDO: ON";
        }
    }
}
