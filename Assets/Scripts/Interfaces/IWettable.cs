public interface IWettable
{
    /// <summary>
    /// Nivel de humedad del objeto (0 = seco, 100 = completamente mojado).
    /// </summary>
    int Wetness { get;}

    /// <summary>
    /// Añade humedad al objeto (clamp entre 0 y 100).
    /// </summary>
    /// <param name="amount">Cantidad de humedad a añadir.</param>
    void AddWetness(int amount);
    void RemoveWetness(int amount);
    void SetWetness(int value);
}