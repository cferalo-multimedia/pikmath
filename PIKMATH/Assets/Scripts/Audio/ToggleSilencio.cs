using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Pon este script en un Toggle para silenciar todo el juego.
/// Marcado = silencio. Recuerda la opción aunque se cierre el navegador.
/// No hace falta conectar nada en el OnValueChanged: lo hace el script.
///
/// Se puede usar en Ajustes y en los ajustes del menú de pausa.
/// </summary>
[RequireComponent(typeof(Toggle))]
public class ToggleSilencio : MonoBehaviour
{
    [Tooltip("Opcional: la barra de volumen se desactiva mientras está en silencio.")]
    [SerializeField] Slider sliderVolumen;

    Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    void Start()
    {
        toggle.SetIsOnWithoutNotify(AudioManager.Muted);
        toggle.onValueChanged.AddListener(OnChanged);
        UpdateSlider(toggle.isOn);
    }

    void OnChanged(bool muted)
    {
        AudioManager.SetMuted(muted);
        UpdateSlider(muted);
    }

    void UpdateSlider(bool muted)
    {
        if (sliderVolumen != null) sliderVolumen.interactable = !muted;
    }
}
