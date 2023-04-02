
namespace XboxAuthNetConsole.Serializer
{
    public interface ISerializer<T>
    {
        Task<T?> Load();
        Task Save(T? obj);
    }
}