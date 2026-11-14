using UnityEngine;

/// <summary>
/// Perfil del jugador: apodo + avatar. Se guarda en el navegador con PlayerPrefs,
/// así que no se recogen datos personales de los alumnos en ningún servidor.
///
/// Uso:
///     PlayerProfile.Nickname        → apodo guardado ("" si no hay perfil)
///     PlayerProfile.Avatar          → avatar guardado
///     PlayerProfile.Save(nombre, avatar)
/// </summary>
public static class PlayerProfile
{
    public const int MaxNicknameLength = 12;

    const string KeyExists   = "pikmath_perfil_creado";
    const string KeyNickname = "pikmath_perfil_apodo";
    const string KeyPreset   = "pikmath_perfil_usa_predeterminado";
    const string KeyPresetId = "pikmath_perfil_predeterminado";
    const string KeySkin     = "pikmath_perfil_piel";
    const string KeyHair     = "pikmath_perfil_pelo";
    const string KeyFace     = "pikmath_perfil_cara";

    public static bool HasProfile => PlayerPrefs.GetInt(KeyExists, 0) == 1;

    public static string Nickname => PlayerPrefs.GetString(KeyNickname, "");

    /// <summary>Apodo para mostrar: si no hay perfil, devuelve el texto indicado.</summary>
    public static string DisplayName(string fallback = "Tú")
    {
        string n = Nickname;
        return string.IsNullOrWhiteSpace(n) ? fallback : n;
    }

    public static AvatarData Avatar => new AvatarData
    {
        usePreset   = PlayerPrefs.GetInt(KeyPreset, 0) == 1,
        presetIndex = PlayerPrefs.GetInt(KeyPresetId, 0),
        skinIndex   = PlayerPrefs.GetInt(KeySkin, 0),
        hairIndex   = PlayerPrefs.GetInt(KeyHair, 0),
        faceIndex   = PlayerPrefs.GetInt(KeyFace, 0)
    };

    public static void Save(string nickname, AvatarData avatar)
    {
        PlayerPrefs.SetInt(KeyExists, 1);
        PlayerPrefs.SetString(KeyNickname, CleanNickname(nickname));
        PlayerPrefs.SetInt(KeyPreset, avatar.usePreset ? 1 : 0);
        PlayerPrefs.SetInt(KeyPresetId, avatar.presetIndex);
        PlayerPrefs.SetInt(KeySkin, avatar.skinIndex);
        PlayerPrefs.SetInt(KeyHair, avatar.hairIndex);
        PlayerPrefs.SetInt(KeyFace, avatar.faceIndex);
        PlayerPrefs.Save();   // necesario en WebGL para que se guarde en el navegador
    }

    public static string CleanNickname(string nickname)
    {
        if (nickname == null) return "";
        nickname = nickname.Trim();
        if (nickname.Length > MaxNicknameLength) nickname = nickname.Substring(0, MaxNicknameLength);
        return nickname;
    }

    // ===== Puntuaciones (las guarda GameManager en el modo contrarreloj) =====

    /// <summary>Mejor número de aciertos en contrarreloj para un nivel (1, 2 o 3).</summary>
    public static int GetTimeAttackRecord(int levelNumber)
    {
        return PlayerPrefs.GetInt($"pikmath_record_contrarreloj_nivel{levelNumber}", 0);
    }

    /// <summary>Suma de los récords de contrarreloj de los tres niveles.</summary>
    public static int GetTotalScore(int levels = 3)
    {
        int total = 0;
        for (int i = 1; i <= levels; i++) total += GetTimeAttackRecord(i);
        return total;
    }
}
