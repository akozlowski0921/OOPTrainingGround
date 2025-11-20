namespace SOLID.ISP.GoodExample
{
    /// <summary>
    /// Małe, wyspecjalizowane interfejsy - każde urządzenie implementuje tylko to, czego potrzebuje
    /// </summary>
    
    public interface IPrinter
    {
        void Print(string document);
    }

    public interface IScanner
    {
        void Scan(string document);
    }

    public interface IFax
    {
        void Fax(string document);
    }

    public interface ICopier
    {
        void Copy(string document);
    }

    public interface IEmailSender
    {
        void SendEmail(string to, string subject, string body);
    }

    public interface IPhone
    {
        void MakeCall(string number);
    }

    public interface IWebBrowser
    {
        void BrowseInternet(string url);
    }
}
