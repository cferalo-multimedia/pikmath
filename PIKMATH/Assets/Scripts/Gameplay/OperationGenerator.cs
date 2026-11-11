using UnityEngine;

/// <summary>
/// Una operación generada: el texto que se muestra y su resultado.
/// </summary>
public struct MathOperation
{
    public string text;   // Ej: "4 + 1 = ?"
    public int result;    // Ej: 5

    /// <summary>Texto con el resultado en lugar del interrogante (Ej: "4 + 1 = 5").</summary>
    public string SolvedText => text.Replace("?", result.ToString());
}

/// <summary>
/// Genera operaciones aleatorias cuyo resultado está entre 0 y maxResult,
/// para que siempre exista un Pikmin con la respuesta correcta (0-9).
/// </summary>
public static class OperationGenerator
{
    public static MathOperation Generate(OperationType type, int maxResult)
    {
        maxResult = Mathf.Clamp(maxResult, 2, 9);

        switch (type)
        {
            case OperationType.Resta:
                return GenerateSubtraction(maxResult);
            case OperationType.Combinada:
                return GenerateCombined(maxResult);
            default:
                return GenerateAddition(maxResult);
        }
    }

    // Sumas: a + b = r  (ej: 2 + 3, 4 + 1)
    static MathOperation GenerateAddition(int maxResult)
    {
        int result = Random.Range(2, maxResult + 1);   // 2..max
        int a = Random.Range(1, result);               // 1..result-1 (evita "+ 0")
        int b = result - a;

        return new MathOperation { text = $"{a} + {b} = ?", result = result };
    }

    // Restas: a - b = r, con a <= 9 y sin resultados negativos  (ej: 4 - 3, 8 - 1)
    static MathOperation GenerateSubtraction(int maxResult)
    {
        int result = Random.Range(0, maxResult);           // 0..max-1
        int b = Random.Range(1, maxResult - result + 1);   // b >= 1
        int a = result + b;                                // a <= max

        return new MathOperation { text = $"{a} - {b} = ?", result = result };
    }

    // Combinadas: a + b - c  o  a - b + c  (ej: 4 - 3 + 1, 4 - 1 + 2)
    // Ningún paso intermedio es negativo ni pasa de 10.
    static MathOperation GenerateCombined(int maxResult)
    {
        for (int attempt = 0; attempt < 200; attempt++)
        {
            int a = Random.Range(1, 10);
            int b = Random.Range(1, 10);
            int c = Random.Range(1, 10);
            bool addFirst = Random.value < 0.5f;

            int intermediate = addFirst ? a + b : a - b;
            int result = addFirst ? intermediate - c : intermediate + c;

            if (intermediate < 0 || intermediate > 10) continue;
            if (result < 0 || result > maxResult) continue;

            string text = addFirst ? $"{a} + {b} - {c} = ?" : $"{a} - {b} + {c} = ?";
            return new MathOperation { text = text, result = result };
        }

        // Por si acaso (no debería ocurrir nunca)
        return new MathOperation { text = "4 - 3 + 1 = ?", result = 2 };
    }
}
