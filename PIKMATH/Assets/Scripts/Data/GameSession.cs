/// <summary>
/// Modos de juego.
/// </summary>
public enum GameMode
{
    Basico,
    Contrarreloj
}

/// <summary>
/// Guarda la modalidad y el nivel seleccionados.
/// </summary>
public static class GameSession
{
    public static GameMode Mode { get; private set; } = GameMode.Basico;

    public static int LevelIndex { get; private set; } = 0;

    public static bool HasSelection { get; private set; } = false;

    // Guarda el modo antes de elegir el nivel.
    public static void SetMode(GameMode mode)
    {
        Mode = mode;
        LevelIndex = 0;
        HasSelection = false;
    }

    // Guarda la selección completa de modo y nivel.
    public static void Select(GameMode mode, int levelIndex)
    {
        Mode = mode;
        LevelIndex = levelIndex;
        HasSelection = true;
    }
}