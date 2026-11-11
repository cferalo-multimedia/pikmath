/// <summary>
/// Modos de juego (apartado 4 del GDD).
/// </summary>
public enum GameMode
{
    Basico,
    Contrarreloj
}

/// <summary>
/// Guarda lo que el jugador ha elegido en los menús para que la escena Juego lo pueda leer.
/// Al ser estático, se mantiene al cambiar de escena.
///
/// Uso desde la pantalla de selección de niveles:
///     GameSession.Select(GameMode.Contrarreloj, 0);   // nivel 1 = índice 0
///     SceneManager.LoadScene("Juego");
/// </summary>
public static class GameSession
{
    public static GameMode Mode { get; private set; } = GameMode.Basico;

    /// <summary>0 = Nivel 1, 1 = Nivel 2, 2 = Nivel 3.</summary>
    public static int LevelIndex { get; private set; } = 0;

    /// <summary>True cuando el jugador ha elegido modo y nivel desde los menús.</summary>
    public static bool HasSelection { get; private set; } = false;

    public static void Select(GameMode mode, int levelIndex)
    {
        Mode = mode;
        LevelIndex = levelIndex;
        HasSelection = true;
    }
}
