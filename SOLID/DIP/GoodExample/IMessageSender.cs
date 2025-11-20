namespace SOLID.DIP.GoodExample
{
    /// <summary>
    /// Abstrakcja (interfejs) dla wysyłania wiadomości
    /// To jest "inwersja" - moduły wysokopoziomowe i niskopoziomowe zależą od tej abstrakcji
    /// </summary>
    public interface IMessageSender
    {
        void SendMessage(string to, string subject, string body);
    }
}
