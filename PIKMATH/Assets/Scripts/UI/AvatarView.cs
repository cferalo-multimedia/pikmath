using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dibuja un avatar con varias capas de Image. Se usa en la pantalla de Usuario
/// y en cada fila del Ranking.
///
/// Estructura recomendada (dentro del círculo blanco del fondo):
///   Avatar          → este script (+ Image con forma de círculo y Mask, opcional)
///     ├── Piel      → Image
///     ├── Pelo      → Image
///     ├── Cara      → Image
///     └── Predeterminado → Image
/// Las cuatro Image deben tener el mismo tamaño y posición (anclajes "stretch").
/// El orden de dibujo correcto es: Piel → Pelo → Cara (de arriba abajo en la jerarquía).
/// </summary>
public class AvatarView : MonoBehaviour
{
    [SerializeField] AvatarCatalog catalog;

    [Header("Capas del avatar personalizado")]
    [SerializeField] Image skinImage;
    [SerializeField] Image hairImage;
    [SerializeField] Image faceImage;

    [Header("Avatar predeterminado")]
    [SerializeField] Image presetImage;

    [Header("Opciones")]
    [Tooltip("Si está activado, al empezar muestra el avatar guardado del jugador.")]
    [SerializeField] bool showPlayerOnStart = false;

    public AvatarCatalog Catalog => catalog;

    void Start()
    {
        if (showPlayerOnStart) Show(PlayerProfile.Avatar);
    }

    public void Show(AvatarData data)
    {
        if (catalog == null)
        {
            Debug.LogError("AvatarView: falta asignar el catálogo de avatares.", this);
            return;
        }

        data = catalog.Validate(data);

        SetLayer(skinImage,   !data.usePreset, catalog.GetSkin(data.skinIndex));
        SetLayer(hairImage,   !data.usePreset, catalog.GetHair(data.hairIndex));
        SetLayer(faceImage,   !data.usePreset, catalog.GetFace(data.faceIndex));
        SetLayer(presetImage,  data.usePreset, catalog.GetPreset(data.presetIndex));
    }

    static void SetLayer(Image image, bool visible, Sprite sprite)
    {
        if (image == null) return;
        image.gameObject.SetActive(visible && sprite != null);
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
    }
}
