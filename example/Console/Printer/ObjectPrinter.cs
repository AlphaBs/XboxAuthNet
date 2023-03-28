using System.Linq;

namespace XboxAuthNetConsole.Printer
{
    public class ObjectPrinter : IObjectPrinter
    {
        private readonly IEnumerable<IObjectPrinter> _printers;

        public ObjectPrinter(IEnumerable<IObjectPrinter> printers)
        {
            this._printers = printers;
        }

        public bool CanPrint(object obj)
        {
            return _printers.Any(printer => printer.CanPrint(obj));
        }

        public void Print(TextWriter writer, object obj)
        {
            var printer = _printers.FirstOrDefault(printer => printer.CanPrint(obj));
            if (printer == null)
                throw new PrinterException(obj);
            
            printer.Print(writer, obj);
        }
    }
}