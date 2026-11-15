
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneHistory
{
    public static string Previous { get; private set; } = "";

    // Guarda específicamente desde dónde se ha abierto Usuario.
    public static string UsuarioReturnScene { get; private set; } = "MenuPrincipal";

    public static void SetUsuarioReturnScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            UsuarioReturnScene = sceneName;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Previous = "";
        UsuarioReturnScene = "MenuPrincipal";

        SceneManager.activeSceneChanged -= OnSceneChanged;
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    static void OnSceneChanged(Scene from, Scene to)
    {
        if (!string.IsNullOrEmpty(from.name) && from.name != to.name)
        {
            Previous = from.name;
        }
    }

    public static void RecordCurrentScene()
    {
        Scene current = SceneManager.GetActiveScene();

        if (current.IsValid() && !string.IsNullOrEmpty(current.name))
        {
            Previous = current.name;
        }
    }

}
