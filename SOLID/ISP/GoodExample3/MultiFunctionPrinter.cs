namespace SOLID.ISP.Good3
{
    public class MultiFunctionPrinter : IPrintable, IScannable, IFaxable, IStapler, IDuplexPrintable
    {
        public void Print() { }
        public void Scan() { }
        public void Fax() { }
        public void Staple() { }
        public void DuplexPrint() { }
    }
}
