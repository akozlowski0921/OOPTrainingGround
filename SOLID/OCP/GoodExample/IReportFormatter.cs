using System.Collections.Generic;

namespace SOLID.OCP.GoodExample
{
    /// <summary>
    /// Interfejs definiujący kontrakt dla formatowania raportów
    /// Dzięki temu możemy dodawać nowe formaty bez modyfikacji istniejącego kodu
    /// </summary>
    public interface IReportFormatter
    {
        string Format(List<SalesData> data);
    }
}
