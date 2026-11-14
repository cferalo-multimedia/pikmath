using System;

/// <summary>
/// Describe un avatar con unos pocos números (no guarda imágenes).
///
/// Hay dos tipos de avatar:
///  - Personalizado (Character 1): se combinan piel + pelo + cara.
///  - Predeterminado (Character 5): un personaje completo, sin poder cambiar nada.
/// </summary>
[Serializable]
public struct AvatarData
{
    public bool usePreset;   // true = personaje predeterminado
    public int presetIndex;  // índice dentro de AvatarCatalog.presets
    public int skinIndex;    // índice dentro de AvatarCatalog.skins
    public int hairIndex;    // índice dentro de AvatarCatalog.hairs
    public int faceIndex;    // índice dentro de AvatarCatalog.faces

    public static AvatarData Default => new AvatarData
    {
        usePreset = false,
        presetIndex = 0,
        skinIndex = 0,
        hairIndex = 0,
        faceIndex = 0
    };

    public static AvatarData Preset(int index) => new AvatarData
    {
        usePreset = true,
        presetIndex = index
    };
}
