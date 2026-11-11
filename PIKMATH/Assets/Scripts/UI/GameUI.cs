using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD y OSD de la pantalla de juego (apartado 7.3 del GDD).
/// Solo muestra información: la lógica está en GameManager.
/// Los campos marcados como opcionales se pueden dejar vacíos.
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("HUD")]
    [Tooltip("Panel de operaciones (arriba en el centro).")]
    [SerializeField] TMP_Text operationText;
    [Tooltip("Contador con formato 00:20.")]
    [SerializeField] TMP_Text timerText;
    [Tooltip("Puntuación de aciertos.")]
    [SerializeField] TMP_Text scoreText;
    [Tooltip("Opcional: barra de progreso del nivel.")]
    [SerializeField] Slider progressBar;

    [Header("Reloj de tiempo extra (contrarreloj)")]
    [SerializeField] GameObject clockGroup;
    [SerializeField] TMP_Text clockText;

    [Header("OSD (mensajes temporales)")]
    [SerializeField] TMP_Text messageText;
    [SerializeField] float messageDuration = 1.2f;
    [SerializeField] Color correctColor = new Color(0.2f, 0.65f, 0.2f);
    [SerializeField] Color wrongColor = new Color(0.85f, 0.25f, 0.25f);

    [Header("Fin de partida")]
    [SerializeField] GameObject endPanel;
    [SerializeField] TMP_Text endTitleText;
    [SerializeField] TMP_Text endResultText;
    [Tooltip("Opcional: botón «Siguiente nivel», solo se muestra si existe.")]
    [SerializeField] GameObject nextLevelButton;

    Coroutine messageRoutine;

    void Awake()
    {
        if (messageText != null) messageText.gameObject.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);
        HideClock();
    }

    public void SetOperation(string text)
    {
        if (operationText != null) operationText.text = text;
    }

    public void SetTimer(float seconds)
    {
        if (timerText == null) return;
        int s = Mathf.CeilToInt(Mathf.Max(0f, seconds));
        timerText.text = $"{s / 60:00}:{s % 60:00}";
    }

    /// <summary>Con target > 0 muestra "3/5"; si no, solo el número de aciertos.</summary>
    public void SetScore(int correct, int target)
    {
        if (scoreText == null) return;
        scoreText.text = target > 0 ? $"{correct}/{target}" : correct.ToString();
    }

    public void SetProgress(float progress)
    {
        if (progressBar != null) progressBar.value = Mathf.Clamp01(progress);
    }

    public void ShowClock(float seconds)
    {
        if (clockGroup != null) clockGroup.SetActive(true);
        UpdateClock(seconds);
    }

    public void UpdateClock(float seconds)
    {
        if (clockText != null) clockText.text = $"{Mathf.CeilToInt(Mathf.Max(0f, seconds))}s";
    }

    public void HideClock()
    {
        if (clockGroup != null) clockGroup.SetActive(false);
    }

    public void ShowMessage(string message, bool positive)
    {
        if (messageText == null) return;
        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(MessageRoutine(message, positive ? correctColor : wrongColor));
    }

    IEnumerator MessageRoutine(string message, Color color)
    {
        messageText.text = message;
        messageText.color = color;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        messageText.gameObject.SetActive(false);
    }

    public void ShowEndPanel(string title, string result, bool showNextLevel)
    {
        if (endPanel == null) return;
        if (endTitleText != null) endTitleText.text = title;
        if (endResultText != null) endResultText.text = result;
        if (nextLevelButton != null) nextLevelButton.SetActive(showNextLevel);
        endPanel.SetActive(true);
    }
}
