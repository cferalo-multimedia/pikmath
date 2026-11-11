using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controla la partida en la escena Juego (apartados 2 y 4 del GDD):
/// genera operaciones, comprueba el Pikmin pulsado, gestiona el tiempo,
/// las flores, el reloj de tiempo extra y el final del nivel.
///
/// Modo básico: tiempo límite por operación; el nivel termina al llegar a los aciertos necesarios.
/// Modo contrarreloj: tiempo total; hay que acertar el máximo de operaciones.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Niveles (en orden: 1, 2, 3)")]
    [SerializeField] LevelData[] levels;

    [Header("Referencias de la escena")]
    [Tooltip("Los 10 Pikmin, con números del 0 al 9.")]
    [SerializeField] Pikmin[] pikmins;
    [SerializeField] FlowerField flowerField;
    [SerializeField] GameUI ui;
    [Tooltip("Image del fondo; cambia según el nivel.")]
    [SerializeField] Image backgroundImage;

    [Header("Ritmo")]
    [Tooltip("Pausa entre una operación y la siguiente, para ver la animación.")]
    [SerializeField] float delayBetweenOperations = 1.2f;

    [Header("Pruebas en el editor")]
    [Tooltip("Si abres la escena Juego directamente (sin pasar por los menús), usa estos valores.")]
    [SerializeField] GameMode testMode = GameMode.Basico;
    [SerializeField, Range(0, 2)] int testLevelIndex = 0;

    [Header("Escenas")]
    [SerializeField] string menuScene = "MenuPrincipal";

    LevelData level;
    GameMode mode;
    int levelIndex;

    MathOperation current;
    string lastOperationText;
    int correctCount;
    float timer;            // básico: tiempo de la operación · contrarreloj: tiempo total
    bool clockActive;
    float clockTimer;
    bool waitingNext;
    bool finished;

    float Progress => level.correctToComplete > 0
        ? Mathf.Clamp01((float)correctCount / level.correctToComplete)
        : 0f;

    void Start()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("GameManager: no hay niveles asignados.");
            enabled = false;
            return;
        }

        if (GameSession.HasSelection)
        {
            mode = GameSession.Mode;
            levelIndex = GameSession.LevelIndex;
        }
        else
        {
            mode = testMode;
            levelIndex = testLevelIndex;
        }

        levelIndex = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
        level = levels[levelIndex];

        if (backgroundImage != null && level.background != null)
            backgroundImage.sprite = level.background;

        foreach (var p in pikmins) p.Clicked += OnPikminClicked;

        if (flowerField != null) flowerField.Clear();

        timer = mode == GameMode.Contrarreloj ? level.timeAttackDuration : level.timePerOperation;
        ui.SetTimer(timer);
        UpdateScoreUI();
        NextOperation();
    }

    void OnDestroy()
    {
        if (pikmins == null) return;
        foreach (var p in pikmins)
            if (p != null) p.Clicked -= OnPikminClicked;
    }

    void Update()
    {
        if (finished || level == null) return;

        if (mode == GameMode.Contrarreloj)
        {
            // El tiempo total corre siempre
            timer -= Time.deltaTime;
            ui.SetTimer(timer);
            if (timer <= 0f)
            {
                timer = 0f;
                EndGame();
                return;
            }
        }
        else if (!waitingNext)
        {
            // Tiempo de la operación actual
            timer -= Time.deltaTime;
            ui.SetTimer(timer);
            if (timer <= 0f) OnOperationTimeUp();
        }

        // Reloj de tiempo extra
        if (clockActive && !waitingNext)
        {
            clockTimer -= Time.deltaTime;
            ui.UpdateClock(clockTimer);
            if (clockTimer <= 0f)
            {
                clockActive = false;
                ui.HideClock();
            }
        }
    }

    void NextOperation()
    {
        // Evita repetir la misma operación dos veces seguidas
        for (int i = 0; i < 10; i++)
        {
            current = OperationGenerator.Generate(level.operationType, level.maxResult);
            if (current.text != lastOperationText) break;
        }
        lastOperationText = current.text;

        ui.SetOperation(current.text);
        SetPikminsInteractable(true);

        if (mode == GameMode.Basico)
        {
            timer = level.timePerOperation;
            ui.SetTimer(timer);
        }
        else
        {
            clockActive = Random.value < level.clockChance;
            if (clockActive)
            {
                clockTimer = level.clockTimeLimit;
                ui.ShowClock(clockTimer);
            }
            else
            {
                ui.HideClock();
            }
        }
    }

    void OnPikminClicked(Pikmin pikmin)
    {
        if (finished || waitingNext || PauseMenu.IsPaused) return;

        if (pikmin.Number == current.result)
            OnCorrect(pikmin);
        else
            OnWrong(pikmin);
    }

    void OnCorrect(Pikmin pikmin)
    {
        correctCount++;
        pikmin.PlayCorrect();
        ui.SetOperation(current.SolvedText);

        string message = "¡Correcto!";
        if (clockActive)
        {
            timer += level.clockBonus;
            message = $"¡Correcto! +{level.clockBonus:0} s";
            clockActive = false;
            ui.HideClock();
        }
        ui.ShowMessage(message, true);

        if (flowerField != null) flowerField.SetProgress(Progress);
        UpdateScoreUI();

        if (mode == GameMode.Basico && correctCount >= level.correctToComplete)
            StartCoroutine(EndAfterDelay());
        else
            StartCoroutine(NextAfterDelay(delayBetweenOperations));
    }

    void OnWrong(Pikmin pikmin)
    {
        pikmin.PlayWrong();
        ui.ShowMessage("¡Inténtalo de nuevo!", false);
    }

    void OnOperationTimeUp()
    {
        timer = 0f;
        ui.SetTimer(0f);
        ui.SetOperation(current.SolvedText);   // muestra la solución para aprender de ella
        ui.ShowMessage("¡Se acabó el tiempo!", false);
        StartCoroutine(NextAfterDelay(delayBetweenOperations * 1.5f));
    }

    IEnumerator NextAfterDelay(float delay)
    {
        waitingNext = true;
        SetPikminsInteractable(false);
        yield return new WaitForSeconds(delay);
        if (finished) yield break;
        waitingNext = false;
        NextOperation();
    }

    IEnumerator EndAfterDelay()
    {
        waitingNext = true;
        SetPikminsInteractable(false);
        yield return new WaitForSeconds(delayBetweenOperations);
        EndGame();
    }

    void EndGame()
    {
        if (finished) return;
        finished = true;
        SetPikminsInteractable(false);
        ui.HideClock();

        if (mode == GameMode.Basico)
        {
            // Guarda el nivel más alto completado (útil para la pantalla de niveles)
            int completed = PlayerPrefs.GetInt("pikmath_niveles_completados", 0);
            PlayerPrefs.SetInt("pikmath_niveles_completados", Mathf.Max(completed, levelIndex + 1));

            bool hasNext = levelIndex < levels.Length - 1;
            ui.ShowEndPanel("¡Prado completado!",
                $"Has llenado de flores el {level.levelName.ToLower()}.",
                hasNext);
        }
        else
        {
            string key = $"pikmath_record_contrarreloj_nivel{levelIndex + 1}";
            int best = Mathf.Max(PlayerPrefs.GetInt(key, 0), correctCount);
            PlayerPrefs.SetInt(key, best);

            ui.ShowEndPanel("¡Se acabó el tiempo!",
                $"Aciertos: {correctCount}\nMejor puntuación: {best}",
                false);
        }

        PlayerPrefs.Save();   // necesario en WebGL para que se guarde en el navegador
    }

    void UpdateScoreUI()
    {
        ui.SetScore(correctCount, mode == GameMode.Basico ? level.correctToComplete : 0);
        ui.SetProgress(Progress);
    }

    void SetPikminsInteractable(bool value)
    {
        foreach (var p in pikmins) p.SetInteractable(value);
    }

    // ===== Botones del panel de fin de partida =====

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        GameSession.Select(mode, levelIndex + 1);
        RestartLevel();
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);
    }
}
