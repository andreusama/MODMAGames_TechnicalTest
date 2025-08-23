public interface IWettable
{
    /// <summary>
    /// Wetness level (0 = dry, 100 = fully wet).
    /// </summary>
    int Wetness { get; }

    /// <summary>
    /// Adds wetness to the object (clamped between 0 and 100).
    /// </summary>
    /// <param name="amount">Wetness amount to add.</param>
    void AddWetness(int amount);
    void RemoveWetness(int amount);
    void SetWetness(int value);
}