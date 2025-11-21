namespace SOLID.ISP.Bad3
{
    // ❌ BAD: Not all printers have all features
    public interface IPrinter
    {
        void Print();
        void Scan();
        void Fax();
        void Staple();
        void DuplexPrint();
    }

    public class MultiFunctionPrinter : IPrinter
    {
        public void Print() { }
        public void Scan() { }
        public void Fax() { }
        public void Staple() { }
        public void DuplexPrint() { }
    }

    public class SimplePrinter : IPrinter
    {
        public void Print() { }
        
        // Problem: Simple printer doesn't have these features
        public void Scan() { throw new System.NotImplementedException(); }
        public void Fax() { throw new System.NotImplementedException(); }
        public void Staple() { throw new System.NotImplementedException(); }
        public void DuplexPrint() { throw new System.NotImplementedException(); }
    }
}
