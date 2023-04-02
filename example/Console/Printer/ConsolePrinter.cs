
namespace XboxAuthNetConsole.Printer
{
    public class ConsolePrinter
    {
        private static IObjectPrinter? printerInstance;
        public static IObjectPrinter GetPrinter()
        {
            if (printerInstance == null)
                printerInstance = initializePrinter();
            return printerInstance;
        }

        private static IObjectPrinter initializePrinter()
        {
            return new PrinterCollection(new IObjectPrinter[]
            {
                new OAuthObjectPrinter(),
                new XboxAuthObjectPrinter()
            });
        }

        public static void Print(object? obj)
        {
            GetPrinter().Print(Console.Out, obj);
        }
    }
}