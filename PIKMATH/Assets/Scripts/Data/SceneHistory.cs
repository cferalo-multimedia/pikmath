using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Recuerda automáticamente cuál fue la escena anterior.
/// No hay que añadirlo a ningún objeto: funciona solo desde que arranca el juego.
///
/// Uso: SceneHistory.Previous  → nombre de la escena anterior ("" si no hay).
/// </summary>
public static class SceneHistory
{
    public static string Previous { get; private set; } = "";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Previous = "";
        SceneManager.activeSceneChanged -= OnSceneChanged;
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    static void OnSceneChanged(Scene from, Scene to)
    {
        if (!string.IsNullOrEmpty(from.name) && from.name != to.name)
            Previous = from.name;
    }
}
