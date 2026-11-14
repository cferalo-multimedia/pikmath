using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ranking estático (apartado 7.2.8 del GDD).
///
/// Los rivales son jugadores ficticios escritos en el Inspector. El jugador real
/// se añade con su apodo, su avatar y la suma de sus récords en el modo contrarreloj
/// (los guarda GameManager). Se ordena por puntuación y se muestran los primeros
/// puestos en las filas del fondo (3 en menu-ranking).
///
/// Más adelante se podría sustituir la lista de rivales por datos de un servidor
/// para tener el ranking provincial en línea.
/// </summary>
public class RankingController : MonoBehaviour
{
    [Serializable]
    public class RankingEntry
    {
        public string name = "Jugador";
        public int score = 0;
        public AvatarData avatar = AvatarData.Preset(0);
    }

    [Header("Filas (de arriba abajo)")]
    [SerializeField] RankingRow[] rows;

    [Header("Rivales ficticios")]
    [SerializeField] List<RankingEntry> rivals = new List<RankingEntry>
    {
        new RankingEntry { name = "Lucía",  score = 42, avatar = AvatarData.Preset(0) },
        new RankingEntry { name = "Pau",    score = 35, avatar = AvatarData.Preset(1) },
        new RankingEntry { name = "Martina",score = 28, avatar = AvatarData.Preset(2) },
        new RankingEntry { name = "Hugo",   score = 21, avatar = AvatarData.Preset(3) },
        new RankingEntry { name = "Noa",    score = 14, avatar = AvatarData.Preset(4) },
        new RankingEntry { name = "Jordi",  score = 8,  avatar = AvatarData.Preset(5) },
    };

    [Header("Jugador")]
    [Tooltip("Si está desactivado, el jugador solo aparece cuando ha jugado algún contrarreloj.")]
    [SerializeField] bool showPlayerWithZero = true;

    [Tooltip("Opcional: texto con la posición del jugador si no entra en las filas visibles.")]
    [SerializeField] TMP_Text playerPositionText;

    [Header("Navegación")]
    [SerializeField] string returnScene = "MenuPrincipal";

    SceneTransition transicion;

    void Awake()
    {
        transicion = GetComponent<SceneTransition>();
    }

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int playerScore = PlayerProfile.GetTotalScore();

        // Lista con los rivales + el jugador (marcado con isPlayer)
        var entries = rivals
            .Select(r => (entry: r, isPlayer: false))
            .ToList();

        if (showPlayerWithZero || playerScore > 0)
        {
            entries.Add((new RankingEntry
            {
                name = PlayerProfile.DisplayName(),
                score = playerScore,
                avatar = PlayerProfile.Avatar
            }, true));
        }

        // Ordena de mayor a menor. En caso de empate, el jugador va delante.
        entries = entries
            .OrderByDescending(e => e.entry.score)
            .ThenByDescending(e => e.isPlayer)
            .ToList();

        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i] == null) continue;
            if (i < entries.Count)
            {
                var e = entries[i];
                rows[i].Show(i + 1, e.entry.name, e.entry.score, e.entry.avatar, e.isPlayer);
            }
            else
            {
                rows[i].Hide();
            }
        }

        // Si el jugador no sale en las filas visibles, mostramos su puesto aparte
        if (playerPositionText != null)
        {
            int playerIndex = entries.FindIndex(e => e.isPlayer);
            bool outside = playerIndex >= rows.Length;
            playerPositionText.gameObject.SetActive(outside);
            if (outside)
                playerPositionText.text = $"Tu puesto: {playerIndex + 1}º · {playerScore} aciertos";
        }
    }

    public void Volver()
    {
        if (transicion != null)
            transicion.LoadSceneWithFade(returnScene);
        else
            SceneManager.LoadScene(returnScene);
    }
}
