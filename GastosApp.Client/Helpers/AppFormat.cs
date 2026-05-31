using System.Globalization;

namespace GastosApp.Client.Helpers;

/// <summary>
/// Helpers de formato compartidos por todas las páginas.
/// Antes FormatMoney y CultureInfo("es-ES") se repetían en
/// MonthView, Dashboard y Home.
/// </summary>
public static class AppFormat
{
    public static readonly CultureInfo Es = new("es-ES");

    public static string Money(decimal amount) =>
        amount.ToString("N2", Es) + " €";
}
