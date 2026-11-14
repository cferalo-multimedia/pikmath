
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Pikmin : MonoBehaviour
{
    public enum State { Escondido, Asomandose, Fuera }

    [Header("Datos")]
    [SerializeField] int number;
    [SerializeField] TMP_Text numberLabel;

    [Header("Animación")]
    [SerializeField] RectTransform body;
    [SerializeField] float hiddenOffset = -180f;
    [SerializeField] float moveDuration = 0.2f;
    [SerializeField] float reappearDelay = 0.8f;

    [Header("Sonidos (opcional)")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip happySound;
    [SerializeField] AudioClip sadSound;

    public int Number => number;
    public State CurrentState { get; private set; } = State.Asomandose;

    public event Action<Pikmin> Clicked;

    Button button;
    Vector2 peekPosition;
    Coroutine currentAnimation;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Clicked?.Invoke(this));

        if (body == null)
            body = (RectTransform)transform;

        peekPosition = body.anchoredPosition;
        CurrentState = State.Asomandose;

        if (numberLabel != null)
            numberLabel.text = number.ToString();
    }

    public void SetInteractable(bool value)
    {
        button.interactable =
            value && CurrentState != State.Escondido;
    }

    public void PlayCorrect()
    {
        Play(happySound, CorrectRoutine());
    }

    public void PlayWrong()
    {
        Play(sadSound, WrongRoutine());
    }

    void Play(AudioClip sound, IEnumerator routine)
    {
        if (audioSource != null && sound != null)
            audioSource.PlayOneShot(sound);

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(routine);
    }

    // PRUEBA: al acertar, el Pikmin no se mueve.
    IEnumerator CorrectRoutine()
    {
        CurrentState = State.Fuera;

        // Sube 2 unidades desde su posición inicial.
        Vector2 outPosition = peekPosition + Vector2.up * 15f;

        yield return MoveTo(outPosition, 0.3f);

        yield return new WaitForSeconds(0.5f);

        // Regresa suavemente a su posición inicial.
        yield return MoveTo(peekPosition, 0.3f);

        CurrentState = State.Asomandose;
        currentAnimation = null;
    }

    // Al fallar, se esconde y vuelve a asomar.
    IEnumerator WrongRoutine()
    {
        CurrentState = State.Escondido;
        button.interactable = false;

        Vector2 hiddenPosition =
            peekPosition + Vector2.down * Mathf.Abs(hiddenOffset);

        yield return MoveTo(hiddenPosition, moveDuration);

        yield return new WaitForSeconds(reappearDelay);

        yield return MoveTo(peekPosition, moveDuration);

        CurrentState = State.Asomandose;
        button.interactable = true;
        currentAnimation = null;
    }

    IEnumerator MoveTo(Vector2 target, float duration)
    {
        Vector2 start = body.anchoredPosition;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float k = Mathf.SmoothStep(
                0f, 1f, t / duration);

            body.anchoredPosition =
                Vector2.Lerp(start, target, k);

            yield return null;
        }

        body.anchoredPosition = target;
    }
}
