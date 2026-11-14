using UnityEngine;

/// <summary>
/// Lista de todos los sprites que se pueden usar en un avatar.
/// Se crea desde el menú: Create → PIKMATH → Catálogo de avatares.
///
/// Todos los PNG deben tener el mismo tamaño de lienzo (128×128) y estar alineados,
/// como los de Art/Characters, para que las capas encajen.
/// </summary>
[CreateAssetMenu(fileName = "CatalogoAvatares", menuName = "PIKMATH/Catálogo de avatares")]
public class AvatarCatalog : ScriptableObject
{
    [Header("Avatar personalizado (Character 1)")]
    [Tooltip("P4_skin_tone_1 … 8")]
    public Sprite[] skins;

    [Tooltip("P4_Character_1_Hair_Variation_1 … 6")]
    public Sprite[] hairs;

    [Tooltip("P4_Character_1_Face_Variation_1 … 10")]
    public Sprite[] faces;

    [Header("Avatares predeterminados (Character 5)")]
    [Tooltip("P4_Character_5_Face_Variation_1 … 6. Son personajes completos.")]
    public Sprite[] presets;

    public Sprite GetSkin(int i)   => Get(skins, i);
    public Sprite GetHair(int i)   => Get(hairs, i);
    public Sprite GetFace(int i)   => Get(faces, i);
    public Sprite GetPreset(int i) => Get(presets, i);

    static Sprite Get(Sprite[] list, int i)
    {
        if (list == null || list.Length == 0) return null;
        return list[Mathf.Clamp(i, 0, list.Length - 1)];
    }

    /// <summary>Corrige índices fuera de rango (por ejemplo, si se quitan sprites del catálogo).</summary>
    public AvatarData Validate(AvatarData d)
    {
        d.presetIndex = Clamp(d.presetIndex, presets);
        d.skinIndex   = Clamp(d.skinIndex, skins);
        d.hairIndex   = Clamp(d.hairIndex, hairs);
        d.faceIndex   = Clamp(d.faceIndex, faces);
        if (d.usePreset && (presets == null || presets.Length == 0)) d.usePreset = false;
        return d;
    }

    static int Clamp(int i, Sprite[] list)
    {
        if (list == null || list.Length == 0) return 0;
        return Mathf.Clamp(i, 0, list.Length - 1);
    }
}
