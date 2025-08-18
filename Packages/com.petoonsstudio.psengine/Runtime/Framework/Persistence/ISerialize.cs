namespace PetoonsStudio.PSEngine.Framework
{
    /// <summary>
    /// Serialize
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISerialize<T>
    {
        T Serialize();

        void Deserialize(T data);
    }
}