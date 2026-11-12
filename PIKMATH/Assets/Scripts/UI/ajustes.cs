using UnityEngine;
using UnityEngine.UI;

public class AjustesController : MonoBehaviour
{
    [Header("Control de volumen")]
    [SerializeField] private Slider sliderVolumen;

    private SceneTransition transicion;

    private const string CLAVE_VOLUMEN = "VolumenPIKMATH";

    private void Awake()
    {
        // Buscamos SceneTransition en el mismo Canvas.
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
        else
        {
            Debug.LogError("Falta asignar SliderVolumen.");
        }
    }

    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;

        PlayerPrefs.SetFloat(CLAVE_VOLUMEN, volumen);
        PlayerPrefs.Save();
    }

    public void VolverMenuPrincipal()
    {
        if (transicion != null)
        {
            transicion.LoadSceneWithFade("MenuPrincipal");
        }
        else
        {
            Debug.LogError("SceneTransition no está en el Canvas.");
        }
    }
}