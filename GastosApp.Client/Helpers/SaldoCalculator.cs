namespace GastosApp.Client.Helpers;

/// <summary>
/// Lógica financiera pura del saldo mensual: solo recibe números y devuelve
/// números, sin saber nada de Supabase ni de la interfaz. Aislada aquí para
/// poder testearla (ver backlog A2) y para no duplicar la fórmula entre el
/// saldo del mes en curso y el arrastre del saldo de meses anteriores.
/// </summary>
public static class SaldoCalculator
{
    /// <summary>
    /// Lo que sale de la cuenta personal hacia la compartida: un porcentaje
    /// (share) del aporte conjunto del mes.
    /// </summary>
    public static decimal AporteACompartida(decimal aporteConjunto, decimal share) =>
        aporteConjunto * share;

    /// <summary>
    /// Saldo final de la cuenta compartida:
    /// saldo_inicial + aporte_conjunto − gastos.
    /// </summary>
    public static decimal SaldoFinalCompartida(decimal opening, decimal aporteConjunto, decimal gastos) =>
        opening + aporteConjunto - gastos;

    /// <summary>
    /// Saldo final de la cuenta personal:
    /// saldo_inicial + nómina − aporte_a_compartida − gastos.
    /// </summary>
    public static decimal SaldoFinalPersonal(decimal opening, decimal nomina, decimal aporteACompartida, decimal gastos) =>
        opening + nomina - aporteACompartida - gastos;
}
