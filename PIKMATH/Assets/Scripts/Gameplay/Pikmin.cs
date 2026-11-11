using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Un Pikmin plantado en un hoyo, con su número (apartado 5 del GDD).
/// Se pulsa con el ratón (o el dedo) para responder.
///
/// Estructura recomendada del objeto:
///   Pikmin_5          → Button + Image (transparente, área de clic) + RectMask2D + este script
///     └── Body        → Image con el sprite del Pikmin (es lo que se mueve)
///           └── Number → TextMeshPro con el número (sobre la hoja)
/// El RectMask2D hace que, al bajar, el Pikmin "desaparezca" dentro del hoyo.
/// </summary>
[RequireComponent(typeof(Button))]
public class Pikmin : MonoBehaviour
{
    public enum State { Escondido, Asomandose, Fuera }

    [Header("Datos")]
    [Tooltip("Número del Pikmin (0-9). Debe ser distinto en cada uno.")]
    [SerializeField] int number;
    [SerializeField] TMP_Text numberLabel;

    [Header("Animación")]
    [Tooltip("Parte que se mueve (el sprite del Pikmin).")]
    [SerializeField] RectTransform body;
    [Tooltip("Cuánto baja al esconderse (negativo).")]
    [SerializeField] float hiddenOffset = -180f;
    [Tooltip("Cuánto sube al salir del suelo.")]
    [SerializeField] float outOffset = 60f;
    [SerializeField] float moveDuration = 0.2f;
    [Tooltip("Segundos que tarda en volver a asomarse tras un fallo.")]
    [SerializeField] float reappearDelay = 0.8f;

    [Header("Sonidos (opcional)")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip happySound;
    [SerializeField] AudioClip sadSound;

    public int Number => number;
    public State CurrentState { get; private set; } = State.Asomandose;

    /// <summary>Se lanza cuando el jugador pulsa este Pikmin.</summary>
    public event Action<Pikmin> Clicked;

    Button button;
    Vector2 peekPosition;
    Coroutine currentAnimation;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Clicked?.Invoke(this));

        if (body == null) body = (RectTransform)transform;
        peekPosition = body.anchoredPosition;

        if (numberLabel != null) numberLabel.text = number.ToString();
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value && CurrentState != State.Escondido;
    }

    /// <summary>Respuesta correcta: sale del suelo, salta de alegría y vuelve a su sitio.</summary>
    public void PlayCorrect()
    {
        Play(sound: happySound, routine: CorrectRoutine());
    }

    /// <summary>Respuesta incorrecta: se esconde bajo tierra y vuelve a asomarse.</summary>
    public void PlayWrong()
    {
        Play(sound: sadSound, routine: WrongRoutine());
    }

    void Play(AudioClip sound, IEnumerator routine)
    {
        if (audioSource != null && sound != null) audioSource.PlayOneShot(sound);
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(routine);
    }

    IEnumerator CorrectRoutine()
    {
        CurrentState = State.Fuera;
        Vector2 outPosition = peekPosition + Vector2.up * outOffset;
        yield return MoveTo(outPosition, moveDuration);

        // Dos saltitos de alegría
        for (int i = 0; i < 2; i++)
        {
            yield return MoveTo(outPosition + Vector2.up * 25f, 0.1f);
            yield return MoveTo(outPosition, 0.1f);
        }

        yield return new WaitForSeconds(0.3f);
        yield return MoveTo(peekPosition, moveDuration);
        CurrentState = State.Asomandose;
    }

    IEnumerator WrongRoutine()
    {
        CurrentState = State.Escondido;
        button.interactable = false;
        yield return MoveTo(peekPosition + Vector2.up * hiddenOffset, moveDuration);

        yield return new WaitForSeconds(reappearDelay);

        yield return MoveTo(peekPosition, moveDuration);
        CurrentState = State.Asomandose;
        button.interactable = true;
    }

    IEnumerator MoveTo(Vector2 target, float duration)
    {
        Vector2 start = body.anchoredPosition;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            body.anchoredPosition = Vector2.Lerp(start, target, k);
            yield return null;
        }
        body.anchoredPosition = target;
    }
}
