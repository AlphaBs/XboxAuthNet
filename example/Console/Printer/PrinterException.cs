
namespace XboxAuthNetConsole
{
    public class PrinterException : Exception
    {
        public PrinterException()
         : base("Cannot print this object")
        {

        }

        public PrinterException(object? obj)
         : base("Cannot print this type of object: " + obj?.GetType()?.Name)
        {

        }
    }
}