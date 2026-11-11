using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hace aparecer flores por el prado según el progreso del nivel (apartado 6.3 del GDD).
/// Con progreso 1 (100 %) el prado queda cubierto.
///
/// Colócalo en un RectTransform que ocupe la zona de césped y que esté
/// DETRÁS de los Pikmin en la jerarquía (más arriba en la lista), para que
/// las flores no tapen los Pikmin ni bloqueen los clics.
/// </summary>
public class FlowerField : MonoBehaviour
{
    [Tooltip("Zona donde pueden aparecer flores. Si está vacío, usa este mismo objeto.")]
    [SerializeField] RectTransform area;

    [Tooltip("Sprites de flores de distintos colores.")]
    [SerializeField] Sprite[] flowerSprites;

    [Tooltip("Flores que habrá con el prado al 100 %.")]
    [SerializeField] int totalFlowers = 40;

    [SerializeField] Vector2 sizeRange = new Vector2(35f, 70f);
    [SerializeField] float popDuration = 0.25f;

    readonly List<RectTransform> spawned = new List<RectTransform>();

    void Awake()
    {
        if (area == null) area = (RectTransform)transform;
    }

    public void Clear()
    {
        foreach (var flower in spawned)
            if (flower != null) Destroy(flower.gameObject);
        spawned.Clear();
    }

    /// <summary>Progreso entre 0 y 1. Solo añade flores, nunca las quita.</summary>
    public void SetProgress(float progress)
    {
        int target = Mathf.RoundToInt(totalFlowers * Mathf.Clamp01(progress));
        while (spawned.Count < target)
            SpawnFlower();
    }

    void SpawnFlower()
    {
        if (flowerSprites == null || flowerSprites.Length == 0)
        {
            Debug.LogWarning("FlowerField: no hay sprites de flores asignados.");
            return;
        }

        var go = new GameObject("Flor", typeof(RectTransform), typeof(Image));
        var rect = (RectTransform)go.transform;
        rect.SetParent(area, false);

        var image = go.GetComponent<Image>();
        image.sprite = flowerSprites[Random.Range(0, flowerSprites.Length)];
        image.preserveAspect = true;
        image.raycastTarget = false;   // no bloquea los clics

        float size = Random.Range(sizeRange.x, sizeRange.y);
        rect.sizeDelta = new Vector2(size, size);

        Rect r = area.rect;
        rect.anchoredPosition = new Vector2(
            Random.Range(r.xMin + size * 0.5f, r.xMax - size * 0.5f),
            Random.Range(r.yMin + size * 0.5f, r.yMax - size * 0.5f));
        rect.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-15f, 15f));

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
            // Pequeño rebote: crece un poco más y vuelve a su tamaño
            float k = t / popDuration;
            float scale = k < 0.7f ? Mathf.Lerp(0f, 1.2f, k / 0.7f) : Mathf.Lerp(1.2f, 1f, (k - 0.7f) / 0.3f);
            rect.localScale = Vector3.one * scale;
            yield return null;
        }
        if (rect != null) rect.localScale = Vector3.one;
    }
}
