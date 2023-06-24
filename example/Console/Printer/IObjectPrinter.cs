
namespace XboxAuthNetConsole
{
    public interface IObjectPrinter
    {
        bool CanPrint(object? obj);
        void Print(TextWriter writeTo, object? obj);
    }
}