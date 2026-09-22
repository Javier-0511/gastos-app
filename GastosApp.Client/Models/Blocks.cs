namespace GastosApp.Client.Models;

/// <summary>
/// Definición central de los bloques de categorías.
/// Único sitio donde se listan los bloques y sus etiquetas, para no
/// duplicarlos en cada página (antes estaban repetidos en MonthView,
/// NewExpense y Categories y se desincronizaban con facilidad).
/// </summary>
public static class Blocks
{
    // Bloques de la cuenta compartida (tarjeta conjunta).
    // 'minicompra' se eliminó (2026-09-22): sus categorías (Chino, Otros)
    // pasaron a 'variable', que es donde encajaban de verdad.
    public static readonly string[] Shared = { "fijo", "comida", "variable" };

    // Bloques de la cuenta personal. El antiguo 'individual' se eliminó
    // (2026-09-22) una vez comprobado que ya no quedaba ninguna categoría
    // usándolo.
    public static readonly string[] Personal = { "fijo", "ocio", "variable", "inversion" };

    /// <summary>Bloques disponibles para crear categorías según el tipo de cuenta.</summary>
    public static string[] For(bool isShared) => isShared ? Shared : Personal;

    /// <summary>Etiqueta legible de un bloque.</summary>
    public static string Label(string block) => block switch
    {
        "fijo" => "Fijos",
        "comida" => "Comida",
        "variable" => "Variables",
        "ocio" => "Ocio",
        "inversion" => "Inversiones",
        _ => block
    };

    /// <summary>Orden de presentación de los bloques.</summary>
    public static int Order(string block) => block switch
    {
        "fijo" => 1,
        "comida" => 2,
        "variable" => 3,
        "ocio" => 4,
        "inversion" => 5,
        _ => 99
    };
}
