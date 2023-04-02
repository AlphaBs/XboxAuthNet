using System.Linq;

namespace XboxAuthNetConsole.Printer
{
    public class PrinterCollection : IObjectPrinter
    {
        private readonly IEnumerable<IObjectPrinter> _printers;

        public PrinterCollection(IEnumerable<IObjectPrinter> printers)
        {
            this._printers = printers;
        }

        public bool CanPrint(object? obj)
        {
            if (obj == null) return true;
            return _printers.Any(printer => printer.CanPrint(obj));
        }

        public void Print(TextWriter writer, object? obj)
        {
            var printer = _printers.FirstOrDefault(printer => printer.CanPrint(obj));

            if (printer != null)
            {
                printer.Print(writer, obj);
            }
            else
            {
                writer.WriteLine(obj?.ToString());
            }
        }
    }
}