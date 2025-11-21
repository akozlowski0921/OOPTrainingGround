using System;
using System.Collections.Generic;
using System.Linq;

namespace SOLID.SRP.Good3
{
    // ✅ Responsibility: Data access
    public class SalesDataRepository
    {
        private List<SalesData> _salesData = new List<SalesData>();

        public List<SalesData> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _salesData
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .ToList();
        }
    }
}
