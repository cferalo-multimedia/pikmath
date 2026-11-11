using UnityEngine;

/// <summary>
/// Tipo de operaciones que aparecen en un nivel.
/// </summary>
public enum OperationType
{
    Suma,
    Resta,
    Combinada
}

/// <summary>
/// Datos de configuración de un nivel (ver tabla del apartado 4 del GDD).
/// Se crea desde el menú: Create → PIKMATH → Datos de nivel.
/// </summary>
[CreateAssetMenu(fileName = "Nivel", menuName = "PIKMATH/Datos de nivel")]
public class LevelData : ScriptableObject
{
    [Header("Identificación")]
    public int levelNumber = 1;
    public string levelName = "Prado verde";

    [Header("Operaciones")]
    public OperationType operationType = OperationType.Suma;

    [Tooltip("Resultado máximo de las operaciones. Los Pikmin van del 0 al 9, así que no puede pasar de 9.")]
    [Range(2, 9)] public int maxResult = 9;

    [Header("Modo básico")]
    [Tooltip("Segundos para resolver cada operación (Velocidad en el GDD).")]
    public float timePerOperation = 50f;

    [Tooltip("Aciertos necesarios para llenar el prado de flores (Reto en el GDD).")]
    public int correctToComplete = 5;

    [Header("Modo contrarreloj")]
    [Tooltip("Duración total de la partida en segundos.")]
    public float timeAttackDuration = 60f;

    [Tooltip("Probabilidad (0-1) de que aparezca el reloj de tiempo extra en cada operación.")]
    [Range(0f, 1f)] public float clockChance = 0.25f;

    [Tooltip("Segundos que tiene el jugador para conseguir el reloj.")]
    public float clockTimeLimit = 10f;

    [Tooltip("Segundos que suma el reloj al tiempo total.")]
    public float clockBonus = 5f;

    [Header("Ambientación")]
    [Tooltip("Fondo del nivel: prado verde, otoñal o nocturno.")]
    public Sprite background;
}
