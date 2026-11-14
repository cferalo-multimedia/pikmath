
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerField : MonoBehaviour
{
    [Header("Zona de aparición")]
    [SerializeField] RectTransform area;

    [Header("Flores")]
    [SerializeField] Sprite[] flowerSprites;
    [SerializeField] int totalFlowers = 40;

    [Header("Tamaño y animación")]
    [SerializeField] Vector2 sizeRange = new Vector2(35f, 70f);
    [SerializeField] float popDuration = 0.25f;

    [Header("Distribución")]
    [SerializeField] float minDistance = 75f;
    [SerializeField] int maxAttempts = 30;

    readonly List<RectTransform> spawned = new List<RectTransform>();
    readonly List<Vector2> positions = new List<Vector2>();

    void Awake()
    {
        if (area == null)
            area = (RectTransform)transform;
    }

    public void Clear()
    {
        foreach (var flower in spawned)
        {
            if (flower != null)
                Destroy(flower.gameObject);
        }

        spawned.Clear();
        positions.Clear();
    }

    public void SetProgress(float progress)
    {
        if (flowerSprites == null || flowerSprites.Length == 0)
        {
            Debug.LogWarning("FlowerField: no hay sprites asignados.");
            return;
        }

        int target = Mathf.RoundToInt(
            totalFlowers * Mathf.Clamp01(progress));

        while (spawned.Count < target)
            SpawnFlower();
    }

    void SpawnFlower()
    {
        Rect r = area.rect;

        float size = Random.Range(sizeRange.x, sizeRange.y);

        Vector2 position = Vector2.zero;
        bool found = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            float x = Random.Range(
                r.xMin + size * 0.5f,
                r.xMax - size * 0.5f);

            float y = Random.Range(
                r.yMin + size * 0.5f,
                r.yMax - size * 0.5f);

            Vector2 candidate = new Vector2(x, y);

            bool separated = true;

            foreach (Vector2 previous in positions)
            {
                if (Vector2.Distance(candidate, previous) < minDistance)
                {
                    separated = false;
                    break;
                }
            }

            if (separated)
            {
                position = candidate;
                found = true;
                break;
            }
        }

        // Si no encuentra espacio, usa una posición aleatoria.
        if (!found)
        {
            position = new Vector2(
                Random.Range(r.xMin + size * 0.5f, r.xMax - size * 0.5f),
                Random.Range(r.yMin + size * 0.5f, r.yMax - size * 0.5f));
        }

        var go = new GameObject(
            "Flor",
            typeof(RectTransform),
            typeof(Image));

        var rect = (RectTransform)go.transform;
        rect.SetParent(area, false);

        var image = go.GetComponent<Image>();
        image.sprite = flowerSprites[
            Random.Range(0, flowerSprites.Length)];

        image.preserveAspect = true;
        image.raycastTarget = false;

        rect.sizeDelta = new Vector2(size, size);
        rect.anchoredPosition = position;

        rect.localRotation = Quaternion.Euler(
            0f, 0f, Random.Range(-15f, 15f));

        positions.Add(position);
        spawned.Add(rect);

        StartCoroutine(Pop(rect));
    }

    IEnumerator Pop(RectTransform rect)
    {
        float t = 0f;
        rect.localScale = Vector3.zero;

        while (t < popDuration && rect != null)
        {
            t += Time.deltaTime;

            float k = t / popDuration;

            float scale = k < 0.7f
                ? Mathf.Lerp(0f, 1.2f, k / 0.7f)
                : Mathf.Lerp(1.2f, 1f, (k - 0.7f) / 0.3f);

            rect.localScale = Vector3.one * scale;

            yield return null;
        }

        if (rect != null)
            rect.localScale = Vector3.one;
    }
}
