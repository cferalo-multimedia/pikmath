using TMPro;
using UnityEngine;

/// <summary>
/// Una fila del ranking: avatar (círculo) + nombre + puntuación.
/// Va en cada una de las tres franjas de color del fondo menu-ranking.
/// </summary>
public class RankingRow : MonoBehaviour
{
    [SerializeField] AvatarView avatar;
    [SerializeField] TMP_Text positionText;   // opcional: "1º"
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text scoreText;

    [Tooltip("Opcional: se activa cuando la fila es la del jugador (marco, estrella…).")]
    [SerializeField] GameObject playerHighlight;

    public void Show(int position, string playerName, int score, AvatarData avatarData, bool isPlayer)
    {
        gameObject.SetActive(true);
        if (avatar != null) avatar.Show(avatarData);
        if (positionText != null) positionText.text = $"{position}º";
        if (nameText != null) nameText.text = playerName;
        if (scoreText != null) scoreText.text = score.ToString();
        if (playerHighlight != null) playerHighlight.SetActive(isPlayer);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
