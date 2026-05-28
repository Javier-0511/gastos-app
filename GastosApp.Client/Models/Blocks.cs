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
    public static readonly string[] Shared = { "fijo", "comida", "variable", "minicompra" };

    // Bloques de la cuenta personal. 'individual' NO se ofrece para nuevas
    // categorías (es legacy), pero sigue siendo válido en la BBDD para no
    // romper las categorías personales antiguas hasta que se reasignen.
    public static readonly string[] Personal = { "fijo", "ocio", "variable", "inversion" };

    /// <summary>Bloques disponibles para crear categorías según el tipo de cuenta.</summary>
    public static string[] For(bool isShared) => isShared ? Shared : Personal;

    /// <summary>Etiqueta legible de un bloque.</summary>
    public static string Label(string block) => block switch
    {
        "fijo" => "Fijos",
        "comida" => "Comida",
        "variable" => "Variables",
        "minicompra" => "Minicompras",
        "individual" => "Individual",
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
        "minicompra" => 4,
        "ocio" => 5,
        "inversion" => 6,
        "individual" => 7,
        _ => 99
    };
}
